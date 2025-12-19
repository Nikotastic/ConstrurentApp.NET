# Complete diagnostic script for the chatbot

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║     COMPLETE DIAGNOSTIC - FIRMNESS CHATBOT                ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Read API Key from .env
$envPath = ".\.env"
$apiKey = $null

if (Test-Path $envPath) {
    $envContent = Get-Content $envPath -Raw
    if ($envContent -match 'GEMINI_API_KEY=(\S+)') {
        $apiKey = $matches[1]
        $keyPreview = $apiKey.Substring(0, [Math]::Min(15, $apiKey.Length)) + "..." + $apiKey.Substring([Math]::Max(0, $apiKey.Length - 4))
        Write-Host "✓ API Key found in .env: $keyPreview" -ForegroundColor Green
    }
}

if (-not $apiKey) {
    Write-Host "✗ Could not read API Key from .env" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "TEST 1: Direct connection with Gemini API" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host ""

$testBody = @{
    contents = @(
        @{
            parts = @(
                @{
                    text = "Say 'Works correctly' in English"
                }
            )
        }
    )
} | ConvertTo-Json -Depth 10

Write-Host "Testing with model: gemini-2.0-flash" -ForegroundColor Gray

try {
    $response = Invoke-RestMethod -Uri "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=$apiKey" `
        -Method POST `
        -ContentType "application/json" `
        -Body $testBody `
        -TimeoutSec 10
    
    $geminiResponse = $response.candidates[0].content.parts[0].text
    Write-Host "✓ Gemini API responds correctly" -ForegroundColor Green
    Write-Host "  Response: $geminiResponse" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "✗ Error connecting with Gemini API" -ForegroundColor Red
    Write-Host "  $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $errorBody = $reader.ReadToEnd()
            Write-Host "  Details: $errorBody" -ForegroundColor Red
        } catch {}
    }
    Write-Host ""
    Write-Host "⚠ If you see a 404 error, verify that the model is available." -ForegroundColor Yellow
    Write-Host "  Try changing to 'gemini-pro' in appsettings.Development.json" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "TEST 2: Firmness API Status" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host ""

try {
    $health = Invoke-RestMethod -Uri "http://localhost:5000/api/chat/health" -Method GET -TimeoutSec 3
    Write-Host "✓ Firmness API is running" -ForegroundColor Green
    Write-Host "  Status: $($health.status)" -ForegroundColor Gray
    Write-Host "  Service: $($health.service)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "✗ Firmness API NOT responding" -ForegroundColor Red
    Write-Host "  Make sure to start it with: docker-compose up -d" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "TEST 3: Echo endpoint (without Gemini)" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host ""

$echoBody = @{
    message = "Test"
} | ConvertTo-Json

try {
    $echoResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/chat/echo" -Method POST -ContentType "application/json" -Body $echoBody
    Write-Host "✓ Endpoint /api/chat/echo works" -ForegroundColor Green
    Write-Host "  Response: $($echoResponse.message)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "✗ Endpoint /api/chat/echo failed" -ForegroundColor Red
    Write-Host "  $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "TEST 4: Full Chatbot (with Gemini)" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host ""

$chatBody = @{
    message = "Hola, ¿qué maquinaria tienen disponible?"
} | ConvertTo-Json

Write-Host "Sending: 'Hola, ¿qué maquinaria tienen disponible?'" -ForegroundColor Gray
Write-Host "Waiting for response (may take a few seconds)..." -ForegroundColor Gray
Write-Host ""

try {
    $chatResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/chat/message" -Method POST -ContentType "application/json" -Body $chatBody -TimeoutSec 30
    
    if ($chatResponse.message -like "*dificultades técnicas*" -or $chatResponse.message -like "*technical difficulties*") {
        Write-Host "⚠ RESPONSE WITH ERROR:" -ForegroundColor Yellow
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
        Write-Host $chatResponse.message -ForegroundColor Yellow
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Possible causes:" -ForegroundColor Red
        Write-Host "  1. The API needs to be restarted after changes" -ForegroundColor White
        Write-Host "  2. The gemini-2.0-flash model is not available" -ForegroundColor White
        Write-Host "  3. There is a problem in the integration code" -ForegroundColor White
        Write-Host ""
        Write-Host "Solutions:" -ForegroundColor Yellow
        Write-Host "  1. Restart the API: docker-compose restart api" -ForegroundColor White
        Write-Host "  2. Check API logs to see the specific error" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host "✓ CHATBOT WORKS CORRECTLY!" -ForegroundColor Green
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
        Write-Host $chatResponse.message -ForegroundColor White
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
        Write-Host ""
    }
} catch {
    Write-Host "✗ ERROR IN CHATBOT" -ForegroundColor Red
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $errorBody = $reader.ReadToEnd()
            Write-Host ""
            Write-Host "Server response:" -ForegroundColor Yellow
            $errorBody | ConvertFrom-Json | ConvertTo-Json -Depth 10 | Write-Host -ForegroundColor Gray
        } catch {
            Write-Host $errorBody -ForegroundColor Gray
        }
    }
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Red
}

Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                 END OF DIAGNOSTIC                         ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan


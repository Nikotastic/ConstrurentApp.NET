# Script to verify Gemini configuration

Write-Host "=== Verifying Gemini Configuration ===" -ForegroundColor Cyan
Write-Host ""

# 1. Verify environment variable
Write-Host "1. GEMINI_API_KEY environment variable:" -ForegroundColor Yellow
if ($env:GEMINI_API_KEY) {
    $keyLength = $env:GEMINI_API_KEY.Length
    $maskedKey = $env:GEMINI_API_KEY.Substring(0, [Math]::Min(10, $keyLength)) + "..." + $env:GEMINI_API_KEY.Substring([Math]::Max(0, $keyLength - 5))
    Write-Host "   ✓ Configured: $maskedKey" -ForegroundColor Green
} else {
    Write-Host "   ✗ NOT configured" -ForegroundColor Red
}
Write-Host ""

# 2. Verify appsettings.Development.json
Write-Host "2. appsettings.Development.json file:" -ForegroundColor Yellow
$appsettingsPath = ".\src\Firmness.Api\appsettings.Development.json"
if (Test-Path $appsettingsPath) {
    $content = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
    if ($content.Gemini.ApiKey -and $content.Gemini.ApiKey -ne "") {
        $keyLength = $content.Gemini.ApiKey.Length
        $maskedKey = $content.Gemini.ApiKey.Substring(0, [Math]::Min(10, $keyLength)) + "..." + $content.Gemini.ApiKey.Substring([Math]::Max(0, $keyLength - 5))
        Write-Host "   ✓ API Key configured: $maskedKey" -ForegroundColor Green
        Write-Host "   ✓ Model: $($content.Gemini.Model)" -ForegroundColor Green
    } else {
        Write-Host "   ✗ API Key NOT configured or empty" -ForegroundColor Red
        Write-Host "   Model: $($content.Gemini.Model)" -ForegroundColor Gray
    }
} else {
    Write-Host "   ✗ File not found" -ForegroundColor Red
}
Write-Host ""

# 3. Verify if API is running
Write-Host "3. API Status:" -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "http://localhost:5000/api/chat/health" -Method GET -TimeoutSec 3
    Write-Host "   ✓ API running" -ForegroundColor Green
    Write-Host "   Status: $($health.status)" -ForegroundColor Gray
} catch {
    Write-Host "   ✗ API not responding at http://localhost:5000" -ForegroundColor Red
}
Write-Host ""

# 4. Suggestions
Write-Host "=== Solutions ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "To configure the API Key, choose ONE of these options:" -ForegroundColor Yellow
Write-Host ""
Write-Host "OPTION 1: Environment Variable (Recommended for development)" -ForegroundColor Green
Write-Host '   $env:GEMINI_API_KEY = "YOUR_API_KEY_HERE"' -ForegroundColor White
Write-Host ""
Write-Host "OPTION 2: In appsettings.Development.json" -ForegroundColor Green
Write-Host '   Edit the file and add your API key in Gemini.ApiKey' -ForegroundColor White
Write-Host ""
Write-Host "To get your API Key:" -ForegroundColor Yellow
Write-Host "   1. Visit: https://aistudio.google.com/apikey" -ForegroundColor White
Write-Host "   2. Create or copy your API Key" -ForegroundColor White
Write-Host "   3. Configure it using one of the options above" -ForegroundColor White
Write-Host ""

# 5. Connection test with Gemini (if API key exists)
if ($env:GEMINI_API_KEY) {
    Write-Host "=== Gemini Connection Test ===" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Testing direct connection with Gemini API..." -ForegroundColor Yellow
    
    $testBody = @{
        contents = @(
            @{
                parts = @(
                    @{
                        text = "Say hello in Spanish"
                    }
                )
            }
        )
    } | ConvertTo-Json -Depth 10
    
    try {
        $response = Invoke-RestMethod -Uri "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key=$($env:GEMINI_API_KEY)" `
            -Method POST `
            -ContentType "application/json" `
            -Body $testBody `
            -TimeoutSec 10
        
        Write-Host "   ✓ Successful connection with Gemini API" -ForegroundColor Green
        Write-Host "   Response: $($response.candidates[0].content.parts[0].text)" -ForegroundColor Gray
    } catch {
        Write-Host "   ✗ Error connecting with Gemini API" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
        
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "   Details: $responseBody" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "=== Verification Finished ===" -ForegroundColor Cyan


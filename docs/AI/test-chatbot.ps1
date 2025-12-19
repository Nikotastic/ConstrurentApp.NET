Write-Host "=== Testing Firmness Chatbot ===" -ForegroundColor Cyan
Write-Host ""

# Verify that the API is running
Write-Host "1. Verifying API..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "http://localhost:5000/api/chat/health" -Method GET -TimeoutSec 3
    Write-Host "   ✓ API is running" -ForegroundColor Green
    Write-Host "   Status: $($health.status)" -ForegroundColor Gray
} catch {
    Write-Host "   ✗ API not responding. Make sure it is running." -ForegroundColor Red
    Write-Host "   Run: docker-compose up -d" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "2. Sending message to chatbot..." -ForegroundColor Yellow
Write-Host "   Message: 'Hola, que maquinaria tienen disponible?' (Hello, what machinery is available?)" -ForegroundColor Gray
Write-Host ""

$body = @{
    message = "Hola, que maquinaria tienen disponible?"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/chat/message" -Method POST -ContentType "application/json" -Body $body -TimeoutSec 30
    
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
    Write-Host "✓ CHATBOT RESPONSE:" -ForegroundColor Green
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
    Write-Host ""
    Write-Host $response.message -ForegroundColor White
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
    Write-Host "Timestamp: $($response.timestamp)" -ForegroundColor Gray
    Write-Host ""
    
    if ($response.message -like "*dificultades técnicas*" -or $response.message -like "*technical difficulties*") {
        Write-Host "⚠ The response indicates a technical error." -ForegroundColor Yellow
        Write-Host "  Check API logs for more details." -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Red
    Write-Host "✗ ERROR SENDING MESSAGE" -ForegroundColor Red
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    
    if ($_.Exception.Response) {
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Server details:" -ForegroundColor Yellow
            Write-Host $responseBody -ForegroundColor Gray
        } catch {}
    }
}

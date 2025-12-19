param (
    [string]$Email = "test@firmness.local"
)

$ErrorActionPreference = "Stop"

$apiUrl = "http://localhost:5000/api/paymenttest/simulate-purchase"

$body = @{
    customerName = "Test User"
    customerEmail = $email
    totalAmount = 2500.50
    items = @(
        @{
            description = "Heavy Machinery Rental"
            quantity = 1
            unitPrice = 2000.00
        },
        @{
            description = "Delivery Fee"
            quantity = 1
            unitPrice = 500.50
        }
    )
    simulateFailure = $false
} | ConvertTo-Json -Depth 3

Write-Host "🚀 Sending simulated purchase request..." -ForegroundColor Cyan

try {
    $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $body -ContentType "application/json"
    
    Write-Host "✅ Success!" -ForegroundColor Green
    Write-Host "Invoice: $($response.data.invoiceNumber)"
    Write-Host "Transaction: $($response.data.transactionId)"
    Write-Host "Check your email ($email) for the receipt!" -ForegroundColor Yellow
}
catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        Write-Host $reader.ReadToEnd()
    }
}

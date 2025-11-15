# Script para probar autenticación JWT
$baseUrl = "https://localhost:7192"

Write-Host "=== PRUEBA 1: Login ===" -ForegroundColor Cyan
try {
    $loginBody = @{
        email = "client@firmness.local"
        password = "Client123!"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
        -Method POST `
        -ContentType "application/json" `
        -Body $loginBody `
        -SkipCertificateCheck

    Write-Host "✅ Login exitoso" -ForegroundColor Green
    Write-Host "Token obtenido:" -ForegroundColor Yellow
    Write-Host $response.token
    Write-Host ""

    $token = $response.token

    # Prueba 2: Endpoint público (sin token)
    Write-Host "=== PRUEBA 2: Endpoint público (sin token) ===" -ForegroundColor Cyan
    $publicResponse = Invoke-RestMethod -Uri "$baseUrl/api/profile/public" `
        -Method GET `
        -SkipCertificateCheck
    Write-Host "✅ Endpoint público funciona" -ForegroundColor Green
    Write-Host ($publicResponse | ConvertTo-Json)
    Write-Host ""

    # Prueba 3: Endpoint protegido CON token
    Write-Host "=== PRUEBA 3: Endpoint protegido CON token ===" -ForegroundColor Cyan
    $headers = @{
        Authorization = "Bearer $token"
    }
    
    $profileResponse = Invoke-RestMethod -Uri "$baseUrl/api/profile/me" `
        -Method GET `
        -Headers $headers `
        -SkipCertificateCheck

    Write-Host "✅ Token JWT funciona correctamente!" -ForegroundColor Green
    Write-Host "Tu perfil:" -ForegroundColor Yellow
    Write-Host ($profileResponse | ConvertTo-Json -Depth 3)

} catch {
    Write-Host "❌ Error:" -ForegroundColor Red
    Write-Host $_.Exception.Message
    if ($_.Exception.Response) {
        Write-Host "Status Code:" $_.Exception.Response.StatusCode.value__
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response:" $responseBody
    }
}


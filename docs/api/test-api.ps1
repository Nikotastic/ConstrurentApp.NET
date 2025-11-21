# ========================================
# Full API Verification Script
# ========================================
# Puerto: 5086
# Swagger: http://localhost:5086/swagger/index.html
# ========================================

$baseUrl = "http://localhost:5086"
$results = @()

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  🧪 API VERIFICATION - FIRMNESS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "🌐 URL Base: $baseUrl" -ForegroundColor Yellow
Write-Host "📊 Swagger: $baseUrl/swagger/index.html" -ForegroundColor Yellow
Write-Host ""

# Función para probar endpoint
function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers = @{},
        [string]$Body = $null,
        [bool]$ExpectAuth = $false
    )
    
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $Headers
            ErrorAction = 'Stop'
            TimeoutSec = 10
        }
        
        if ($Body) {
            $params['Body'] = $Body
            $params['ContentType'] = 'application/json'
        }
        
        $response = Invoke-WebRequest @params
        
        if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300) {
            Write-Host "✅ $Name" -ForegroundColor Green
            return @{ Name = $Name; Status = "OK"; Code = $response.StatusCode }
        } else {
            Write-Host "⚠️  $Name (Code: $($response.StatusCode))" -ForegroundColor Yellow
            return @{ Name = $Name; Status = "WARNING"; Code = $response.StatusCode }
        }
    }
    catch {
        if ($_.Exception.Response.StatusCode -eq 401 -and $ExpectAuth) {
            Write-Host "🔒 $Name (Authentication required - OK)" -ForegroundColor Cyan
            return @{ Name = $Name; Status = "AUTH_REQUIRED"; Code = 401 }
        }
        elseif ($_.Exception.Response.StatusCode -eq 404) {
            Write-Host "❌ $Name (404 - No found)" -ForegroundColor Red
            return @{ Name = $Name; Status = "NOT_FOUND"; Code = 404 }
        }
        elseif ($_.Exception.Response.StatusCode -eq 409) {
            Write-Host "⚠️  $Name (409 - Conflict probably already exists)" -ForegroundColor Yellow
            return @{ Name = $Name; Status = "CONFLICT"; Code = 409 }
        }
        else {
            Write-Host "❌ $Name (Error: $($_.Exception.Message))" -ForegroundColor Red
            return @{ Name = $Name; Status = "ERROR"; Code = 0; Error = $_.Exception.Message }
        }
    }
}

# ========================================
# 1.TEST PUBLIC ENDPOINTS
# ========================================
Write-Host ""
Write-Host "1️⃣  TESTING PUBLIC ENDPOINTS..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host ""

$results += Test-Endpoint -Name "Profile Public" -Method "GET" -Url "$baseUrl/api/profile/public"
$results += Test-Endpoint -Name "Products List" -Method "GET" -Url "$baseUrl/api/products?page=1&pageSize=10"
$results += Test-Endpoint -Name "Products Count" -Method "GET" -Url "$baseUrl/api/products/count"
$results += Test-Endpoint -Name "Customers List" -Method "GET" -Url "$baseUrl/api/customersapi"

# ========================================
# 2.REGISTER A TRIAL USER
# ========================================
Write-Host ""
Write-Host "2️⃣  REGISTERING A TEST USER..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host ""

$timestamp = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
$testEmail = "testuser$timestamp@example.com"

$registerData = @{
    username = "testuser$timestamp"
    email = $testEmail
    password = "Test123!@#"
    firstName = "Test"
    lastName = "User"
    document = "12345678"
    phone = "+1234567890"
    address = "123 Test Street"
} | ConvertTo-Json

Write-Host "📝 Test email: $testEmail" -ForegroundColor Gray

$token = $null
try {
    $registerResponse = Invoke-RestMethod `
        -Uri "$baseUrl/api/auth/register-client" `
        -Method POST `
        -ContentType "application/json" `
        -Body $registerData `
        -ErrorAction Stop
    
    $token = $registerResponse.token
    Write-Host "✅ User successfully registered" -ForegroundColor Green
    Write-Host "🎫 JWT token received: $($token.Substring(0, [Math]::Min(50, $token.Length)))..." -ForegroundColor Gray
    $results += @{ Name = "Register Client"; Status = "OK"; Code = 200 }
}
catch {
    Write-Host "❌ Error registering user: $($_.Exception.Message)" -ForegroundColor Red
    $results += @{ Name = "Register Client"; Status = "ERROR"; Code = 0 }
}

# ========================================
# 3.VERIFY THAT THE USER APPEARS IN CUSTOMERS
# ========================================
Write-Host ""
Write-Host "3️⃣  VERIFYING USER ON DASHBOARD..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host ""

try {
    $customersResponse = Invoke-RestMethod `
        -Uri "$baseUrl/api/customersapi" `
        -Method GET `
        -ErrorAction Stop
    
    $foundUser = $customersResponse.data | Where-Object { $_.email -eq $testEmail }
    
    if ($foundUser) {
        Write-Host "✅ User found in the customer list" -ForegroundColor Green
        Write-Host "   📧 Email: $($foundUser.email)" -ForegroundColor Gray
        Write-Host "   👤 Name: $($foundUser.fullName)" -ForegroundColor Gray
        Write-Host "   📱 Phone: $($foundUser.phone)" -ForegroundColor Gray
        Write-Host "   🆔 ID: $($foundUser.id)" -ForegroundColor Gray
        Write-Host ""
        Write-Host "🎉 SUCCESS! The registered user appears on the dashboard" -ForegroundColor Green
        $results += @{ Name = "User in Dashboard"; Status = "OK"; Code = 200 }
    }
    else {
        Write-Host "❌ User NOT found in the customer list" -ForegroundColor Red
        Write-Host "⚠️  Total customers in database: $($customersResponse.data.Count)" -ForegroundColor Yellow
        $results += @{ Name = "User in Dashboard"; Status = "NOT_FOUND"; Code = 404 }
    }
}
catch {
    Write-Host "❌ Error verifying customers: $($_.Exception.Message)" -ForegroundColor Red
    $results += @{ Name = "User in Dashboard"; Status = "ERROR"; Code = 0 }
}

# ========================================
# 4. TEST LOGIN
# ========================================
Write-Host ""
Write-Host "4️⃣  TESTING LOGIN..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host ""

$loginData = @{
    email = $testEmail
    password = "Test123!@#"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod `
        -Uri "$baseUrl/api/auth/login" `
        -Method POST `
        -ContentType "application/json" `
        -Body $loginData `
        -ErrorAction Stop
    
    $token = $loginResponse.token
    Write-Host "✅ Successful login" -ForegroundColor Green
    Write-Host "🎫 Token JWT: $($token.Substring(0, [Math]::Min(50, $token.Length)))..." -ForegroundColor Gray
    $results += @{ Name = "Login"; Status = "OK"; Code = 200 }
}
catch {
    Write-Host "❌ Login error: $($_.Exception.Message)" -ForegroundColor Red
    $results += @{ Name = "Login"; Status = "ERROR"; Code = 0 }
}

# ========================================
# 5.TEST ENDPOINTS WITH AUTHENTICATION
# ========================================
if ($token) {
    Write-Host ""
    Write-Host "5️⃣  TESTING AUTHENTICATED ENDPOINTS..." -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
    Write-Host ""
    
    $authHeaders = @{
        "Authorization" = "Bearer $token"
    }
    
    $results += Test-Endpoint -Name "My Profile" -Method "GET" -Url "$baseUrl/api/profile/me" -Headers $authHeaders
    $results += Test-Endpoint -Name "Authenticated Endpoint" -Method "GET" -Url "$baseUrl/api/profile/authenticated" -Headers $authHeaders
    $results += Test-Endpoint -Name "Client Only Endpoint" -Method "GET" -Url "$baseUrl/api/profile/client-only" -Headers $authHeaders
    $results += Test-Endpoint -Name "Vehicles List" -Method "GET" -Url "$baseUrl/api/vehicles" -Headers $authHeaders
    $results += Test-Endpoint -Name "Vehicles Available" -Method "GET" -Url "$baseUrl/api/vehicles/available" -Headers $authHeaders
}

# ========================================
# 6. TEST ENDPOINTS THAT REQUIRE ADMIN
# ========================================
Write-Host ""
Write-Host "6️⃣ TESTING ADMIN ENDPOINTS (Should require auth)..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host ""

$results += Test-Endpoint -Name "Admin Only Endpoint" -Method "GET" -Url "$baseUrl/api/profile/admin-only" -ExpectAuth $true
$results += Test-Endpoint -Name "Sales List (Admin)" -Method "GET" -Url "$baseUrl/api/sales" -ExpectAuth $true

# ========================================
# 7. RESUMEN FINAL
# ========================================
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  📊 VERIFICATION SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$okCount = ($results | Where-Object { $_.Status -eq "OK" }).Count
$authCount = ($results | Where-Object { $_.Status -eq "AUTH_REQUIRED" }).Count
$errorCount = ($results | Where-Object { $_.Status -eq "ERROR" }).Count
$notFoundCount = ($results | Where-Object { $_.Status -eq "NOT_FOUND" }).Count
$warningCount = ($results | Where-Object { $_.Status -eq "WARNING" }).Count
$conflictCount = ($results | Where-Object { $_.Status -eq "CONFLICT" }).Count

$total = $results.Count

Write-Host "Total de pruebas: $total" -ForegroundColor White
Write-Host ""
Write-Host "✅ Successful: $okCount" -ForegroundColor Green
Write-Host "🔒 Require Auth (OK): $authCount" -ForegroundColor Cyan
Write-Host "⚠️Warnings: $warningCount" -ForegroundColor Yellow
Write-Host "⚠️ Conflicts: $conflictCount" -ForegroundColor Yellow
Write-Host "❌ Errors: $errorCount" -ForegroundColor Red
Write-Host "❌ Not Found: $notFoundCount" -ForegroundColor Red
Write-Host ""

# VERIFICATION OF THE MAIN PROBLEM
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host ""
Write-Host "🎯 VERIFICATION OF THE MAIN PROBLEM:" -ForegroundColor Cyan
Write-Host ""

$registerOk = ($results | Where-Object { $_.Name -eq "Register Client" -and $_.Status -eq "OK" }).Count -gt 0
$dashboardOk = ($results | Where-Object { $_.Name -eq "User in Dashboard" -and $_.Status -eq "OK" }).Count -gt 0
$loginOk = ($results | Where-Object { $_.Name -eq "Login" -and $_.Status -eq "OK" }).Count -gt 0

if ($registerOk -and $dashboardOk -and $loginOk) {
    Write-Host "🎉 PROBLEM SOLVED!" -ForegroundColor Green
    Write-Host ""
    Write-Host "✅ Registration creates AuthUser + Customer" -ForegroundColor Green
    Write-Host "✅ User appears on the dashboard" -ForegroundColor Green
    Write-Host "✅ Login works correctly" -ForegroundColor Green
    Write-Host ""
    Write-Host " The /api/auth/register-client endpoint now works perfectly." -ForegroundColor Green
    Write-Host " Registered users automatically appear on the admin dashboard." -ForegroundColor Green
}
else {
    Write-Host "❌ PROBLEM DETECTED:" -ForegroundColor Red
    Write-Host ""
    if (-not $registerOk) {
        Write-Host "❌ Registration failed" -ForegroundColor Red
    }
    if (-not $dashboardOk) {
        Write-Host "❌ The user does NOT appear on the dashboard" -ForegroundColor Red
    }
    if (-not $loginOk) {
        Write-Host " ❌ Login failed" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host ""
Write-Host "📝 Details of all tests:" -ForegroundColor Yellow
Write-Host ""

$results | ForEach-Object {
    $icon = switch ($_.Status) {
        "OK" { "✅" }
        "AUTH_REQUIRED" { "🔒" }
        "WARNING" { "⚠️ " }
        "ERROR" { "❌" }
        "NOT_FOUND" { "❌" }
        "CONFLICT" { "⚠️ " }
        default { "❓" }
    }
    
    $statusText = $_.Status
    if ($_.Code) {
        $statusText += " ($($_.Code))"
    }
    
    Write-Host "  $icon $($_.Name.PadRight(30)) - $statusText" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "🌐 Swagger UI: http://localhost:5086/swagger/index.html" -ForegroundColor Yellow
Write-Host "📚 Documentación: docs/API_ENDPOINTS_STATUS.md" -ForegroundColor Yellow
Write-Host "🧪 Tests HTTP: docs/TEST_ENDPOINTS.http" -ForegroundColor Yellow
Write-Host ""


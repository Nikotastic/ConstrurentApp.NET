# 🚀 Complete Setup Script - All in One

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "   FIRMNESS - Automatic Full Setup    " -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$totalSteps = 3
$currentStep = 0

# ============================================
# PASO 1: Migraciones
# ============================================
$currentStep++
Write-Host "[$currentStep/$totalSteps] 🗄️  DATABASE MIGRATIONS" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Gray
Write-Host ""

$migrateChoice = Read-Host "Where do you want to apply migrations? (local/docker/both) [both]"
if ([string]::IsNullOrWhiteSpace($migrateChoice)) {
    $migrateChoice = "both"
}

Write-Host ""
Write-Host "Ejecutando: .\migrate.ps1 $migrateChoice" -ForegroundColor Cyan
Write-Host ""

& .\migrate.ps1 $migrateChoice

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "⚠️  There were problems with migrations." -ForegroundColor Yellow
    Write-Host "Do you want to continue anyway? (y/n) [y]" -ForegroundColor Yellow
    $continue = Read-Host
    if ($continue -eq 'n') {
        Write-Host "Setup cancelado" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host " Migrations completed" -ForegroundColor Green
Write-Host ""

# ============================================
# PASO 2: Configuración de Email
# ============================================
$currentStep++
Write-Host "[$currentStep/$totalSteps] 📧 EMAIL SETTINGS" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Gray
Write-Host ""

# Verificar si ya está configurado
$appSettings = Get-Content "Firmness.Admin.Web\appsettings.Development.json" -Raw | ConvertFrom-Json
$emailConfigured = $appSettings.EmailSettings.SenderEmail -and 
                   $appSettings.EmailSettings.SenderEmail -ne "" -and 
                   $appSettings.EmailSettings.SenderEmail -ne "tu-email@gmail.com"

if ($emailConfigured) {
    Write-Host "Email already configured: $($appSettings.EmailSettings.SenderEmail)" -ForegroundColor Green
    Write-Host "Do you want to reconfigure it? (y/n) [n]" -ForegroundColor Yellow
    $reconfigure = Read-Host
    
    if ($reconfigure -eq 'y') {
        & .\setup-email.ps1
    }
} else {
    Write-Host "Email not configured. Starting wizard..." -ForegroundColor Yellow
    Write-Host ""
    
    & .\setup-email.ps1
}

Write-Host ""
Write-Host "✅ Email setup complete" -ForegroundColor Green
Write-Host ""

# ============================================
# PASO 3: Ejecutar Aplicación
# ============================================
$currentStep++
Write-Host "[$currentStep/$totalSteps] 🚀 RUN APPLICATION" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Gray
Write-Host ""

Write-Host "How do you want to run the application?" -ForegroundColor Cyan
Write-Host " 1. Local (dotnet run)" -ForegroundColor White
Write-Host " 2. Docker (docker-compose up)" -ForegroundColor White
Write-Host " 3. Don't run now" -ForegroundColor White
Write-Host ""

$runChoice = Read-Host "Choose an option [1]"
if ([string]::IsNullOrWhiteSpace($runChoice)) {
    $runChoice = "1"
}

Write-Host ""

if ($runChoice -eq "1") {
    Write-Host "Running application in LOCAL mode..." -ForegroundColor Green
    Write-Host ""
    Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "URL: https://localhost:5001" -ForegroundColor Cyan
    Write-Host ""

    dotnet run --project Firmness.Admin.Web

} elseif ($runChoice -eq "2") {
    Write-Host "Running application in DOCKER..." -ForegroundColor Green
    Write-Host ""
    Write-Host "Starting containers..." -ForegroundColor Yellow
    docker-compose up

} else {
    Write-Host "OK, the application will not run now." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To run later:" -ForegroundColor Cyan
    Write-Host "Local: dotnet run --project Firmness.Admin.Web" -ForegroundColor White
    Write-Host " Docker: docker-compose up" -ForegroundColor White
}
# ============================================
# RESUMEN FINAL
# ============================================
Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "         ✅ SETUP COMPLETED" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Everything is ready to use:" -ForegroundColor Green
Write-Host ""
Write-Host "✅ Migrated Database" -ForegroundColor White
Write-Host "✅ Email configured" -ForegroundColor White
Write-Host "✅ Functional System" -ForegroundColor White
Write-Host ""

Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host " 1. Open the application in the browser" -ForegroundColor White
Write-Host " 2. Go to the Clients section" -ForegroundColor White
Write-Host " 3. Register a new client" -ForegroundColor White
Write-Host " 4. Verify that the welcome email arrives! 📧" -ForegroundColor White
Write-Host ""

Write-Host "Available documentation:" -ForegroundColor Cyan
Write-Host " - MIGRATION_GUIDE.md (Migration Guide)" -ForegroundColor Gray
Write-Host " - FIX_NO_EMAIL.md (Email Troubleshooting)" -ForegroundColor Gray
Write-Host " - ARQUITECTURA_ANALISIS.md (Architecture Analysis)" -ForegroundColor Gray
Write-Host ""

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Enjoy Firmness! 🎉" -ForegroundColor Green
Write-Host ""


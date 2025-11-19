# Script to Configure Gmail Email
# This script guides you through configuring your email step by step.

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "EMAIL SETTINGS - GMAIL" -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "This wizard will help you configure Gmail SMTP" -ForegroundColor Yellow
Write-Host ""

# Request email
Write-Host "Step 1: Enter your Gmail email" -ForegroundColor Cyan
Write-Host ""
$email = Read-Host "Gmail Email"

if (-not $email -or -not $email.Contains("@")) {
    Write-Host "✗ Invalid Email" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Step 2: Generate your Application Password" -ForegroundColor Cyan
Write-Host ""
Write-Host "IMPORTANT: DO NOT use your regular Gmail password" -ForegroundColor Red
Write-Host ""
Write-Host "Follow these steps:" -ForegroundColor Yellow
Write-Host "1. Open: https://myaccount.google.com/security" -ForegroundColor White
Write-Host "2. Find 'Two-step verification' and turn it on" -ForegroundColor White
Write-Host "3. Then find 'Application passwords'" -ForegroundColor White
Write-Host "4. Select 'Mail' and 'Other' (type: Firmness)" -ForegroundColor White
Write-Host "5. Copy the 16-character password it gives you" -ForegroundColor White
Write-Host ""
Write-Host "Press Enter when you have copied the password..." -ForegroundColor Yellow
Read-Host

Write-Host ""
$password = Read-Host "Paste the application password (16 characters)" -AsSecureString
$passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
)

if (-not $passwordPlain -or $passwordPlain.Length -lt 10) {
    Write-Host "✗ CPassword too short or invalid -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Paso 3: Updating settings...." -ForegroundColor Cyan
Write-Host ""

# Read the configuration file
$configPath = "Firmness.Admin.Web\appsettings.Development.json"
$config = Get-Content $configPath -Raw | ConvertFrom-Json

# Update settings
$config.EmailSettings.SenderEmail = $email
$config.EmailSettings.Username = $email
$config.EmailSettings.Password = $passwordPlain

# Keep
$config | ConvertTo-Json -Depth 10 | Set-Content $configPath
Write-Host "✓ Updated configuration in $configPath" -ForegroundColor Green
Write-Host ""

# User Secrets Option (More Secure)
Write-Host "Step 4: Do you want to use User Secrets? (More Secure)" -ForegroundColor Cyan
Write-Host "This will securely store the password outside of code" -ForegroundColor Gray
Write-Host ""
$useSecrets = Read-Host "Use User Secrets? (y/n)"

if ($useSecrets -eq 'y') {
    Write-Host ""
    Write-Host "Setting User Secrets..." -ForegroundColor Yellow

    Push-Location Firmness.Admin.Web

    # Initialize secrets if they don't exist
    dotnet user-secrets init 2>$null

    # Set secrets
    dotnet user-secrets set "EmailSettings:SenderEmail" $email
    dotnet user-secrets set "EmailSettings:Username" $email
    dotnet user-secrets set "EmailSettings:Password" $passwordPlain

    Pop-Location

    Write-Host "✓ User Secrets configured" -ForegroundColor Green
    Write-Host ""
    Write-Host "NOTE: The password in appsettings.Development.json" -ForegroundColor Yellow
    Write-Host " will be overwritten by User Secrets" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "✓ FULL SETUP" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Email configured: $email" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host " 1. Run the application:" -ForegroundColor White
Write-Host "dotnet run --project Firmness.Admin.Web" -ForegroundColor Cyan
Write-Host ""
Write-Host " 2. Register a new client" -ForegroundColor White
Write-Host ""
Write-Host " 3. Check the welcome email in your inbox!" -ForegroundColor White
Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan

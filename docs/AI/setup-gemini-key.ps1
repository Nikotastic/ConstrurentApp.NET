# Script to configure Gemini API Key

param(
    [Parameter(Mandatory=$false)]
    [string]$ApiKey
)

Write-Host "=== Gemini API Key Configuration ===" -ForegroundColor Cyan
Write-Host ""

if (-not $ApiKey) {
    Write-Host "INSTRUCTIONS:" -ForegroundColor Yellow
    Write-Host "1. Get your API Key from: https://aistudio.google.com/apikey" -ForegroundColor White
    Write-Host "2. Run this script with your API Key:" -ForegroundColor White
    Write-Host '   .\setup-gemini-key.ps1 -ApiKey "YOUR_API_KEY_HERE"' -ForegroundColor Green
    Write-Host ""
    Write-Host "Or you can configure it manually:" -ForegroundColor Yellow
    Write-Host '   $env:GEMINI_API_KEY = "YOUR_API_KEY_HERE"' -ForegroundColor Green
    Write-Host ""
    exit
}

# Validate that the API Key has a reasonable format
if ($ApiKey.Length -lt 20) {
    Write-Host "✗ The API Key seems too short. Please verify it is correct." -ForegroundColor Red
    exit 1
}

Write-Host "Configuring API Key..." -ForegroundColor Yellow
Write-Host ""

# OPTION 1: Configure as user environment variable (persistent)
try {
    [System.Environment]::SetEnvironmentVariable("GEMINI_API_KEY", $ApiKey, [System.EnvironmentVariableTarget]::User)
    Write-Host "✓ API Key configured as user environment variable (persistent)" -ForegroundColor Green
    Write-Host "  This configuration will persist after restart." -ForegroundColor Gray
} catch {
    Write-Host "✗ Error configuring user environment variable: $($_.Exception.Message)" -ForegroundColor Red
}

# Configure for current session as well
$env:GEMINI_API_KEY = $ApiKey
Write-Host "✓ API Key configured for current session" -ForegroundColor Green
Write-Host ""

# OPTION 2: Update appsettings.Development.json
Write-Host "Do you also want to update appsettings.Development.json? (y/n): " -NoNewline -ForegroundColor Yellow
$updateFile = Read-Host

if ($updateFile -eq 'y' -or $updateFile -eq 'Y') {
    $appsettingsPath = ".\src\Firmness.Api\appsettings.Development.json"
    
    if (Test-Path $appsettingsPath) {
        try {
            $content = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
            $content.Gemini.ApiKey = $ApiKey
            $content | ConvertTo-Json -Depth 10 | Set-Content $appsettingsPath
            Write-Host "✓ appsettings.Development.json updated" -ForegroundColor Green
        } catch {
            Write-Host "✗ Error updating appsettings: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "=== Connection Test ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Testing connection with Gemini API..." -ForegroundColor Yellow

$testBody = @{
    contents = @(
        @{
            parts = @(
                @{
                    text = "Say 'Hello' in English"
                }
            )
        }
    )
} | ConvertTo-Json -Depth 10

try {
    $response = Invoke-RestMethod -Uri "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key=$ApiKey" `
        -Method POST `
        -ContentType "application/json" `
        -Body $testBody `
        -TimeoutSec 10
    
    Write-Host "✓ Connection successful!" -ForegroundColor Green
    Write-Host "  Test response: $($response.candidates[0].content.parts[0].text)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "✓ Your API Key is working correctly" -ForegroundColor Green
    Write-Host ""
    Write-Host "Now restart the Firmness API to apply the new configuration." -ForegroundColor Yellow
} catch {
    Write-Host "✗ Error connecting with Gemini API" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "  Details: $responseBody" -ForegroundColor Red
        } catch {}
    }
    Write-Host ""
    Write-Host "Please verify that your API Key is correct." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Configuration Finished ===" -ForegroundColor Cyan


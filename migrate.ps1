# Script to run Entity Framework migrations
# Uso: .\migrate.ps1 [local|docker]

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('local', 'docker')]
    [string]$Env = 'local'
)

Write-Host "=== Firmness Migration Script ===" -ForegroundColor Cyan
Write-Host "Entorno: $Env" -ForegroundColor Yellow
Write-Host ""

if ($Env -eq 'local') {
    # Migración LOCAL - usa localhost
    Write-Host "Running migration on LOCAL (localhost)..." -ForegroundColor Green
    
    $localConnStr = "Host=localhost;Database=FirmnessDB;Username=postgres;Password=niko;Port=5432"
    
    # Set environment variable for this session
    $env:CONN_STR = $localConnStr
    
    Write-Host "Connection String: $localConnStr" -ForegroundColor Gray
    Write-Host ""
    
    # Run migration
    dotnet ef database update --project Firmness.Infrastructure --startup-project Firmness.Admin.Web
    
} elseif ($Env -eq 'docker') {
    # Docker migration - runs inside the container
    Write-Host "Running migration in Docker..." -ForegroundColor Green
    
    # Check if the container is running
    $webContainer = docker ps --filter "name=construrentappnet-web-1" --format "{{.Names}}"
    
    if (-not $webContainer) {
        Write-Host "ERROR: The container 'construrentappnet-web-1' is not running." -ForegroundColor Red
        Write-Host "Run first: docker-compose up -d" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "Running migration inside the container..." -ForegroundColor Gray
    Write-Host ""
    
    # Run migration inside the container
    docker exec construrentappnet-web-1 dotnet ef database update --project /app/Firmness.Infrastructure --startup-project /app/Firmness.Admin.Web
}

Write-Host ""
Write-Host "=== Migration completed ===" -ForegroundColor Cyan

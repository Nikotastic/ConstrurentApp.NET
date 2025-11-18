# Script to run Entity Framework migrations
# Uso: .\migrate.ps1 [local|docker|both]

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('local', 'docker', 'both')]
    [string]$Env = 'local'
)

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "   FIRMNESS - Migration Script              " -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Entorno: $Env" -ForegroundColor Yellow
Write-Host ""

function Run-LocalMigration {
    Write-Host "📍 LOCAL MIGRATION (localhost)" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Gray
    Write-Host ""
    
    # Verificar PostgreSQL local
    Write-Host "Verifying local PostgreSQL..." -ForegroundColor Yellow
    try {
        $pgTest = Test-NetConnection -ComputerName localhost -Port 5432 -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
        if (-not $pgTest.TcpTestSucceeded) {
            Write-Host "❌ PostgreSQL is NOT running on localhost:5432" -ForegroundColor Red
            Write-Host ""
            Write-Host "Options:" -ForegroundColor Yellow
            Write-Host " 1. Start local PostgreSQL" -ForegroundColor White
            Write-Host " 2. Or run: docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres" -ForegroundColor White
            Write-Host ""
            return $false
        }
        Write-Host "✅ PostgreSQL is running" -ForegroundColor Green
    } catch {
        Write-Host "⚠️ Could not verify PostgreSQL" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "Applying migrations..." -ForegroundColor Yellow
    Write-Host ""

    # Navigate to the Infrastructure directory 
    Push-Location Firmness.Infrastructure

    try {
        # Run migration 
        dotnet ef database update --startup-project ..\Firmness.Admin.Web --verbose

        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "✅ Migrations successfully applied on LOCAL" -ForegroundColor Green
            Pop-Location
            return $true
        } else {
            Write-Host ""
            Write-Host "❌ Error applying migrations on LOCAL" -ForegroundColor Red
            Pop-Location
            return $false
        }
    } catch {
        Write-Host ""
        Write-Host "❌ Error: $_" -ForegroundColor Red
        Pop-Location
        return $false
    }
}
function Run-DockerMigration {
    Write-Host ""
    Write-Host "🐳 DOCKER MIGRATION" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Gray
    Write-Host ""

    # Verify that Docker is running 
    Write-Host "Verifying Docker..." -ForegroundColor Yellow
    try {
        docker ps | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Docker is not running" -ForegroundColor Red
            return $false
        }
        Write-Host "✅ Docker is running" -ForegroundColor Green
    } catch {
        Write-Host "❌ Docker is not installed or responding" -ForegroundColor Red
        return $false
    }

    Write-Host ""
    Write-Host "Verifying containers..." -ForegroundColor Yellow

    # Check if containers are running 
    $webContainer = docker ps --filter "name=construrentappnet-web-1" --format "{{.Names}}" 2>$null
    $pgContainer = docker ps --filter "name=construrentappnet-postgres-1" --format "{{.Names}}" 2>$null

    if (-not $webContainer -or -not $pgContainer) {
        Write-Host "⚠️ Containers are not running" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Starting containers with docker-compose..." -ForegroundColor Yellow
        docker-compose up -d

        Write-Host "Waiting for PostgreSQL to be ready..." -ForegroundColor Yellow
        Start-Sleep-Seconds 10

        # Check again 
        $webContainer = docker ps --filter "name=construrentappnet-web-1" --format "{{.Names}}" 2>$null
        $pgContainer = docker ps --filter "name=construrentappnet-postgres-1" --format "{{.Names}}" 2>$null

        if (-not $webContainer -or -not $pgContainer) {
            Write-Host "❌ Failed to start containers" -ForegroundColor Red
            return $false
        }
    }

    Write-Host "✅ Containers running:" -ForegroundColor Green
    Write-Host " - $webContainer" -ForegroundColor Gray
    Write-Host " - $pgContainer" -ForegroundColor Gray
    Write-Host ""

    # Verify/install dotnet-ef in the container 
    Write-Host "Checking dotnet-ef in container..." -ForegroundColor Yellow
    $efCheck = docker exec construrentappnet-web-1 dotnet ef --version 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Installing dotnet-ef in the container..." -ForegroundColor Yellow
        docker exec construrentappnet-web-1 dotnet tool install --global dotnet-ef

        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Error installing dotnet-ef" -ForegroundColor Red
            return $false
        }
        Write-Host "✅ dotnet-ef installed" -ForegroundColor Green
    } else {
        Write-Host "✅ dotnet-ef is already installed" -ForegroundColor Green
    }

    Write-Host ""
    Write-Host "Applying migrations in Docker..." -ForegroundColor Yellow
    Write-Host ""

    # Run migration inside the container 
    $migrationCommand = "export PATH=`$PATH:/root/.dotnet/tools && cd /app/Firmness.Infrastructure && dotnet ef database update --startup-project /app/Firmness.Admin.Web --verbose"

    docker exec buildrentappnet-web-1 bash -c $migrationCommand

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Migrations successfully applied in DOCKER" -ForegroundColor Green
        return $true
    } else {
        Write-Host ""
        Write-Host "❌ Error applying migrations in DOCKER" -ForegroundColor Red
        return $false
    }
}
# ============================================
# EJECUCIÓN PRINCIPAL
# ============================================

$localSuccess = $false
$dockerSuccess = $false

if ($Env -eq 'local') {
    $localSuccess = Run-LocalMigration
    
} elseif ($Env -eq 'docker') {
    $dockerSuccess = Run-DockerMigration
    
} elseif ($Env -eq 'both') {
    Write-Host "Running migrations in BOTH environments..." -ForegroundColor Cyan
    Write-Host ""
    
    $localSuccess = Run-LocalMigration
    $dockerSuccess = Run-DockerMigration
}

# ============================================
# RESUMEN FINAL
# ============================================

Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "              RESUMEN                        " -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

if ($Env -eq 'local') {
    if ($localSuccess) {
        Write-Host "✅ Migración LOCAL: EXITOSA" -ForegroundColor Green
    } else {
        Write-Host "❌ Migración LOCAL: FALLÓ" -ForegroundColor Red
    }
} elseif ($Env -eq 'docker') {
    if ($dockerSuccess) {
        Write-Host "✅ Migración DOCKER: EXITOSA" -ForegroundColor Green
    } else {
        Write-Host "❌ Migración DOCKER: FALLÓ" -ForegroundColor Red
    }
} elseif ($Env -eq 'both') {
    if ($localSuccess) {
        Write-Host "✅ Migración LOCAL: EXITOSA" -ForegroundColor Green
    } else {
        Write-Host "❌ Migración LOCAL: FALLÓ" -ForegroundColor Red
    }
    
    if ($dockerSuccess) {
        Write-Host "✅ Migración DOCKER: EXITOSA" -ForegroundColor Green
    } else {
        Write-Host "❌ Migración DOCKER: FALLÓ" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

# Exit code
if (($Env -eq 'local' -and $localSuccess) -or 
    ($Env -eq 'docker' -and $dockerSuccess) -or 
    ($Env -eq 'both' -and $localSuccess -and $dockerSuccess)) {
    exit 0
} else {
    exit 1
}


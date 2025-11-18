# 🚀 QUICK GUIDE - Local and Docker Migrations

## 📋 Available Options

The `migrate.ps1` script now supports 3 options:

```powershell
# 1. Local migration only
.\migrate.ps1 local

# 2. Docker migration only
.\migrate.ps1 docker

# 3. Both migrations (local and Docker)
.\migrate.ps1 both
```

---
## 🏠 LOCAL MIGRATION

### Requirements:
- ✅ PostgreSQL running on `localhost:5432`
- ✅ Username: `postgres`
- ✅ Password configured in `appsettings.Development.json`
### Ejecutar:
```powershell
.\migrate.ps1 local
```

### If PostgreSQL is not running:

**Opción 1: Docker rápido**
```powershell
docker run -d --name postgres-local -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres:15
```
**Option 2: Local Service**
- Start PostgreSQL from Windows Services

---

## 🐳 DOCKER MIGRATION

### Requirements:
- ✅ Docker Desktop running
- ✅ Configured `.env` file
### Ejecutar:
```powershell
.\migrate.ps1 docker
```

### The script automatically:
1. Verifies that Docker is running
2. Starts containers if they are not running (`docker-compose up -d`)
3. Installs `dotnet-ef` in the container if necessary
4. Applies migrations

---

## 🔄 BOTH MIGRATIONS

To apply migrations to LOCAL and DOCKER simultaneously:

```powershell
.\migrate.ps1 both
```

This is useful when:
- You develop locally but also use Docker
- You want to ensure both environments are synchronized

---

## 📝 Verify .env File

Make sure you have a `.env` file in the project root:

```env
# PostgreSQL
POSTGRES_DB=firmness_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=tu-password-segura

# Web
WEB_PORT=5000
ASPNETCORE_ENVIRONMENT=Development

# PgAdmin
PGADMIN_EMAIL=admin@firmness.com
PGADMIN_PASSWORD=admin123
```

---

## ✅ Verify Applied Migrations

### Local:
```powershell
cd Firmness.Infrastructure
dotnet ef migrations list --startup-project ..\Firmness.Admin.Web
```

### Docker:
```powershell
docker exec construrentappnet-web-1 bash -c "cd /app/Firmness.Infrastructure && dotnet ef migrations list --startup-project /app/Firmness.Admin.Web"
```

The applied migrations will have an asterisk (*) next to them.

---

## 🐛 Troubleshooting

### Error: "PostgreSQL is not running"
```powershell
# Check port
Test-NetConnection -ComputerName localhost -Port 5432

# If no response, start PostgreSQL
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres:15
```

### Error: "Docker is not running"
1. Open Docker Desktop
2. Wait for it to fully start
3. Re-run the script

### Error: "Cannot start containers"
```powershell
# View logs
docker-compose logs

# Restart containers
docker-compose down
docker-compose up -d
```

### Error: "dotnet-ef could not be installed"
```powershell
# Enter the container manually
`docker exec -it construrentappnet-web-1 bash`

# Install dotnet-ef
`dotnet tool install --global dotnet-ef`

# Add to PATH
`export PATH=$PATH:/root/.dotnet/tools`

# Run migration
`cd /app/Firmness.Infrastructure`
`dotnet ef database update --startup-project /app/Firmness.Admin.Web`
` ...

---

## 📊 Typical Development Flow

### Scenario 1: Local Development
```powershell
# 1. Start PostgreSQL locally
`docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres:15

# 2. Apply migrations
`.\migrate.ps1 local

# 3. Run application
`dotnet run --project Firmness.Admin.Web
```

### Scenario 2: Development with Docker
```powershell
# 1. Start everything with Docker
`docker-compose up -d

# 2. Apply migrations
`.\migrate.ps1 docker

# 3. View logs
`docker-compose logs -f web
```

### Scenario 3: Both environments
```powershell
# 1. Apply migrations to both
`.\migrate.ps1 both

# 2. Use the one that you prefer
# Local: dotnet run --project Firmness.Admin.Web
# Docker: docker-compose up
```
---

## 🎯After Migration

Once the migrations are applied, configure the email:

```powershell
.\setup-email.ps1
```

And then run the application:

```powershell
# Local
dotnet run --project Firmness.Admin.Web

# Docker
docker-compose up
```

---

## 📞 Useful Commands

```powershell
# View container status
`docker-compose ps

# View real-time logs
`docker-compose logs -f`

# Stop everything
`docker-compose down`

# Stop and delete volumes (CAUTION: deletes data)
`docker-compose down -v`

# Rebuild containers
`docker-compose up -d --build`

# Access the web container
`docker exec -it construrentappnet-web-1 bash`

# Access PostgreSQL
`docker exec -it construrentappnet-postgres-1`
` ...
---

## ✨ Summary of Main Commands

```powershell
# MIGRATIONS
.\migrate.setup-complete # Local, docker, both

# CONFIGURATION
.\setup-email.ps1 # Configure Gmail

# RUN
dotnet run --project Firmness.Admin.Web # Local
docker-compose up # Docker
```

---

**Ready to migrate!** 🚀

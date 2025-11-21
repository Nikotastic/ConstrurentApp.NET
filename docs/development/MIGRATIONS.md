# Database Migrations

## 📋 Description

The project uses **Entity Framework Core Migrations** to manage the PostgreSQL database schema.

---

## 🚀 Basic Commands

### Create new migration

```bash
  cd src/Firmness.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Firmness.Api
```

### Apply migrations

```bash
    dotnet ef database update --startup-project ../Firmness.Api
```

### View applied migrations

```bash
  dotnet ef migrations list --startup-project ../Firmness.Api
```

### Revert last migration

```bash
  dotnet ef migrations remove --startup-project ../Firmness.Api
```

### Generate SQL script

```bash
  dotnet ef migrations script --startup-project ../Firmness.Api --output migration.sql
```

---

## 🗄️ Database Context

**DbContext:** `ApplicationDbContext` (located in `Firmness.Infrastructure`)

**Migrations location:** `src/Firmness.Infrastructure/Migrations/`

**Provider:** Npgsql (PostgreSQL)

---

## 📦 Main Entities

| Entity | Table | Description |
|---------|-------|-------------|
| `Customer` | `Customer` | System clients |
| `Product` | `Product` | Products/construction supplies |
| `Category` | `Category` | Product categories |
| `Sale` | `Sale` | Completed sales |
| `SaleItem` | `SaleItem` | Items of each sale |
| `Vehicle` | `Vehicle` | Industrial vehicles |
| `VehicleRental` | `VehicleRental` | Vehicle rentals |
| `ApplicationUser` | `AspNetUsers` | Identity users |

---

## 🔧 Configuration

The DbContext is configured in `Program.cs`:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        b => b.MigrationsAssembly("Firmness.Infrastructure")
    )
);
```

---

## 🐳 Migrations with Docker

Migrations are applied **automatically** when starting the application:

```csharp
// In Program.cs
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // ← Applies pending migrations
}
```

### Manually apply migrations in Docker

If you need to manually apply migrations in a container:

```bash
  # Start services
docker-compose up -d

# Run migration in container
docker-compose exec web dotnet ef database update \
  --project /app/src/Firmness.Infrastructure \
  --startup-project /app/src/Firmness.Api
```

---

## 📝 Typical Workflow

### 1. Local Development

```bash
  # 1. Modify entities in Firmness.Domain
# Example: add Phone property to Customer

# 2. Create migration
cd src/Firmness.Infrastructure
dotnet ef migrations add AddPhoneToCustomer --startup-project ../Firmness.Api

# 3. Review generated files in Migrations/

# 4. Apply migration
dotnet ef database update --startup-project ../Firmness.Api

# 5. Verify it was applied
dotnet ef migrations list --startup-project ../Firmness.Api
```

### 2. With Docker

```bash
  # 1. Create migration locally (same as above)
cd src/Firmness.Infrastructure
dotnet ef migrations add AddPhoneToCustomer --startup-project ../Firmness.Api

# 2. Commit the migration to Git

# 3. Rebuild container
docker-compose down
docker-compose up -d --build

# Migration is applied automatically on startup
```

---

## 📋 Best Practices

### ✅ DO

1. **Descriptive names:**
   ```bash
   dotnet ef migrations add AddPhoneToCustomer          # ✅ Good
   dotnet ef migrations add UpdateCustomer              # ❌ Too generic
   ```

2. **Review generated code** before applying

3. **In production, use SQL scripts:**
   ```bash
   dotnet ef migrations script --output migration.sql
   # Review and apply manually
   ```

4. **Commit migrations** to Git

5. **Test migration in development** before production

### ❌ DON'T

- ❌ Don't modify already applied migrations
- ❌ Don't make manual changes to the DB (use migrations)
- ❌ Don't delete migration history
- ❌ Don't use auto-migrate in production (use SQL scripts)

---

## 🌍 Environments

### Development (Local)

```bash
# Connection string in .env
POSTGRES_HOST=localhost
POSTGRES_DB=FirmnessDB
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password
```

Migrations are applied with `dotnet ef database update`.

### Development (Docker)

Connection string points to `postgres` container:

```
Host=postgres;Port=5432;Database=FirmnessDB;Username=postgres;Password=...
```

Migrations are applied automatically on startup.

### Production

1. **Generate SQL script:**
   ```bash
   dotnet ef migrations script --startup-project ../Firmness.Api --output migration.sql
   ```

2. **Review the script** manually

3. **Apply in production:**
   ```bash
   psql -h production-host -U postgres -d FirmnessDB -f migration.sql
   ```

---

## ⚠️ Troubleshooting

### Error: "No DbContext was found"

**Solution:** Specify the startup project:
```bash
dotnet ef migrations add Name --startup-project ../Firmness.Api
```

### Error: "Cannot connect to database"

**Common causes:**
- PostgreSQL is not running
- Incorrect credentials in `.env`
- Port 5432 is busy

**Solution:** Verify PostgreSQL is running:
```bash
# Windows
docker ps | grep postgres

# Start PostgreSQL if not running
docker-compose up -d postgres
```

### Error: "The migration has already been applied"

**Solution:** The migration already exists. List applied migrations:
```bash
  dotnet ef migrations list --startup-project ../Firmness.Api
```

### Error: "Pending model changes"

**Solution:** There are model changes without migration:
```bash
  dotnet ef migrations add PendingChanges --startup-project ../Firmness.Api
```

---

## 🔄 Reverting Migrations

### Revert to specific migration

```bash
  # View list of migrations
dotnet ef migrations list --startup-project ../Firmness.Api

# Revert to previous migration
dotnet ef database update MigrationName --startup-project ../Firmness.Api
```

### Remove last migration (if not applied yet)

```bash
  dotnet ef migrations remove --startup-project ../Firmness.Api
```

### Revert all migrations

```bash
  dotnet ef database update 0 --startup-project ../Firmness.Api
```

⚠️ **Warning:** This deletes all data.

---

## 📚 References

- [EF Core Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Npgsql EF Core Provider](https://www.npgsql.org/efcore/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)


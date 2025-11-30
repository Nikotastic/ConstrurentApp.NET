# Environment Variables Configuration

> [⬅️ Back to Main README](../../README.md) | [📚 Documentation Hub](../README.md)

## 📋 Description

This project uses environment variables to configure database credentials, JWT, and other sensitive services, following security best practices (12-Factor App).

## 🚀 Quick Setup

### 1. Copy example file

```bash
  cp .env.example .env
```

### 2. Configure credentials

Edit the `.env` file with your values:

```env
# PostgreSQL
POSTGRES_HOST=localhost
POSTGRES_DB=FirmnessDB
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password_here

# JWT (optional - uses secure defaults)
JWT__Key=your_jwt_key_at_least_32_characters
```

### 3. Verify

The application will automatically load the `.env` file on startup.

## 📝 Available Variables

### Database (Required)

| Variable            | Description     | Default Value | Required |
| ------------------- | --------------- | ------------- | -------- |
| `POSTGRES_HOST`     | PostgreSQL host | `localhost`   | No       |
| `POSTGRES_DB`       | Database name   | `FirmnessDB`  | No       |
| `POSTGRES_USER`     | PostgreSQL user | `postgres`    | No       |
| `POSTGRES_PASSWORD` | Password        | -             | **YES**  |
| `PG_PORT`           | Port            | `5432`        | No       |

### JWT (Optional)

| Variable        | Description                                      |
| --------------- | ------------------------------------------------ |
| `JWT__Key`      | Secret key to sign JWT tokens (minimum 32 chars) |
| `JWT__Issuer`   | Token issuer                                     |
| `JWT__Audience` | Token audience                                   |

### Email (Use User Secrets)

To configure email, use `dotnet user-secrets` instead of `.env`:

```bash
cd src/Firmness.Api
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SenderEmail" "your@email.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
```

## 🔒 Security

- ✅ `.env` is in `.gitignore` - **NOT uploaded to repository**
- ✅ Use `.env.local` for local configuration (also ignored)
- ✅ In production, use system environment variables, not `.env` files

## 🐳 Docker

The project includes `compose.yaml` which automatically reads from `.env`:

```bash
  docker-compose up
```

## 🔍 Troubleshooting

### Error: "POSTGRES_PASSWORD not found"

**Solution:** Add `POSTGRES_PASSWORD` to the `.env` file

### Error: "Connection failed"

**Solution:** Verify that PostgreSQL is running and credentials are correct

## 📚 References

- [DotNetEnv](https://github.com/tonerdo/dotnet-env) - Library used
- [12-Factor App Config](https://12factor.net/config)

---

<div align="center">
  <a href="../../README.md">⬅️ Back to Main README</a> | 
  <a href="../README.md">📚 Documentation Hub</a> | 
  <a href="EMAIL_CONFIGURATION.md">📧 Email Configuration</a>
</div>

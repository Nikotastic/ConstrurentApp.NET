# ⚙️ Setup & Installation Guide

This guide covers the complete process to set up the Firmness system locally using Docker.

## ✅ Prerequisites

- **Docker Desktop**: Ensure it's installed and running.
  - _Windows_: Use WSL2 backend (recommended).
  - _Linux/Mac_: Standard installation.
- **Git**: To clone the repository.
- **.NET 8 SDK** (Optional, only if running without Docker).

---

## 🚀 Step-by-Step Installation

### 1. Clone the Repository

```bash
git clone https://github.com/your-repo/firmness.git
cd firmness
```

### 2. Environment Configuration

The system relies on environment variables. Create a `.env` file in the root directory.

**Example `.env`:**

```env
# Database
POSTGRES_DB=FirmnessDB
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password
PG_PORT=5432

# API
API_PORT=5000
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=FirmnessDB;Username=postgres;Password=your_secure_password

# JWT Authentication
JWT__Key=YourSuperSecretKeyMustBeLongEnough123!
JWT__Issuer=FirmnessApi
JWT__Audience=FirmnessClient

# Admin Panel
ADMIN_PORT=5001

# Client
CLIENT_PORT=80

# PgAdmin (Optional)
PGADMIN_EMAIL=admin@firmness.local
PGADMIN_PASSWORD=admin123
PGADMIN_PORT=8080
```

### 3. Run with Docker Compose

We use a **Safety-First Deployment** pipeline. This single command handles everything:

```bash
docker compose up --build
```

**The Pipeline Process:**

1.  🐳 **Build**: Docker images for API, Admin, Client, and Tests are built from source.
2.  🗄️ **Database**: PostgreSQL container starts and initializes.
3.  🧪 **Tests**: The `firmness-tests` container runs 90+ automated tests.
    - ✅ **Success**: If tests pass (Exit Code 0), the API, Admin, and Client containers start.
    - ❌ **Failure**: If tests fail, the deployment aborts to prevent running broken code.

### 4. Verify Deployment

Wait about 1-2 minutes for the database to initialize and migrations to apply.

Check running containers:

```bash
docker ps
```

You should see:

- `firmness-client`
- `firmness-api`
- `firmness-admin`
- `firmness-db`

---

## 🌐 Access Points

| Service         | URL                                                            | Default Credentials                    |
| --------------- | -------------------------------------------------------------- | -------------------------------------- |
| **Client App**  | [http://localhost:80](http://localhost:80)                     | `client@firmness.local` / `Client123!` |
| **Admin Panel** | [http://localhost:5001](http://localhost:5001)                 | `admin@firmness.local` / `Admin123!`   |
| **API Swagger** | [http://localhost:5000/swagger](http://localhost:5000/swagger) | -                                      |
| **PgAdmin**     | [http://localhost:8080](http://localhost:8080)                 | `admin@firmness.local` / `admin123`    |

---

## 🔄 Common Commands

**Stop all services:**

```bash
docker compose down
```

**View logs (e.g., API):**

```bash
docker logs -f firmness-api
```

**Run tests manually:**

```bash
docker compose run --rm tests
```

# Firmness 🏗️

**Comprehensive Management System for Construction Supply Sales and Industrial Vehicle Rental**

Firmness is a complete business solution that digitizes and optimizes construction company operations:

- 🏪 **Sales Management** - Construction materials and supplies
- 🚜 **Vehicle Rental** - Heavy machinery, cranes, forklifts
- 🤖 **AI Chatbot** - Intelligent assistant powered by Google Gemini
- 👥 **Customer Management** - User control and authentication
- 📊 **Admin Dashboard** - Complete web-based control panel
- 🔌 **REST API** - Modular API for system integration
- 🐳 **Docker Deployment** - Production-ready containerization

---

## 🚀 Quick Start

### Prerequisites

- [Docker Desktop](https://www.docker.com/) (recommended)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (for Angular client)

### Start with Docker (Recommended)

```powershell
# Windows
.\rebuild-docker.ps1

# Linux/macOS
chmod +x rebuild-docker.sh && ./rebuild-docker.sh
```

### Access the Application

| Service       | URL                            | Description       |
| ------------- | ------------------------------ | ----------------- |
| **API**       | https://localhost:7192         | RESTful API       |
| **Swagger**   | https://localhost:7192/swagger | API Documentation |
| **Dashboard** | http://localhost:5000          | Admin Panel       |
| **Client**    | http://localhost:4200          | Angular Frontend  |
| **PgAdmin**   | http://localhost:8080          | Database Admin    |

### Default Credentials

**Admin Dashboard:**

- Email: `admin@firmness.com`
- Password: `Admin123!`

**PgAdmin:**

- Email: `admin@firmness.com`
- Password: `admin123`

---

## 🏗️ Architecture

### Clean Architecture (4 Layers)

```
┌─────────────────────────────────────────────┐
│         Presentation Layer                  │
│  ┌──────────────┐  ┌──────────────┐        │
│  │ Firmness.Api │  │ Firmness.Web │        │
│  │  (REST API)  │  │  (MVC Admin) │        │
│  └──────────────┘  └──────────────┘        │
├─────────────────────────────────────────────┤
│        Application Layer                    │
│  - Services, DTOs, Interfaces               │
├─────────────────────────────────────────────┤
│        Domain Layer (Core)                  │
│  - Entities, Business Rules                 │
├─────────────────────────────────────────────┤
│        Infrastructure Layer                 │
│  - EF Core, PostgreSQL, Identity            │
└─────────────────────────────────────────────┘
```

### Project Structure

```
Firmness/
├── src/
│   ├── Firmness.Api/           # REST API
│   ├── Firmness.Web/           # Admin Dashboard
│   ├── Firmness.Application/   # Business Logic
│   ├── Firmness.Domain/        # Core Entities
│   └── Firmness.Infrastructure/# Data Access
├── client/                     # Angular Frontend
├── tests/                      # Automated Tests
└── docs/                       # Documentation
```

---

## 📚 Documentation

### Setup & Configuration

- **[Environment Variables](docs/setup/ENVIRONMENT.md)** - Configure `.env` and credentials
- **[Email Configuration](docs/setup/EMAIL_CONFIGURATION.md)** - Gmail SMTP setup
- **[AI Chatbot Setup](docs/AI/README.md)** - Configure Gemini-powered assistant

### Development

- **[Architecture Guide](docs/development/ARCHITECTURE.md)** - Hexagonal architecture details
- **[Database Migrations](docs/development/MIGRATIONS.md)** - EF Core migration guide

### API

- **[API Endpoints](docs/api/ENDPOINTS.md)** - Complete REST API reference
- **[Authentication](docs/api/AUTHENTICATION.md)** - JWT and authorization

### Testing

- **[Test Guide](tests/Firmness.Test/README_TESTS.md)** - Running automated tests (90 tests)

---

## 🛠️ Technology Stack

**Backend:** ASP.NET Core 8.0, Entity Framework Core, PostgreSQL  
**Frontend:** Angular 17, Angular Material  
**AI:** Google Gemini 2.0 Flash  
**DevOps:** Docker, Docker Compose  
**Testing:** xUnit, Moq, FluentAssertions

---

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

**Coverage:** 90 tests (60 services, 29 controllers, 1 integration)  
**Status:** ✅ 100% Passing

---

## 🔧 Useful Commands

### Docker

```bash
docker-compose up -d              # Start all services
docker-compose logs -f            # View logs
docker-compose restart api        # Restart API
docker-compose down               # Stop all services
```

### Development

```bash
dotnet build                      # Build solution
dotnet test                       # Run tests
dotnet ef migrations add Name     # Create migration
dotnet ef database update         # Apply migrations
```

---

## 🆘 Troubleshooting

### Common Issues

**CORS Errors (Linux):** See [docs/CORS_FIX_LINUX.md](docs/CORS_FIX_LINUX.md)  
**Database Issues:** `docker-compose restart db`  
**API Not Responding:** `docker logs firmness_api -f`

### More Help

- [API Documentation](docs/api/ENDPOINTS.md)
- [Architecture Guide](docs/development/ARCHITECTURE.md)
- [Test Guide](tests/Firmness.Test/README_TESTS.md)

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/NewFeature`
3. Commit changes: `git commit -m "feat: add new feature"`
4. Push to branch: `git push origin feature/NewFeature`
5. Open a Pull Request

### Coding Standards

- Follow **Clean Architecture** principles
- Use **PascalCase** for C# classes/methods
- Write **tests** for new features
- Keep controllers **lean** (logic in services)

---

## 📄 License

This project is licensed under the **MIT** license. See [LICENSE](LICENSE) for details.

---

<div align="center">

**Firmness** - Digitalize your construction business 🏗️

[Documentation](docs/) • [API](https://localhost:7192/swagger) • [Dashboard](http://localhost:5000)

**Made with ❤️ using .NET 8, Angular, and Docker**

</div>

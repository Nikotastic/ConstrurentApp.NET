# Firmness 🏗️

**Comprehensive Management System for the Sale of Construction Supplies and Rental of Industrial Vehicles**

Firmness is a complete business solution that digitizes and optimizes the operations of construction companies, combining:
- 🏪 **Sales Management** of construction materials and supplies
- 🚜 **Rental of Industrial Vehicles** (heavy machinery, cranes, forklifts, etc.)
- 👥 **Customer Management** and User Control
- 📊 **Complete Web-Based Administrative Dashboard**
- 🔌 **Modular REST API** for integration with other systems
- 🐳 **Orchestrated Deployment** with Docker for production
- ✅ **Automated Testing** to ensure quality
---

## 🎯 Main Features

### 📦 Sales Module - Construction Supplies
- ✅ Complete catalog of products and materials
- ✅ Categorization and advanced search
- ✅ Real-time inventory control
- ✅ Price, cost, and SKU management
- ✅ Sales system with invoicing
- ✅ Sales reports and export to Excel/PDF

### 🚗 Vehicle Rental Module
- ✅ Management of commercial vehicle fleets
- ✅ Reservation system and availability control
- ✅ Management of active and completed rentals
- ✅ Complete history by client and vehicle
- ✅ Automatic calculation of rental costs
### 👥 Customer Management
- ✅ Customer registration and administration
- ✅ Purchase and rental history
- ✅ Roles and permissions system
- ✅ Secure authentication with JWT
- ✅ Complete data deletion (GDPR compliant)

### 💼 Administrative Dashboard (Firmness.Web)
- ✅ Complete control panel for administrators
- ✅ Visual management of clients, products, and vehicles
- ✅ Business reports and statistics
- ✅ Data export to Excel and PDF
- ✅ Responsive interface developed in ASP.NET MVC

### 🔌 REST API (Firmness.Api)
- ✅ Complete and documented RESTful API
- ✅ Swagger/OpenAPI for interactive documentation
- ✅ JWT Bearer authentication
- ✅ Endpoints for all business modules
- ✅ Designed for Angular frontend integration

### 🔐 Security and Authentication
- ✅ ASP.NET Core Identity for user management
- ✅ JWT (JSON Web Tokens) for APIs
- ✅ Role system: Admin and Client
- ✅ Endpoint authorization policies
- ✅ Protection against common attacks (CORS, XSS)

### 🐳 DevOps and Deployment
- ✅ Docker Compose for service orchestration
- ✅ Containers for API, Web, PostgreSQL, and PgAdmin
- ✅ Environment variables for configuration
- ✅ CI/CD ready
- ✅ Automated migration scripts

---

## 🏗 System Architecture

### Clean Architecture in 4 Layers

```
┌─────────────────────────────────────────────┐
│         Presentation Layer                  │
│  ┌──────────────┐  ┌──────────────┐        │
│  │ Firmness.Api │  │ Firmness.Web │        │
│  │  (REST API)  │  │  (MVC Admin) │        │
│  └──────────────┘  └──────────────┘        │
├─────────────────────────────────────────────┤
│        Application Layer                    │
│  ┌──────────────────────────────┐          │
│  │   Firmness.Application        │          │
│  │  - Services                   │          │
│  │  - DTOs, Interfaces           │          │
│  └──────────────────────────────┘          │
├─────────────────────────────────────────────┤
│          Domain Layer (Core)                │
│  ┌──────────────────────────────┐          │
│  │   Firmness.Domain             │          │
│  │  - Entities (Customer,        │          │
│  │    Product, Vehicle, Sale)    │          │
│  │  - Business Rules             │          │
│  └──────────────────────────────┘          │
├─────────────────────────────────────────────┤
│       Infrastructure Layer                  │
│  ┌──────────────────────────────┐          │
│  │   Firmness.Infrastructure     │          │
│  │  - EF Core + PostgreSQL       │          │
│  │  - Identity                   │          │
│  │  - Email Service              │          │
│  │  - Repositories               │          │
│  └──────────────────────────────┘          │
└─────────────────────────────────────────────┘

┌─────────────────────────────────────────────┐
│        Frontend (en desarrollo)             │
│  ┌──────────────────────────────┐          │
│  │   Client (Angular 17)         │          │
│  │  - Módulo de Cliente          │          │
│  │  - Interfaz web moderna       │          │
│  └──────────────────────────────┘          │
└─────────────────────────────────────────────┘
```

### System Modules

```
Firmness/
├── src/
│   ├── Firmness.Api/           # 🔌 API REST
│   ├── Firmness.Web/           # 💼 Dashboard Admin (MVC)
│   ├── Firmness.Application/   # 📋 Use Cases and Services
│   ├── Firmness.Domain/        # 🏛️ Business Entities
│   └── Firmness.Infrastructure/# 🔧 Data, Identity, Email
├── client/                     # 🎨 Frontend Angular 
├── tests/
│   └── Firmness.Test/          # ✅ Automated Testing
└── docs/                       # 📚 Technical Documentation
```

---

## 🚀Quick Start

### 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Docker Desktop](https://www.docker.com/) (recommended for production)
- [Node.js 18+](https://nodejs.org/) (for Angular client)

### 🐳 Option 1: Docker Compose (Recommended)

The fastest way to run the entire project:

#### 🚀 Quick Start with Scripts

**Windows (PowerShell):**
```powershell
# Execute the automated script
.\rebuild-docker.ps1
```

**Linux/macOS:**
```bash
# Make the script executable
chmod +x rebuild-docker.sh

# Execute the automated script
./rebuild-docker.sh
```

The script will:
- ✅ Stop existing containers
- ✅ Rebuild images without cache
- ✅ Start all services
- ✅ Show logs and service status

#### 📝 Manual Setup

```bash
    # 1. Clone repository
    git clone <repo-url>
    cd ConstrurentApp.NET
    
    # 2. Configure environment variables
    cp .env.example .env
    # Edit .env with your PostgreSQL credentials
    
    # 3. Start all services (API, Web, PostgreSQL, PgAdmin)
    docker-compose up -d
    
    # 4. Apply migrations (first time)
    docker-compose exec web dotnet ef database update
    
    # 5. Access the services
    # API: https://localhost:7192
    # Dashboard: http://localhost:5000
    # PgAdmin: http://localhost:8080
```

> **⚠️ CORS Issues on Linux?** If you encounter "Failed to fetch" errors when running on Linux, see the complete guide: **[docs/CORS_FIX_LINUX.md](docs/CORS_FIX_LINUX.md)**

### 💻 Option 2: Local Development

For active development without Docker:

```bash
        # 1. Configure environment variables
        cp .env.example .env
        # Edit .env with local PostgreSQL credentials
        
        # 2. Restore dependencies
        dotnet restore
        
        # 3. Apply database migrations
        cd src/Firmness.Infrastructure
        dotnet ef database update --startup-project ../Firmness.Api
        
        # 4. Start REST API (Terminal 1)
        cd ../Firmness.Api
        dotnet run
        # API available at: https://localhost:7192
        
        # 5. Start Web Dashboard (Terminal 2)
        cd ../Firmness.Web
        dotnet run
        # Dashboard at: http://localhost:5000
        
        # 6. Start Angular Client (Terminal 3) - Optional
        cd ../../client
        npm install
        ng serve
        # Client at: http://localhost:4200
```

---

## 🔐 Default Credentials

### Administrative Dashboard
- **URL:** http://localhost:5000
- **Email:** `admin@firmness.com`
- **Password:** `Admin123!`

### PostgreSQL Database
- **Host:** `localhost:5432`
- **Database:** `FirmnessDB`
- **User:** `postgres`
- **Password:** (configure in `.env`)

### PgAdmin (with Docker)
- **URL:** http://localhost:8080
- **Email:** `admin@firmness.com`
- **Password:** `admin123`

---

## 🔗 Servicios y URLs

| Service            | URL | Description                      |
|--------------------|-----|----------------------------------|
| **API REST**       | https://localhost:7192 | Main RESTful API           |
| **Swagger**        | https://localhost:7192/swagger | Interactive API documentation    |
| **Dashboard Web**  | http://localhost:5000 | MVC Administrative Panel       |
| **Client Angular** | http://localhost:4200 | Modern interface (under development)) |
| **PgAdmin**        | http://localhost:8080 | PostgreSQL Administration       |

---

## 📚 Technical Documentation

### 🔧 Setup and Configuration
- **[Environment Variables](docs/setup/ENVIRONMENT.md)** - Configuring `.env` and credentials
- **[Email](docs/setup/EMAIL_CONFIGURATION.md)** - Configuring Gmail SMTP for notifications

### 💻 Development Guides
- **[Architecture](docs/development/ARCHITECTURE.md)** - Hexagonal architecture, ports, and adapters
- **[Migrations](docs/development/MIGRATIONS.md)** - Entity Framework Core, commands, and best practices

### 🔌 API Documentation
- **[Endpoints](docs/api/ENDPOINTS.md)** - Complete list of REST endpoints
- **[Authentication](docs/api/AUTHENTICATION.md)** - JWT, Roles and Authorization
- **[Test Collection](docs/api/TEST_ENDPOINTS.http)** - Examples with REST Client

### 🧪 Testing Documentation
- **[Test Summary](docs/TEST_SUMMARY.md)** - Complete test coverage and analysis (90 tests)
- **[Test Guide](tests/Firmness.Test/README_TESTS.md)** - Quick reference for running tests

### 📖 More Documentation
See [docs/README.md](docs/README.md) for the complete technical documentation index.

---

## 🧪 Testing

### Automated Tests with xUnit

The project includes **90 comprehensive tests** covering services, controllers, and database integration:

```
✅ Services: 60 tests (Business Logic)
✅ Controllers: 29 tests (HTTP APIs)
✅ Integration: 1 test (Database)
Status: 100% Passing
```

**Quick Commands:**

```bash
# Run all tests
dotnet test

# Run only service tests (most important)
dotnet test --filter "FullyQualifiedName~Services"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

📘 **See [Test Documentation](tests/Firmness.Test/README_TESTS.md) for detailed information.**

### Test Coverage

**Integration Tests:**
- ✅ **AuthenticationTests** - Login, registration, JWT validation
- ✅ **CustomersApiTests** - CRUD operations, pagination, authorization
- ✅ **ProductRepositoryIntegrationTests** - Repository layer

**Test Technologies:**
- xUnit - Test framework
- FluentAssertions - Readable assertions
- Moq - Mocking framework
- ASP.NET Core Testing - Integration tests with in-memory DB

See [tests/Firmness.Test/README.md](tests/Firmness.Test/README.md) for detailed testing documentation.

### Manual API Testing

Use the `docs/api/TEST_ENDPOINTS.http` file with **REST Client** extension (VS Code):

```http
### Login as Admin
POST https://localhost:7192/api/Auth/login
Content-Type: application/json

{
  "email": "admin@firmness.com",
  "password": "Admin123!"
}
```

---

## 🛠️ Technology Stack
### Backend (.NET)
- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core 8.0
- **Data base:** PostgreSQL 15
- **Autenticación:** ASP.NET Core Identity + JWT
- **Mapeo:** AutoMapper
- **API Docs:** Swagger/OpenAPI (Swashbuckle)
- **Email:** System.Net.Mail + Gmail SMTP

### Frontend
- **Framework:** Angular 17 (in development)
- **UI:** Angular Material / Bootstrap
- **Estado:** RxJS

### DevOps
- **Containers:** Docker + Docker Compose
- **Orchestration:** Docker Compose
- **Database:** PostgreSQL (official container)
- **Tools:** PgAdmin 4 (container)

### Testing
- **Framework:** xUnit
- **Mocking:** Moq (futuro)
- **Assertions:** FluentAssertions (futuro)

---

## 📦 Main Entities

### Customer Management
- **Customer** - Complete customer information
- **ApplicationUser** - Identity users (authentication)

### Product Management
- **Product** - Construction materials
- **Category** - Product categories

### Sales Management
- **Sale** - Sales made
- **SaleItem** - Individual items in each sale

### Vehicle Management
- **Vehicle** - Available industrial vehicles
- **VehicleRental** - Active/historical vehicle rentals

---

## 🔧 Useful Commands

### Development

```bash
    # Compile complete solution
    dotnet build
    
    # Run tests
    dotnet test
    
    # Create new migration
    cd src/Firmness.Infrastructure
    dotnet ef migrations add MigrationName --startup-project ../Firmness.Api
    
    # Apply migrations
    dotnet ef database update --startup-project ../Firmness.Api
    
    # Revert last migration
    dotnet ef migrations remove --startup-project ../Firmness.Api
```

### Docker

```bash
        # Start services
        docker-compose up -d
        
        # View logs in real time
        docker-compose logs -f
        
        # Restart a specific service
        docker-compose restart api
        
        # Stop services
        docker-compose down
        
        # Clean up volumes (⚠️ Deletes the database)
        docker-compose down -v
```

### Angular Frontend

```bash
    cd client
    
    # Install dependencies
    npm install
    
    # Development server
    ng serve
    
    # Production build
    ng build --configuration production
    
    # Run tests
    ng test
```

---

## 🆘 Troubleshooting / Solución de Problemas

### 🔴 CORS Errors on Linux/Docker
If you encounter errors like "Failed to fetch" or CORS issues when running on Linux:

**Quick Solution:**
```bash
# Use the automated rebuild script
./rebuild-docker.sh
```

**Complete Documentation:** [docs/CORS_FIX_LINUX.md](docs/CORS_FIX_LINUX.md)

**Common Issues:**
- ✅ HTTPS redirection disabled in Docker containers
- ✅ CORS properly configured with exposed headers
- ✅ Nginx proxy configuration fixed
- ✅ Environment variables set correctly

### 🔴 Database Connection Issues
```bash
# Check database container is running
docker ps | grep firmness_db

# View database logs
docker logs firmness_db

# Restart database container
docker-compose restart db
```

### 🔴 API Not Responding
```bash
# Check API logs
docker logs firmness_api -f

# Verify environment variables
docker exec firmness_api env | grep JWT
docker exec firmness_api env | grep CONN_STR

# Restart API
docker-compose restart api
```

### 🔴 Frontend Issues
```bash
# Check nginx logs
docker logs firmness_client

# Rebuild only frontend
docker-compose build client
docker-compose up -d client
```

### 🔴 Port Already in Use
```bash
# Find what's using the port (Linux)
sudo lsof -i :5000

# Find what's using the port (Windows)
netstat -ano | findstr :5000

# Change port in docker-compose.yml or .env file
```

### 📚 More Help
- **API Documentation:** [docs/api/ENDPOINTS.md](docs/api/ENDPOINTS.md)
- **Architecture Guide:** [docs/development/ARCHITECTURE.md](docs/development/ARCHITECTURE.md)
- **Test Guide:** [tests/Firmness.Test/README_TESTS.md](tests/Firmness.Test/README_TESTS.md)

---

## 🤝 Contribución

### Flujo de Trabajo

1. **Fork** the repository
2. Create a branch for your feature:
   ```bash
   git checkout -b feature/NewFunctionality
   ```
3. Make your changes following the standards.
4. Commit with descriptive messages:
   ```bash
   git commit -m "feat: add reports module"
   ```
5. Push your fork:
   ```bash
   git push origin feature/NewDunctionality
   ```
6. Open a **Pull Request**

### ECoding Standards

#### Backend (.NET)
- ✅ Follow **Hexagonal Architecture (Ports and Adapters)**
- ✅ Use **PascalCase** naming conventions for classes and methods
- ✅ Document public methods with XML comments
- ✅ Write **tests** for new functionality
- ✅ Keep controllers **lean** (logic in services)
- ✅ The **Domain** should not depend on external frameworks

#### Frontend (Angular)
- ✅ Follow the **Angular style guide**
- ✅ Use **small and reusable** components
- ✅ Use **TypeScript strict mode**
- ✅ Implement **lazy loading** for routes

#### Database
- ✅ Use **EF Core migrations**, never manual changes
- ✅ Use **singular** table names: `Customer`, `Product`
- ✅ Configure **indexes** in frequently searched columns

---

## 📄 License

This project is licensed under the **MIT** license. See [LICENSE](LICENSE) for details.


## 👥 Team

Developed as an academic project for the comprehensive management of construction companies.

---

## 📞 Support

- **Documentation:** [/docs](docs/)

---

## 🗺️ Roadmap

### ✅ Completed (v1.0)
- [x] Complete REST API with JWT
- [x] MVC administrative dashboard
- [x] Client and product management
- [x] Sales system
- [x] Vehicle and rental management
- [x] Docker Compose deployment
- [x] Basic integration tests

### 🚧 In Development (v1.5)
- [ ] Complete Angular client
- [ ] Real-time statistics dashboard
- [ ] Push notification system
- [ ] More automated tests

### 🔮 Future (v2.0)
- [ ] Electronic invoicing system
- [ ] Payment gateway integration
- [ ] Mobile application (React Native)
- [ ] BI dashboard with advanced charts
- [ ] External ERP integration

---

<div align="center">

**Firmness** - Digitalize your construction business 🏗️

[Documentation](docs/) • [API](https://localhost:7192/swagger) • [Dashboard](http://localhost:5000)

**Made with ❤️ using .NET 8, Angular, and Docker**

</div>


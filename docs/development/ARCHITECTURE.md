# Project Architecture

## 📐 Structure

The project follows **Clean Architecture** with a clear separation of responsibilities:

```
Firmness/
├── src/
│   ├── Firmness.Api/           # REST API (ASP.NET Core)
│   ├── Firmness.Web/           # Web Admin Dashboard (MVC)
│   ├── Firmness.Application/   # Use cases and services
│   ├── Firmness.Domain/        # Entities and business logic
│   └── Firmness.Infrastructure/ # Data access, Identity, Email
├── client/                     # Angular Frontend
└── tests/
    └── Firmness.Test/          # Integration tests
```

## 🎯 Layers

### 1. Domain (Core)

**Responsibility:** Business entities and rules

```
Firmness.Domain/
├── Entities/        # Customer, Product, Sale, etc.
├── DTOs/           # Data Transfer Objects
├── Interfaces/     # Contracts (IRepository, IEmailService)
├── Enums/          # Enumerations
└── Common/         # Base classes, helpers
```

**Dependencies:** None (independent core)

### 2. Application

**Responsibility:** Use cases and application logic

```
Firmness.Application/
├── Services/       # CustomerService, NotificationService
├── Interfaces/     # ICustomerService, INotificationService
├── Models/         # ViewModels, DTOs
└── Mappings/       # AutoMapper profiles
```

**Dependencies:** `Firmness.Domain`

### 3. Infrastructure

**Responsibility:** Concrete implementations (DB, Email, Identity)

```
Firmness.Infrastructure/
├── Data/                    # ApplicationDbContext
├── Repositories/            # IRepository implementation
├── Identity/               # ApplicationUser, roles
├── Email/                  # GmailEmailService
├── Migrations/             # EF Core migrations
└── DependencyInjection/    # Service registration
```

**Dependencies:** `Firmness.Domain`, `Firmness.Application`

### 4. Presentation

**REST API:**
```
Firmness.Api/
├── Controllers/    # AuthController, CustomersApiController
└── Program.cs      # Configuration and DI
```

**Web Dashboard:**
```
Firmness.Web/
├── Controllers/    # CustomersController, ProductsController
├── Views/          # Razor views
├── ViewModels/     # View models
└── Areas/Identity/ # Identity pages
```

**Dependencies:** `Firmness.Application`, `Firmness.Infrastructure`

## 🔄 Data Flow

```
Request → Controller → Service (Application) → Repository → Database
                           ↓
                    Domain Entities
```

### Example: Register Customer

```
POST /api/Auth/register-client
    ↓
AuthController.RegisterClient()
    ↓
UserManager.CreateAsync() (Identity)
    ↓
CustomerService.AddAsync()
    ↓
Repository.AddAsync()
    ↓
DbContext.SaveChangesAsync()
    ↓
NotificationService.SendWelcomeEmailAsync()
```

## 🔌 Dependency Injection

Each layer registers its services in `DependencyInjection/`:

```csharp
// Application
services.AddScoped<ICustomerService, CustomerService>();

// Infrastructure
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<IEmailService, GmailEmailService>();
```

## 🔐 Identity & Authorization

**Identity Framework:** ASP.NET Core Identity

**Roles:**
- `Admin` - Full access to dashboard
- `Client` - Registered clients

**JWT:** For REST API authentication

## 📦 Patterns Used

- **Repository Pattern** - Data access abstraction
- **Unit of Work** - `IUnitOfWork` for transactions
- **Dependency Injection** - ASP.NET Core native IoC container
- **DTO Pattern** - Separation between entities and transfer models
- **Service Layer** - Business logic in Application

## 🧪 Testing

```
Firmness.Test/
├── ProductRepositoryIntegrationTests.cs
└── MockEmailService.cs
```

Uses in-memory database or test container for integration tests.

## 🔧 Configuration

Everything centralized in `Program.cs`:
- DbContext with PostgreSQL
- Identity with roles
- JWT authentication
- CORS
- AutoMapper
- Application services

## 📚 References

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/)


# Firmness - Angular Frontend

Modern Angular frontend with **Clean Architecture** (4-layer pattern) for the Firmness construction rental platform.

> ✅ **Implementación Completa** - 23 archivos creados siguiendo Clean Architecture  
> 📖 Ver [ARCHITECTURE_IMPLEMENTED.md](./ARCHITECTURE_IMPLEMENTED.md) para detalles

---

## 🏗️ Architecture

This project follows **Hexagonal Architecture** to ensure:
- Clean separation of concerns
- Testability and maintainability
- Independence from external frameworks
- Easy API integration changes

### Project Structure

```
src/app/
├── core/              # Application core (services, guards, state)
├── domain/            # Business logic & models (pure TypeScript)
├── features/          # Feature modules (smart components)
├── infrastructure/    # External adapters (HTTP, Storage)
├── layout/            # Layout components (header, footer, shell)
└── shared/            # Shared UI components & utilities
```

**Read more:**
- 📖 [Complete Architecture Documentation](./docs/ARCHITECTURE.md)
- 🚀 [Step-by-step Implementation Guide](./docs/IMPLEMENTATION_GUIDE.md)

---

## 🚀 Quick Start

### Prerequisites

- Node.js 18+
- Angular CLI 19+
- Backend API running on `https://localhost:7192`

### Installation

```bash
# Install dependencies
npm install

# Start development server
npm start
# or
ng serve
```

Open your browser and navigate to `http://localhost:4200/`

---

## 📋 Available Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Start development server |
| `npm run build` | Build for production |
| `npm test` | Run unit tests |
| `npm run watch` | Build in watch mode |

---

## 🎯 Key Features

- ✅ Hexagonal Architecture (mirrors backend)
- ✅ Angular 19 with Standalone Components
- ✅ Lazy-loaded feature modules
- ✅ JWT Authentication with interceptors
- ✅ Role-based access control
- ✅ Reactive forms with validation
- ✅ HTTP error handling
- ✅ Local storage abstraction

---

## 📁 Folder Structure

### Domain Layer (Business Logic)
```
domain/
├── models/            # Business entities (Customer, Product, Sale)
├── enums/             # Enumerations (Role, SaleStatus)
├── ports/             # Interfaces/Contracts
│   ├── repositories/  # Repository interfaces
│   └── services/      # Service interfaces
└── value-objects/     # Immutable objects
```

### Infrastructure Layer (Adapters)
```
infrastructure/
├── http/              # HTTP adapters (API clients)
│   ├── interceptors/  # Auth & error interceptors
│   └── *.repository.ts
├── storage/           # Storage adapters (LocalStorage)
├── mappers/           # DTO ↔ Domain mappers
└── config/            # Configuration services
```

### Core Layer (Application)
```
core/
├── services/          # Application services (facades)
│   ├── auth.service.ts
│   └── customer-facade.service.ts
├── guards/            # Route guards
│   ├── auth.guard.ts
│   └── role.guard.ts
└── state/             # Application state (optional)
```

### Features Layer
```
features/
├── auth/              # Authentication feature
│   ├── pages/         # Login, Register
│   └── auth.routes.ts
├── customers/         # Customer management
├── products/          # Product management
├── sales/             # Sales feature
└── vehicles/          # Vehicle rentals
```

---

## 🔧 Configuration

### Environment Variables

**File:** `src/environments/environment.ts`

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7192/api',
  apiTimeout: 30000
};
```

### Path Aliases

Path aliases are configured in `tsconfig.json`:

```typescript
import { Customer } from '@domain/models/customer.model';
import { CustomerRepository } from '@domain/ports/repositories/customer.repository';
import { AuthService } from '@core/services/auth.service';
```

---

## 🧪 Testing

```bash
# Run unit tests
npm test

# Run tests in watch mode
ng test --watch

# Generate coverage report
ng test --code-coverage
```

---

## 🛠️ Code Scaffolding

Generate code following hexagonal architecture:

```bash
# Generate a new feature module
ng generate component features/my-feature/pages/my-page

# Generate a domain model
# Create manually in: src/app/domain/models/

# Generate a repository adapter
ng generate service infrastructure/http/my-entity-http-repository

# Generate a facade service
ng generate service core/services/my-entity-facade

# Generate a shared component
ng generate component shared/components/my-component
```

---

## 🔌 API Integration

### Backend API

The frontend connects to the .NET backend API:

- **Base URL:** `https://localhost:7192/api`
- **Authentication:** JWT Bearer Token
- **Endpoints:** See [API Documentation](../docs/api/ENDPOINTS.md)

### Key Repositories

| Repository | Endpoint | Description |
|------------|----------|-------------|
| `AuthHttpRepository` | `/api/auth` | Login, register, logout |
| `CustomerHttpRepository` | `/api/customers` | Customer CRUD |
| `ProductHttpRepository` | `/api/products` | Product management |
| `SaleHttpRepository` | `/api/sales` | Sales operations |
| `VehicleHttpRepository` | `/api/vehicles` | Vehicle rentals |

---

## 🔐 Authentication Flow

1. User logs in via `AuthService.login()`
2. JWT token received and stored in `LocalStorage`
3. `authInterceptor` adds token to all HTTP requests
4. `AuthGuard` protects private routes
5. `RoleGuard` checks user roles for admin routes

**Example:**

```typescript
// Login
this.authService.login(new LoginRequest(email, password)).subscribe({
  next: (response) => {
    console.log('Logged in:', response.email);
    this.router.navigate(['/dashboard']);
  },
  error: (error) => console.error('Login failed', error)
});

// Check authentication
if (this.authService.isAuthenticated()) {
  // User is logged in
}

// Check role
if (this.authService.hasRole('Admin')) {
  // User is admin
}
```

---

## 📐 Hexagonal Architecture Benefits

| Benefit | Description |
|---------|-------------|
| **Testability** | Mock repositories easily for unit tests |
| **Maintainability** | Clear separation of concerns |
| **Flexibility** | Swap HTTP for WebSockets without changing domain |
| **Scalability** | Add features without breaking existing code |
| **Backend Alignment** | Mirrors backend hexagonal architecture |

---

## 📚 Documentation

- 📖 [Architecture Overview](./docs/ARCHITECTURE.md) - Complete hexagonal architecture explanation
- 🚀 [Implementation Guide](./docs/IMPLEMENTATION_GUIDE.md) - Step-by-step implementation
- 🔌 [API Endpoints](../docs/api/ENDPOINTS.md) - Backend API reference
- 🔐 [Authentication](../docs/api/AUTHENTICATION.md) - JWT authentication guide

---

## 🏗️ Building for Production

```bash
# Build with production configuration
npm run build

# Output will be in dist/ folder
# Deploy to your web server
```

**Environment configuration:**

Create `src/environments/environment.production.ts` with production API URL.

---

## 🧩 Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 19.2+ | Frontend framework |
| TypeScript | 5.7+ | Programming language |
| RxJS | 7.8+ | Reactive programming |
| Angular Router | 19.2+ | Routing & navigation |
| HttpClient | 19.2+ | HTTP communication |

---

## 📝 Contributing

When adding new features:

1. Create domain models first (`domain/models/`)
2. Define ports (interfaces) (`domain/ports/`)
3. Implement adapters (`infrastructure/http/`)
4. Create facade services (`core/services/`)
5. Build feature components (`features/`)
6. Add shared UI components (`shared/`)

---

## 🔗 Related Projects

- [Backend API](../src/Firmness.Api/) - .NET Core REST API
- [Admin Dashboard](../src/Firmness.Web/) - MVC Admin Panel
- [Tests](../tests/Firmness.Test/) - Backend unit tests

---

## 📄 License

See [LICENSE](../LICENSE) file in root directory.

---

## 🆘 Support

For questions or issues:
- Check [Backend Architecture](../docs/development/ARCHITECTURE.md)
- Review [API Documentation](../docs/api/ENDPOINTS.md)
- See [Implementation Guide](./docs/IMPLEMENTATION_GUIDE.md)

---

**Architecture:** Hexagonal (Ports & Adapters)
**Framework:** Angular 19 (Standalone Components)  
**Backend:** .NET 8 REST API

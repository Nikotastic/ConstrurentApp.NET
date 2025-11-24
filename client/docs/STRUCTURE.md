# Frontend Structure - Visual Guide

Visual representation of the Angular frontend architecture and file organization.

---

## 📂 Complete File Structure

```
client/
├── src/
│   ├── app/
│   │   ├── core/                               # APPLICATION CORE (Singletons)
│   │   │   ├── services/                       # Application services (facades)
│   │   │   │   ├── auth.service.ts            # Authentication facade
│   │   │   │   ├── customer-facade.service.ts # Customer use cases
│   │   │   │   ├── product-facade.service.ts  # Product use cases
│   │   │   │   └── notification.service.ts    # Notifications
│   │   │   │
│   │   │   ├── guards/                         # Route guards
│   │   │   │   ├── auth.guard.ts              # Check if user is authenticated
│   │   │   │   └── role.guard.ts              # Check user roles (Admin, Client)
│   │   │   │
│   │   │   ├── state/                          # Application state (optional)
│   │   │   │   ├── auth.state.ts
│   │   │   │   └── customer.state.ts
│   │   │   │
│   │   │   └── utils/                          # Core utilities
│   │   │       ├── validators.ts
│   │   │       └── error-handler.ts
│   │   │
│   │   ├── domain/                             # BUSINESS LOGIC (Pure TypeScript)
│   │   │   ├── models/                         # Business entities
│   │   │   │   ├── customer.model.ts          # Customer entity
│   │   │   │   ├── product.model.ts           # Product entity
│   │   │   │   ├── sale.model.ts              # Sale entity
│   │   │   │   ├── vehicle.model.ts           # Vehicle entity
│   │   │   │   ├── vehicle-rental.model.ts    # Rental entity
│   │   │   │   ├── auth.model.ts              # Auth models (Login, Register, Response)
│   │   │   │   └── category.model.ts          # Category entity
│   │   │   │
│   │   │   ├── enums/                          # Enumerations
│   │   │   │   ├── role.enum.ts               # User roles (Admin, Client)
│   │   │   │   ├── sale-status.enum.ts        # Sale statuses
│   │   │   │   └── rental-status.enum.ts      # Rental statuses
│   │   │   │
│   │   │   ├── ports/                          # Interfaces (Contracts)
│   │   │   │   ├── repositories/               # Repository interfaces
│   │   │   │   │   ├── customer.repository.ts
│   │   │   │   │   ├── product.repository.ts
│   │   │   │   │   ├── sale.repository.ts
│   │   │   │   │   ├── vehicle.repository.ts
│   │   │   │   │   ├── vehicle-rental.repository.ts
│   │   │   │   │   └── auth.repository.ts
│   │   │   │   │
│   │   │   │   └── services/                   # Service interfaces
│   │   │   │       ├── storage.service.ts
│   │   │   │       └── notification.service.ts
│   │   │   │
│   │   │   └── value-objects/                  # Immutable value objects
│   │   │       ├── email.vo.ts
│   │   │       ├── money.vo.ts
│   │   │       └── phone.vo.ts
│   │   │
│   │   ├── infrastructure/                     # EXTERNAL ADAPTERS
│   │   │   ├── http/                           # HTTP adapters (API clients)
│   │   │   │   ├── customer-http.repository.ts # Customer API adapter
│   │   │   │   ├── product-http.repository.ts  # Product API adapter
│   │   │   │   ├── sale-http.repository.ts     # Sale API adapter
│   │   │   │   ├── vehicle-http.repository.ts  # Vehicle API adapter
│   │   │   │   ├── vehicle-rental-http.repository.ts
│   │   │   │   ├── auth-http.repository.ts     # Auth API adapter
│   │   │   │   │
│   │   │   │   └── interceptors/               # HTTP interceptors
│   │   │   │       ├── auth.interceptor.ts    # Add JWT token to requests
│   │   │   │       └── error.interceptor.ts   # Handle HTTP errors
│   │   │   │
│   │   │   ├── storage/                        # Storage adapters
│   │   │   │   ├── local-storage.service.ts   # LocalStorage implementation
│   │   │   │   └── session-storage.service.ts # SessionStorage implementation
│   │   │   │
│   │   │   ├── mappers/                        # DTO ↔ Domain mappers
│   │   │   │   ├── customer.mapper.ts
│   │   │   │   ├── product.mapper.ts
│   │   │   │   ├── sale.mapper.ts
│   │   │   │   └── vehicle.mapper.ts
│   │   │   │
│   │   │   └── config/                         # Configuration
│   │   │       ├── environment.service.ts
│   │   │       └── api-config.ts
│   │   │
│   │   ├── features/                           # FEATURE MODULES
│   │   │   ├── auth/                           # Authentication feature
│   │   │   │   ├── auth.routes.ts             # Auth routing
│   │   │   │   │
│   │   │   │   ├── pages/                      # Smart components
│   │   │   │   │   ├── login/
│   │   │   │   │   │   ├── login.component.ts
│   │   │   │   │   │   ├── login.component.html
│   │   │   │   │   │   ├── login.component.scss
│   │   │   │   │   │   └── login.component.spec.ts
│   │   │   │   │   │
│   │   │   │   │   └── register/
│   │   │   │   │       ├── register.component.ts
│   │   │   │   │       ├── register.component.html
│   │   │   │   │       ├── register.component.scss
│   │   │   │   │       └── register.component.spec.ts
│   │   │   │   │
│   │   │   │   └── components/                 # Feature-specific components
│   │   │   │       └── login-form/
│   │   │   │           ├── login-form.component.ts
│   │   │   │           ├── login-form.component.html
│   │   │   │           └── login-form.component.scss
│   │   │   │
│   │   │   ├── customers/                      # Customer management
│   │   │   │   ├── customers.routes.ts
│   │   │   │   │
│   │   │   │   ├── pages/
│   │   │   │   │   ├── customer-list/         # List all customers
│   │   │   │   │   │   ├── customer-list.component.ts
│   │   │   │   │   │   ├── customer-list.component.html
│   │   │   │   │   │   └── customer-list.component.scss
│   │   │   │   │   │
│   │   │   │   │   ├── customer-detail/       # View customer details
│   │   │   │   │   │   ├── customer-detail.component.ts
│   │   │   │   │   │   ├── customer-detail.component.html
│   │   │   │   │   │   └── customer-detail.component.scss
│   │   │   │   │   │
│   │   │   │   │   └── customer-form/         # Create/edit customer
│   │   │   │   │       ├── customer-form.component.ts
│   │   │   │   │       ├── customer-form.component.html
│   │   │   │   │       └── customer-form.component.scss
│   │   │   │   │
│   │   │   │   └── components/
│   │   │   │       └── customer-card/         # Display customer card
│   │   │   │           ├── customer-card.component.ts
│   │   │   │           ├── customer-card.component.html
│   │   │   │           └── customer-card.component.scss
│   │   │   │
│   │   │   ├── products/                       # Product management
│   │   │   │   ├── products.routes.ts
│   │   │   │   ├── pages/
│   │   │   │   │   ├── product-list/
│   │   │   │   │   ├── product-detail/
│   │   │   │   │   └── product-form/
│   │   │   │   └── components/
│   │   │   │       └── product-card/
│   │   │   │
│   │   │   ├── sales/                          # Sales feature
│   │   │   │   ├── sales.routes.ts
│   │   │   │   ├── pages/
│   │   │   │   │   ├── sale-list/
│   │   │   │   │   ├── sale-detail/
│   │   │   │   │   └── sale-form/
│   │   │   │   └── components/
│   │   │   │       └── sale-summary/
│   │   │   │
│   │   │   └── vehicles/                       # Vehicle rentals
│   │   │       ├── vehicles.routes.ts
│   │   │       ├── pages/
│   │   │       │   ├── vehicle-list/
│   │   │       │   ├── vehicle-detail/
│   │   │       │   ├── rental-list/
│   │   │       │   └── rental-form/
│   │   │       └── components/
│   │   │           └── vehicle-card/
│   │   │
│   │   ├── layout/                             # LAYOUT COMPONENTS
│   │   │   ├── header/
│   │   │   │   ├── header.component.ts
│   │   │   │   ├── header.component.html
│   │   │   │   └── header.component.scss
│   │   │   │
│   │   │   ├── footer/
│   │   │   │   ├── footer.component.ts
│   │   │   │   ├── footer.component.html
│   │   │   │   └── footer.component.scss
│   │   │   │
│   │   │   ├── sidebar/
│   │   │   │   ├── sidebar.component.ts
│   │   │   │   ├── sidebar.component.html
│   │   │   │   └── sidebar.component.scss
│   │   │   │
│   │   │   └── main-layout/
│   │   │       ├── main-layout.component.ts
│   │   │       ├── main-layout.component.html
│   │   │       └── main-layout.component.scss
│   │   │
│   │   ├── shared/                             # SHARED COMPONENTS & UTILITIES
│   │   │   ├── components/                     # Reusable UI components
│   │   │   │   ├── button/
│   │   │   │   │   ├── button.component.ts
│   │   │   │   │   ├── button.component.html
│   │   │   │   │   └── button.component.scss
│   │   │   │   │
│   │   │   │   ├── card/
│   │   │   │   │   ├── card.component.ts
│   │   │   │   │   ├── card.component.html
│   │   │   │   │   └── card.component.scss
│   │   │   │   │
│   │   │   │   ├── modal/
│   │   │   │   │   ├── modal.component.ts
│   │   │   │   │   ├── modal.component.html
│   │   │   │   │   └── modal.component.scss
│   │   │   │   │
│   │   │   │   ├── table/
│   │   │   │   │   ├── table.component.ts
│   │   │   │   │   ├── table.component.html
│   │   │   │   │   └── table.component.scss
│   │   │   │   │
│   │   │   │   ├── form-input/
│   │   │   │   │   ├── form-input.component.ts
│   │   │   │   │   ├── form-input.component.html
│   │   │   │   │   └── form-input.component.scss
│   │   │   │   │
│   │   │   │   ├── loading-spinner/
│   │   │   │   │   ├── loading-spinner.component.ts
│   │   │   │   │   ├── loading-spinner.component.html
│   │   │   │   │   └── loading-spinner.component.scss
│   │   │   │   │
│   │   │   │   └── alert/
│   │   │   │       ├── alert.component.ts
│   │   │   │       ├── alert.component.html
│   │   │   │       └── alert.component.scss
│   │   │   │
│   │   │   ├── pipes/                          # Custom pipes
│   │   │   │   ├── currency.pipe.ts
│   │   │   │   ├── date-format.pipe.ts
│   │   │   │   └── truncate.pipe.ts
│   │   │   │
│   │   │   ├── directives/                     # Custom directives
│   │   │   │   ├── highlight.directive.ts
│   │   │   │   └── auto-focus.directive.ts
│   │   │   │
│   │   │   └── utils/                          # Shared utilities
│   │   │       ├── format.utils.ts
│   │   │       ├── validation.utils.ts
│   │   │       └── date.utils.ts
│   │   │
│   │   ├── app.component.ts                    # Root component
│   │   ├── app.component.html
│   │   ├── app.component.scss
│   │   ├── app.component.spec.ts
│   │   ├── app.config.ts                       # App configuration & DI
│   │   └── app.routes.ts                       # Root routing
│   │
│   ├── environments/                           # Environment configurations
│   │   ├── environment.ts                      # Development
│   │   └── environment.production.ts           # Production
│   │
│   ├── assets/                                 # Static assets
│   │   ├── images/
│   │   ├── icons/
│   │   └── fonts/
│   │
│   ├── styles.scss                             # Global styles
│   ├── main.ts                                 # Application entry point
│   └── index.html                              # HTML entry point
│
├── docs/                                       # Frontend documentation
│   ├── ARCHITECTURE.md                         # Architecture explanation
│   ├── IMPLEMENTATION_GUIDE.md                 # Step-by-step guide
│   └── STRUCTURE.md                            # This file
│
├── angular.json                                # Angular CLI configuration
├── tsconfig.json                               # TypeScript configuration
├── tsconfig.app.json                           # App TypeScript config
├── tsconfig.spec.json                          # Test TypeScript config
├── package.json                                # NPM dependencies
├── package-lock.json                           # NPM lock file
└── README.md                                   # Frontend README
```

---

## 🎯 Layer Responsibilities

### 🧠 Domain Layer (Business Logic)
**Files:** `domain/`

- ❌ **No Angular dependencies** (`@angular/*`)
- ❌ No HTTP, LocalStorage, or external APIs
- ✅ Pure TypeScript classes and interfaces
- ✅ Business rules and validations
- ✅ Entity behavior (methods)

**Example:** `Customer.isDocumentValid()`, `Product.isAvailable()`

---

### 🔌 Infrastructure Layer (Adapters)
**Files:** `infrastructure/`

- ✅ Implements domain ports (interfaces)
- ✅ Contains Angular/HTTP dependencies
- ✅ Handles DTOs and API communication
- ✅ Maps API responses to domain models
- ❌ No business logic

**Example:** `CustomerHttpRepository` implements `CustomerRepository`

---

### 🎯 Core Layer (Application)
**Files:** `core/`

- ✅ Singleton services (`providedIn: 'root'`)
- ✅ Orchestrates use cases
- ✅ Uses domain ports (dependency injection)
- ✅ Application-wide guards and utilities
- ❌ No direct HTTP calls

**Example:** `AuthService` manages authentication state

---

### 📦 Features Layer (UI)
**Files:** `features/`

- ✅ Smart components (use facade services)
- ✅ Feature-specific routing
- ✅ Lazy-loaded modules
- ✅ Use shared components for UI
- ❌ No direct repository access

**Example:** `CustomerListComponent` uses `CustomerFacadeService`

---

### 🎨 Shared Layer (Reusable UI)
**Files:** `shared/`

- ✅ Dumb/Presentational components
- ✅ Receive data via `@Input()`
- ✅ Emit events via `@Output()`
- ✅ Pipes, directives, utilities
- ❌ No business logic
- ❌ No service calls

**Example:** `ButtonComponent`, `CardComponent`

---

### 🏗️ Layout Layer (Structure)
**Files:** `layout/`

- ✅ Application shell components
- ✅ Navigation, header, footer
- ✅ Main layout structure
- ❌ No business logic

**Example:** `HeaderComponent`, `SidebarComponent`

---

## 🔄 Data Flow Example

### Customer List Feature

```
1. User navigates to /customers
   ↓
2. CustomerListComponent (features/customers/pages/customer-list/)
   ↓
3. Calls CustomerFacadeService.getAllCustomers() (core/services/)
   ↓
4. Injects CustomerRepository (domain port)
   ↓
5. CustomerHttpRepository implements the port (infrastructure/http/)
   ↓
6. Makes HTTP GET to API
   ↓
7. CustomerMapper converts DTO → Domain model
   ↓
8. Returns Observable<Customer[]>
   ↓
9. Component receives domain entities
   ↓
10. Passes to CustomerCardComponent (shared/components/)
```

---

## 📋 File Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Component | `*.component.ts` | `customer-list.component.ts` |
| Service | `*.service.ts` | `auth.service.ts` |
| Repository | `*-http.repository.ts` | `customer-http.repository.ts` |
| Guard | `*.guard.ts` | `auth.guard.ts` |
| Interceptor | `*.interceptor.ts` | `auth.interceptor.ts` |
| Model | `*.model.ts` | `customer.model.ts` |
| Enum | `*.enum.ts` | `role.enum.ts` |
| Pipe | `*.pipe.ts` | `currency.pipe.ts` |
| Directive | `*.directive.ts` | `highlight.directive.ts` |
| Mapper | `*.mapper.ts` | `customer.mapper.ts` |

---

## 🚀 Quick Reference

### Import Aliases

```typescript
// Domain
import { Customer } from '@domain/models/customer.model';
import { CustomerRepository } from '@domain/ports/repositories/customer.repository';
import { Role } from '@domain/enums/role.enum';

// Infrastructure
import { CustomerHttpRepository } from '@infrastructure/http/customer-http.repository';
import { LocalStorageService } from '@infrastructure/storage/local-storage.service';

// Core
import { AuthService } from '@core/services/auth.service';
import { authGuard } from '@core/guards/auth.guard';

// Features
import { CustomerListComponent } from '@features/customers/pages/customer-list/customer-list.component';

// Shared
import { ButtonComponent } from '@shared/components/button/button.component';
import { CurrencyPipe } from '@shared/pipes/currency.pipe';

// Environment
import { environment } from '@environments/environment';
```

---

## 📚 Related Documentation

- [Architecture Overview](./ARCHITECTURE.md)
- [Implementation Guide](./IMPLEMENTATION_GUIDE.md)
- [Frontend README](../README.md)

---

**Last Updated:** November 2025  
**Architecture:** Hexagonal (Ports & Adapters)  
**Framework:** Angular 19


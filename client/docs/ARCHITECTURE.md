# Angular Frontend Architecture - Firmness

## 📐 Overview

The frontend follows **Hexagonal Architecture** (Ports & Adapters) to mirror the backend structure and ensure:
- Clean separation of concerns
- Testability and maintainability
- Independence from external frameworks
- Easy adaptation to API changes

---

## 🎯 Hexagonal Architecture Layers

```
client/src/app/
├── core/              # Application Core (Singleton services, guards)
├── domain/            # Business Logic & Models (Pure TypeScript)
├── features/          # Feature Modules (Smart Components)
├── infrastructure/    # External Adapters (HTTP, Storage, etc.)
├── layout/            # Layout Components (Shell, Nav, Footer)
└── shared/            # Shared UI Components & Utilities
```

---

## 📁 Layer Details

### 1. **Domain** (Business Logic)

**Purpose:** Pure business logic, entities, and contracts (Ports)

```
domain/
├── models/            # Business entities
│   ├── customer.model.ts
│   ├── product.model.ts
│   ├── sale.model.ts
│   └── vehicle.model.ts
│
├── enums/             # Enumerations
│   ├── role.enum.ts
│   └── sale-status.enum.ts
│
├── ports/             # Interfaces (Contracts)
│   ├── repositories/
│   │   ├── customer.repository.ts
│   │   ├── product.repository.ts
│   │   └── auth.repository.ts
│   └── services/
│       ├── notification.service.ts
│       └── storage.service.ts
│
└── value-objects/     # Immutable objects
    ├── email.vo.ts
    └── money.vo.ts
```

**Rules:**
- ❌ No Angular dependencies (`@angular/*`)
- ❌ No HTTP, LocalStorage, or external libraries
- ✅ Pure TypeScript classes and interfaces
- ✅ Only business rules and validations

**Example:**

```typescript
// domain/models/customer.model.ts
export class Customer {
  constructor(
    public id: string,
    public email: string,
    public firstName: string,
    public lastName: string,
    public document: string,
    public phone: string,
    public address: string
  ) {}

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }

  isDocumentValid(): boolean {
    return this.document.length >= 6;
  }
}

// domain/ports/repositories/customer.repository.ts
export abstract class CustomerRepository {
  abstract getAll(): Observable<Customer[]>;
  abstract getById(id: string): Observable<Customer>;
  abstract create(customer: Customer): Observable<Customer>;
  abstract update(id: string, customer: Customer): Observable<Customer>;
  abstract delete(id: string): Observable<void>;
}
```

---

### 2. **Infrastructure** (Adapters)

**Purpose:** Concrete implementations of domain ports (adapters to external services)

```
infrastructure/
├── http/              # HTTP Adapters (API clients)
│   ├── customer-http.repository.ts
│   ├── product-http.repository.ts
│   ├── auth-http.repository.ts
│   └── interceptors/
│       ├── auth.interceptor.ts
│       └── error.interceptor.ts
│
├── storage/           # Storage Adapters
│   ├── local-storage.service.ts
│   └── session-storage.service.ts
│
├── config/            # Configuration
│   ├── environment.service.ts
│   └── api-config.ts
│
└── mappers/           # DTOs to Domain mappers
    ├── customer.mapper.ts
    └── product.mapper.ts
```

**Rules:**
- ✅ Implements domain ports
- ✅ Contains Angular/HTTP dependencies
- ✅ Handles DTOs and API communication
- ✅ No business logic

**Example:**

```typescript
// infrastructure/http/customer-http.repository.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { CustomerRepository } from '@domain/ports/repositories/customer.repository';
import { Customer } from '@domain/models/customer.model';
import { CustomerMapper } from '@infrastructure/mappers/customer.mapper';

@Injectable({
  providedIn: 'root'
})
export class CustomerHttpRepository implements CustomerRepository {
  private apiUrl = 'https://localhost:7192/api/customers';

  constructor(
    private http: HttpClient,
    private mapper: CustomerMapper
  ) {}

  getAll(): Observable<Customer[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(dtos => dtos.map(dto => this.mapper.toDomain(dto)))
    );
  }

  getById(id: string): Observable<Customer> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(dto => this.mapper.toDomain(dto))
    );
  }

  create(customer: Customer): Observable<Customer> {
    const dto = this.mapper.toDTO(customer);
    return this.http.post<any>(this.apiUrl, dto).pipe(
      map(dto => this.mapper.toDomain(dto))
    );
  }

  update(id: string, customer: Customer): Observable<Customer> {
    const dto = this.mapper.toDTO(customer);
    return this.http.put<any>(`${this.apiUrl}/${id}`, dto).pipe(
      map(dto => this.mapper.toDomain(dto))
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

---

### 3. **Core** (Application Core)

**Purpose:** Singleton services, application state, guards, and use cases

```
core/
├── services/          # Application services (Use cases)
│   ├── auth.service.ts
│   ├── customer-facade.service.ts
│   └── notification-facade.service.ts
│
├── guards/            # Route guards
│   ├── auth.guard.ts
│   └── role.guard.ts
│
├── state/             # State management (optional: NgRx/Signals)
│   ├── auth.state.ts
│   └── customer.state.ts
│
└── utils/             # Core utilities
    ├── validators.ts
    └── error-handler.ts
```

**Rules:**
- ✅ Singleton services (`providedIn: 'root'`)
- ✅ Use domain ports (dependency injection)
- ✅ Orchestrate use cases
- ✅ No direct HTTP calls

**Example:**

```typescript
// core/services/customer-facade.service.ts
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer } from '@domain/models/customer.model';
import { CustomerRepository } from '@domain/ports/repositories/customer.repository';

@Injectable({
  providedIn: 'root'
})
export class CustomerFacadeService {
  constructor(
    private customerRepo: CustomerRepository // Injected port (will be HTTP adapter)
  ) {}

  getAllCustomers(): Observable<Customer[]> {
    return this.customerRepo.getAll();
  }

  getCustomerById(id: string): Observable<Customer> {
    return this.customerRepo.getById(id);
  }

  createCustomer(customer: Customer): Observable<Customer> {
    // Add business validations if needed
    if (!customer.isDocumentValid()) {
      throw new Error('Invalid document');
    }
    return this.customerRepo.create(customer);
  }

  updateCustomer(id: string, customer: Customer): Observable<Customer> {
    return this.customerRepo.update(id, customer);
  }

  deleteCustomer(id: string): Observable<void> {
    return this.customerRepo.delete(id);
  }
}
```

---

### 4. **Features** (Feature Modules)

**Purpose:** Feature-specific smart components and routing

```
features/
├── auth/
│   ├── auth.routes.ts
│   ├── pages/
│   │   ├── login/
│   │   │   ├── login.component.ts
│   │   │   ├── login.component.html
│   │   │   └── login.component.scss
│   │   └── register/
│   │       ├── register.component.ts
│   │       ├── register.component.html
│   │       └── register.component.scss
│   └── components/
│       └── login-form/
│
├── customers/
│   ├── customers.routes.ts
│   ├── pages/
│   │   ├── customer-list/
│   │   │   ├── customer-list.component.ts
│   │   │   ├── customer-list.component.html
│   │   │   └── customer-list.component.scss
│   │   ├── customer-detail/
│   │   └── customer-form/
│   └── components/
│       └── customer-card/
│
├── products/
│   ├── products.routes.ts
│   └── pages/
│
├── sales/
│   ├── sales.routes.ts
│   └── pages/
│
└── vehicles/
    ├── vehicles.routes.ts
    └── pages/
```

**Rules:**
- ✅ Smart components (use facade services)
- ✅ Feature-specific routing
- ✅ Lazy-loaded modules
- ✅ Use shared components for UI

**Example:**

```typescript
// features/customers/pages/customer-list/customer-list.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerFacadeService } from '@core/services/customer-facade.service';
import { Customer } from '@domain/models/customer.model';
import { CustomerCardComponent } from '@shared/components/customer-card/customer-card.component';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, CustomerCardComponent],
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent implements OnInit {
  customers: Customer[] = [];
  loading = false;

  constructor(private customerFacade: CustomerFacadeService) {}

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.loading = true;
    this.customerFacade.getAllCustomers().subscribe({
      next: (customers) => {
        this.customers = customers;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading customers', error);
        this.loading = false;
      }
    });
  }

  deleteCustomer(id: string): void {
    this.customerFacade.deleteCustomer(id).subscribe({
      next: () => this.loadCustomers(),
      error: (error) => console.error('Error deleting customer', error)
    });
  }
}
```

---

### 5. **Shared** (Shared Components & Utilities)

**Purpose:** Reusable dumb components, pipes, directives, and utilities

```
shared/
├── components/        # Reusable UI components
│   ├── button/
│   │   ├── button.component.ts
│   │   ├── button.component.html
│   │   └── button.component.scss
│   ├── card/
│   ├── modal/
│   ├── table/
│   └── form-input/
│
├── pipes/             # Custom pipes
│   ├── currency.pipe.ts
│   └── date-format.pipe.ts
│
├── directives/        # Custom directives
│   └── highlight.directive.ts
│
└── utils/             # Utilities
    ├── format.utils.ts
    └── validation.utils.ts
```

**Rules:**
- ✅ Dumb/Presentational components
- ✅ No business logic
- ✅ Receive data via `@Input()`
- ✅ Emit events via `@Output()`

**Example:**

```typescript
// shared/components/customer-card/customer-card.component.ts
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Customer } from '@domain/models/customer.model';

@Component({
  selector: 'app-customer-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card">
      <h3>{{ customer.fullName }}</h3>
      <p>{{ customer.email }}</p>
      <p>{{ customer.phone }}</p>
      <button (click)="onEdit()">Edit</button>
      <button (click)="onDelete()">Delete</button>
    </div>
  `,
  styleUrls: ['./customer-card.component.scss']
})
export class CustomerCardComponent {
  @Input() customer!: Customer;
  @Output() edit = new EventEmitter<Customer>();
  @Output() delete = new EventEmitter<string>();

  onEdit(): void {
    this.edit.emit(this.customer);
  }

  onDelete(): void {
    this.delete.emit(this.customer.id);
  }
}
```

---

### 6. **Layout** (Layout Components)

**Purpose:** Application shell, navigation, header, footer

```
layout/
├── header/
│   ├── header.component.ts
│   ├── header.component.html
│   └── header.component.scss
├── footer/
├── sidebar/
└── main-layout/
    ├── main-layout.component.ts
    ├── main-layout.component.html
    └── main-layout.component.scss
```

---

## 🔄 Dependency Injection Setup

**app.config.ts:**

```typescript
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';

// Domain Ports
import { CustomerRepository } from '@domain/ports/repositories/customer.repository';
import { ProductRepository } from '@domain/ports/repositories/product.repository';
import { AuthRepository } from '@domain/ports/repositories/auth.repository';

// Infrastructure Adapters
import { CustomerHttpRepository } from '@infrastructure/http/customer-http.repository';
import { ProductHttpRepository } from '@infrastructure/http/product-http.repository';
import { AuthHttpRepository } from '@infrastructure/http/auth-http.repository';
import { authInterceptor } from '@infrastructure/http/interceptors/auth.interceptor';
import { errorInterceptor } from '@infrastructure/http/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor, errorInterceptor])
    ),
    
    // Port → Adapter binding (Hexagonal Architecture)
    { provide: CustomerRepository, useClass: CustomerHttpRepository },
    { provide: ProductRepository, useClass: ProductHttpRepository },
    { provide: AuthRepository, useClass: AuthHttpRepository }
  ]
};
```

---

## 🚀 Benefits of This Architecture

| Benefit | Description |
|---------|-------------|
| **Testability** | Easy to mock repositories and test use cases |
| **Maintainability** | Clear separation of concerns |
| **Flexibility** | Swap HTTP for WebSockets without changing domain |
| **Scalability** | Add features without breaking existing code |
| **Backend Alignment** | Mirrors backend hexagonal architecture |

---

## 📦 Path Aliases (tsconfig.json)

Add these aliases for clean imports:

```json
{
  "compilerOptions": {
    "paths": {
      "@domain/*": ["src/app/domain/*"],
      "@infrastructure/*": ["src/app/infrastructure/*"],
      "@core/*": ["src/app/core/*"],
      "@features/*": ["src/app/features/*"],
      "@shared/*": ["src/app/shared/*"],
      "@layout/*": ["src/app/layout/*"]
    }
  }
}
```

---

## 📚 Next Steps

1. Create domain models and ports
2. Implement HTTP adapters in infrastructure
3. Create facade services in core
4. Build feature components
5. Add shared UI components
6. Configure routing and lazy loading

---

## 🔗 Related Documentation

- [Backend Architecture](../../docs/development/ARCHITECTURE.md)
- [API Endpoints](../../docs/api/ENDPOINTS.md)
- [Authentication](../../docs/api/AUTHENTICATION.md)

---

**Architecture Pattern:** Hexagonal (Ports & Adapters)  
**Framework:** Angular 19 (Standalone Components)  
**State Management:** (Optional: NgRx Signals / RxJS)


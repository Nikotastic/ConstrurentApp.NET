# Angular Frontend - Implementation Guide

Step-by-step guide to implement Hexagonal Architecture in the Angular frontend.

---

## 📋 Prerequisites

- Angular CLI 19+
- Node.js 18+
- Backend API running on `https://localhost:7192`

---

## 🚀 Step 1: Configure Path Aliases

**File:** `tsconfig.json`

```json
{
  "compilerOptions": {
    "baseUrl": "./",
    "paths": {
      "@domain/*": ["src/app/domain/*"],
      "@infrastructure/*": ["src/app/infrastructure/*"],
      "@core/*": ["src/app/core/*"],
      "@features/*": ["src/app/features/*"],
      "@shared/*": ["src/app/shared/*"],
      "@layout/*": ["src/app/layout/*"],
      "@environments/*": ["src/environments/*"]
    }
  }
}
```

---

## 🏗️ Step 2: Create Domain Layer

### 2.1 Create Domain Models

**File:** `src/app/domain/models/customer.model.ts`

```typescript
export class Customer {
  constructor(
    public id: string,
    public email: string,
    public firstName: string,
    public lastName: string,
    public document: string,
    public phone: string,
    public address: string,
    public isActive: boolean = true,
    public createdAt?: Date
  ) {}

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }

  isDocumentValid(): boolean {
    return this.document.length >= 6 && this.document.length <= 20;
  }

  isEmailValid(): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(this.email);
  }
}
```

**File:** `src/app/domain/models/product.model.ts`

```typescript
export class Product {
  constructor(
    public id: string,
    public name: string,
    public description: string,
    public price: number,
    public stock: number,
    public categoryId: string,
    public categoryName?: string,
    public photoUrl?: string,
    public isActive: boolean = true
  ) {}

  isAvailable(): boolean {
    return this.isActive && this.stock > 0;
  }

  getTotalValue(): number {
    return this.price * this.stock;
  }
}
```

**File:** `src/app/domain/models/auth.model.ts`

```typescript
export class LoginRequest {
  constructor(
    public email: string,
    public password: string
  ) {}
}

export class RegisterRequest {
  constructor(
    public username: string,
    public email: string,
    public password: string,
    public firstName: string,
    public lastName: string,
    public document: string,
    public phone: string,
    public address: string = ''
  ) {}
}

export class AuthResponse {
  constructor(
    public token: string,
    public email: string,
    public roles: string[],
    public expiresAt: Date
  ) {}

  isExpired(): boolean {
    return new Date() > this.expiresAt;
  }

  hasRole(role: string): boolean {
    return this.roles.includes(role);
  }
}
```

### 2.2 Create Domain Enums

**File:** `src/app/domain/enums/role.enum.ts`

```typescript
export enum Role {
  Admin = 'Admin',
  Client = 'Client'
}
```

**File:** `src/app/domain/enums/sale-status.enum.ts`

```typescript
export enum SaleStatus {
  Pending = 'Pending',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}
```

### 2.3 Create Domain Ports (Interfaces)

**File:** `src/app/domain/ports/repositories/customer.repository.ts`

```typescript
import { Observable } from 'rxjs';
import { Customer } from '@domain/models/customer.model';

export abstract class CustomerRepository {
  abstract getAll(): Observable<Customer[]>;
  abstract getById(id: string): Observable<Customer>;
  abstract create(customer: Customer): Observable<Customer>;
  abstract update(id: string, customer: Partial<Customer>): Observable<Customer>;
  abstract delete(id: string): Observable<void>;
}
```

**File:** `src/app/domain/ports/repositories/product.repository.ts`

```typescript
import { Observable } from 'rxjs';
import { Product } from '@domain/models/product.model';

export abstract class ProductRepository {
  abstract getAll(): Observable<Product[]>;
  abstract getById(id: string): Observable<Product>;
  abstract create(product: Product): Observable<Product>;
  abstract update(id: string, product: Partial<Product>): Observable<Product>;
  abstract delete(id: string): Observable<void>;
}
```

**File:** `src/app/domain/ports/repositories/auth.repository.ts`

```typescript
import { Observable } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse } from '@domain/models/auth.model';

export abstract class AuthRepository {
  abstract login(credentials: LoginRequest): Observable<AuthResponse>;
  abstract registerClient(data: RegisterRequest): Observable<AuthResponse>;
  abstract logout(): Observable<void>;
  abstract verifyToken(): Observable<boolean>;
}
```

**File:** `src/app/domain/ports/services/storage.service.ts`

```typescript
export abstract class StorageService {
  abstract setItem(key: string, value: string): void;
  abstract getItem(key: string): string | null;
  abstract removeItem(key: string): void;
  abstract clear(): void;
}
```

---

## 🔌 Step 3: Create Infrastructure Layer

### 3.1 Environment Configuration

**File:** `src/environments/environment.ts`

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7192/api',
  apiTimeout: 30000
};
```

**File:** `src/environments/environment.production.ts`

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-production-api.com/api',
  apiTimeout: 30000
};
```

### 3.2 Create Mappers

**File:** `src/app/infrastructure/mappers/customer.mapper.ts`

```typescript
import { Injectable } from '@angular/core';
import { Customer } from '@domain/models/customer.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerMapper {
  toDomain(dto: any): Customer {
    return new Customer(
      dto.id,
      dto.email,
      dto.firstName,
      dto.lastName,
      dto.document,
      dto.phone,
      dto.address,
      dto.isActive,
      dto.createdAt ? new Date(dto.createdAt) : undefined
    );
  }

  toDTO(customer: Customer): any {
    return {
      email: customer.email,
      firstName: customer.firstName,
      lastName: customer.lastName,
      document: customer.document,
      phone: customer.phone,
      address: customer.address,
      isActive: customer.isActive
    };
  }
}
```

**File:** `src/app/infrastructure/mappers/product.mapper.ts`

```typescript
import { Injectable } from '@angular/core';
import { Product } from '@domain/models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductMapper {
  toDomain(dto: any): Product {
    return new Product(
      dto.id,
      dto.name,
      dto.description,
      dto.price,
      dto.stock,
      dto.categoryId,
      dto.categoryName,
      dto.photoUrl,
      dto.isActive
    );
  }

  toDTO(product: Product): any {
    return {
      name: product.name,
      description: product.description,
      price: product.price,
      stock: product.stock,
      categoryId: product.categoryId,
      photoUrl: product.photoUrl,
      isActive: product.isActive
    };
  }
}
```

### 3.3 Create HTTP Repositories (Adapters)

**File:** `src/app/infrastructure/http/customer-http.repository.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { CustomerRepository } from '@domain/ports/repositories/customer.repository';
import { Customer } from '@domain/models/customer.model';
import { CustomerMapper } from '@infrastructure/mappers/customer.mapper';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CustomerHttpRepository implements CustomerRepository {
  private readonly apiUrl = `${environment.apiUrl}/customers`;

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

  update(id: string, customer: Partial<Customer>): Observable<Customer> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, customer).pipe(
      map(dto => this.mapper.toDomain(dto))
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

**File:** `src/app/infrastructure/http/auth-http.repository.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthRepository } from '@domain/ports/repositories/auth.repository';
import { LoginRequest, RegisterRequest, AuthResponse } from '@domain/models/auth.model';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthHttpRepository implements AuthRepository {
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      map(dto => new AuthResponse(
        dto.token,
        dto.email,
        dto.roles,
        new Date(dto.expiresAt)
      ))
    );
  }

  registerClient(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<any>(`${this.apiUrl}/register-client`, data).pipe(
      map(dto => new AuthResponse(
        dto.token,
        dto.email,
        dto.roles,
        new Date(dto.expiresAt)
      ))
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/logout`, {});
  }

  verifyToken(): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/verify`);
  }
}
```

### 3.4 Create Interceptors

**File:** `src/app/infrastructure/http/interceptors/auth.interceptor.ts`

```typescript
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { StorageService } from '@domain/ports/services/storage.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const storage = inject(StorageService);
  const token = storage.getItem('auth_token');

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};
```

**File:** `src/app/infrastructure/http/interceptors/error.interceptor.ts`

```typescript
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An error occurred';

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Error: ${error.error.message}`;
      } else {
        // Server-side error
        errorMessage = error.error?.message || `Error Code: ${error.status}`;
      }

      console.error('HTTP Error:', errorMessage);
      return throwError(() => new Error(errorMessage));
    })
  );
};
```

### 3.5 Create Storage Adapter

**File:** `src/app/infrastructure/storage/local-storage.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { StorageService } from '@domain/ports/services/storage.service';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService implements StorageService {
  setItem(key: string, value: string): void {
    localStorage.setItem(key, value);
  }

  getItem(key: string): string | null {
    return localStorage.getItem(key);
  }

  removeItem(key: string): void {
    localStorage.removeItem(key);
  }

  clear(): void {
    localStorage.clear();
  }
}
```

---

## 🎯 Step 4: Create Core Layer (Use Cases)

### 4.1 Auth Service (Facade)

**File:** `src/app/core/services/auth.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { AuthRepository } from '@domain/ports/repositories/auth.repository';
import { StorageService } from '@domain/ports/services/storage.service';
import { LoginRequest, RegisterRequest, AuthResponse } from '@domain/models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private authRepo: AuthRepository,
    private storage: StorageService
  ) {
    this.loadStoredUser();
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.authRepo.login(credentials).pipe(
      tap(response => this.storeAuthData(response))
    );
  }

  registerClient(data: RegisterRequest): Observable<AuthResponse> {
    return this.authRepo.registerClient(data).pipe(
      tap(response => this.storeAuthData(response))
    );
  }

  logout(): void {
    this.storage.removeItem('auth_token');
    this.storage.removeItem('auth_user');
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    const user = this.currentUserSubject.value;
    return user !== null && !user.isExpired();
  }

  hasRole(role: string): boolean {
    const user = this.currentUserSubject.value;
    return user?.hasRole(role) ?? false;
  }

  private storeAuthData(response: AuthResponse): void {
    this.storage.setItem('auth_token', response.token);
    this.storage.setItem('auth_user', JSON.stringify(response));
    this.currentUserSubject.next(response);
  }

  private loadStoredUser(): void {
    const storedUser = this.storage.getItem('auth_user');
    if (storedUser) {
      const userData = JSON.parse(storedUser);
      const user = new AuthResponse(
        userData.token,
        userData.email,
        userData.roles,
        new Date(userData.expiresAt)
      );
      if (!user.isExpired()) {
        this.currentUserSubject.next(user);
      } else {
        this.logout();
      }
    }
  }
}
```

### 4.2 Customer Facade Service

**File:** `src/app/core/services/customer-facade.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer } from '@domain/models/customer.model';
import { CustomerRepository } from '@domain/ports/repositories/customer.repository';

@Injectable({
  providedIn: 'root'
})
export class CustomerFacadeService {
  constructor(private customerRepo: CustomerRepository) {}

  getAllCustomers(): Observable<Customer[]> {
    return this.customerRepo.getAll();
  }

  getCustomerById(id: string): Observable<Customer> {
    return this.customerRepo.getById(id);
  }

  createCustomer(customer: Customer): Observable<Customer> {
    // Business validation
    if (!customer.isDocumentValid()) {
      throw new Error('Invalid document format');
    }
    if (!customer.isEmailValid()) {
      throw new Error('Invalid email format');
    }
    return this.customerRepo.create(customer);
  }

  updateCustomer(id: string, customer: Partial<Customer>): Observable<Customer> {
    return this.customerRepo.update(id, customer);
  }

  deleteCustomer(id: string): Observable<void> {
    return this.customerRepo.delete(id);
  }
}
```

### 4.3 Auth Guard

**File:** `src/app/core/guards/auth.guard.ts`

```typescript
import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/auth/login']);
  return false;
};
```

**File:** `src/app/core/guards/role.guard.ts`

```typescript
import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const requiredRole = route.data['role'] as string;

  if (authService.hasRole(requiredRole)) {
    return true;
  }

  router.navigate(['/']);
  return false;
};
```

---

## 🎨 Step 5: Configure Dependency Injection

**File:** `src/app/app.config.ts`

```typescript
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';

// Domain Ports
import { CustomerRepository } from '@domain/ports/repositories/customer.repository';
import { ProductRepository } from '@domain/ports/repositories/product.repository';
import { AuthRepository } from '@domain/ports/repositories/auth.repository';
import { StorageService } from '@domain/ports/services/storage.service';

// Infrastructure Adapters
import { CustomerHttpRepository } from '@infrastructure/http/customer-http.repository';
import { ProductHttpRepository } from '@infrastructure/http/product-http.repository';
import { AuthHttpRepository } from '@infrastructure/http/auth-http.repository';
import { LocalStorageService } from '@infrastructure/storage/local-storage.service';
import { authInterceptor } from '@infrastructure/http/interceptors/auth.interceptor';
import { errorInterceptor } from '@infrastructure/http/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor, errorInterceptor])
    ),
    
    // Hexagonal Architecture: Port → Adapter binding
    { provide: CustomerRepository, useClass: CustomerHttpRepository },
    { provide: ProductRepository, useClass: ProductHttpRepository },
    { provide: AuthRepository, useClass: AuthHttpRepository },
    { provide: StorageService, useClass: LocalStorageService }
  ]
};
```

---

## 📝 Step 6: Create Feature Module Example

**File:** `src/app/features/customers/customers.routes.ts`

```typescript
import { Routes } from '@angular/router';
import { authGuard } from '@core/guards/auth.guard';
import { roleGuard } from '@core/guards/role.guard';

export const customersRoutes: Routes = [
  {
    path: '',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' },
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/customer-list/customer-list.component')
          .then(m => m.CustomerListComponent)
      },
      {
        path: 'new',
        loadComponent: () => import('./pages/customer-form/customer-form.component')
          .then(m => m.CustomerFormComponent)
      },
      {
        path: ':id',
        loadComponent: () => import('./pages/customer-detail/customer-detail.component')
          .then(m => m.CustomerDetailComponent)
      }
    ]
  }
];
```

**File:** `src/app/app.routes.ts`

```typescript
import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.authRoutes)
  },
  {
    path: 'customers',
    loadChildren: () => import('./features/customers/customers.routes').then(m => m.customersRoutes)
  },
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full'
  }
];
```

---

## ✅ Step 7: Testing

Create tests for each layer:

```typescript
// Example: customer.repository.spec.ts
describe('CustomerHttpRepository', () => {
  let repository: CustomerHttpRepository;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CustomerHttpRepository, CustomerMapper]
    });
    repository = TestBed.inject(CustomerHttpRepository);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should fetch all customers', () => {
    const mockCustomers = [/* ... */];
    
    repository.getAll().subscribe(customers => {
      expect(customers.length).toBe(2);
    });

    const req = httpMock.expectOne(environment.apiUrl + '/customers');
    expect(req.request.method).toBe('GET');
    req.flush(mockCustomers);
  });
});
```

---

## 🎯 Summary

You now have a complete **Hexagonal Architecture** setup that:

1. ✅ Separates business logic (domain) from infrastructure
2. ✅ Uses ports (interfaces) and adapters (implementations)
3. ✅ Mirrors your backend architecture
4. ✅ Is testable, maintainable, and scalable
5. ✅ Uses Angular 19 standalone components

---

## 📚 Next Steps

- Implement remaining repositories (Product, Sale, Vehicle)
- Create feature modules for all entities
- Add shared UI components
- Implement state management (optional)
- Add unit and integration tests

---

## 🔗 Related Documentation

- [Architecture Overview](./ARCHITECTURE.md)
- [Backend API](../../docs/api/ENDPOINTS.md)


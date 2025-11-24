# Hexagonal Architecture - Backend & Frontend Alignment

Visual comparison showing how the Angular frontend mirrors the .NET backend architecture.

---

## 🎯 Architecture Overview

Both backend and frontend follow **Hexagonal Architecture** (Ports & Adapters) ensuring:
- Consistent design patterns across the stack
- Easy communication between layers
- Clear separation of concerns
- High testability and maintainability

---

## 🔄 Layer Comparison

### Backend (.NET) ↔️ Frontend (Angular)

```
┌─────────────────────────────────────────────────────────────────┐
│                     PRESENTATION LAYER                          │
├─────────────────────────────────────────────────────────────────┤
│  Backend: Controllers                                           │
│  - AuthController                                               │
│  - CustomersApiController                                       │
│  - ProductsController                                           │
│                                                                 │
│  Frontend: Feature Components                                   │
│  - LoginComponent                                               │
│  - CustomerListComponent                                        │
│  - ProductListComponent                                         │
└─────────────────────────────────────────────────────────────────┘
                              ↕️
┌─────────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                           │
├─────────────────────────────────────────────────────────────────┤
│  Backend: Services (Use Cases)                                  │
│  - CustomerService                                              │
│  - NotificationService                                          │
│  - AuthService                                                  │
│                                                                 │
│  Frontend: Facade Services                                      │
│  - CustomerFacadeService                                        │
│  - AuthService                                                  │
│  - NotificationService                                          │
└─────────────────────────────────────────────────────────────────┘
                              ↕️
┌─────────────────────────────────────────────────────────────────┐
│                        DOMAIN LAYER                             │
├─────────────────────────────────────────────────────────────────┤
│  Backend: Domain Entities & Interfaces                          │
│  - Customer (Entity)                                            │
│  - ICustomerRepository (Interface)                              │
│  - IEmailService (Interface)                                    │
│                                                                 │
│  Frontend: Domain Models & Ports                                │
│  - Customer (Model)                                             │
│  - CustomerRepository (Abstract Class)                          │
│  - StorageService (Abstract Class)                              │
└─────────────────────────────────────────────────────────────────┘
                              ↕️
┌─────────────────────────────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYER                         │
├─────────────────────────────────────────────────────────────────┤
│  Backend: Concrete Implementations                              │
│  - CustomerRepository (EF Core)                                 │
│  - GmailEmailService                                            │
│  - ApplicationDbContext                                         │
│                                                                 │
│  Frontend: HTTP Adapters                                        │
│  - CustomerHttpRepository                                       │
│  - LocalStorageService                                          │
│  - HTTP Interceptors                                            │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📊 Detailed Comparison

### 1. Domain Layer

| Concept | Backend (.NET) | Frontend (Angular) |
|---------|----------------|-------------------|
| **Location** | `Firmness.Domain/` | `src/app/domain/` |
| **Entities/Models** | `Customer.cs`, `Product.cs` | `customer.model.ts`, `product.model.ts` |
| **Interfaces** | `ICustomerRepository`, `IEmailService` | `CustomerRepository`, `StorageService` |
| **Enums** | `Role`, `SaleStatus` | `Role`, `SaleStatus` |
| **DTOs** | `CustomerDto` | Handled by mappers |
| **Dependencies** | None (pure C#) | None (pure TypeScript) |

**Example - Customer Entity:**

```csharp
// Backend: Firmness.Domain/Entities/Customer.cs
public class Customer
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string GetFullName() => $"{FirstName} {LastName}";
}
```

```typescript
// Frontend: src/app/domain/models/customer.model.ts
export class Customer {
  constructor(
    public id: string,
    public email: string,
    public firstName: string,
    public lastName: string
  ) {}
  
  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
}
```

---

### 2. Ports (Interfaces)

| Concept | Backend (.NET) | Frontend (Angular) |
|---------|----------------|-------------------|
| **Location** | `Firmness.Domain/Interfaces/` | `src/app/domain/ports/` |
| **Repository** | `ICustomerRepository` | `CustomerRepository` (abstract class) |
| **Services** | `IEmailService` | `StorageService` (abstract class) |
| **Purpose** | Define contracts | Define contracts |

**Example - Repository Interface:**

```csharp
// Backend: Firmness.Domain/Interfaces/IRepository.cs
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
```

```typescript
// Frontend: src/app/domain/ports/repositories/customer.repository.ts
export abstract class CustomerRepository {
  abstract getAll(): Observable<Customer[]>;
  abstract getById(id: string): Observable<Customer>;
  abstract create(customer: Customer): Observable<Customer>;
  abstract update(id: string, customer: Partial<Customer>): Observable<Customer>;
  abstract delete(id: string): Observable<void>;
}
```

---

### 3. Application Layer (Use Cases)

| Concept | Backend (.NET) | Frontend (Angular) |
|---------|----------------|-------------------|
| **Location** | `Firmness.Application/Services/` | `src/app/core/services/` |
| **Services** | `CustomerService` | `CustomerFacadeService` |
| **Purpose** | Business logic orchestration | Use case orchestration |
| **Dependencies** | Domain interfaces | Domain ports |

**Example - Application Service:**

```csharp
// Backend: Firmness.Application/Services/CustomerService.cs
public class CustomerService
{
    private readonly IRepository<Customer> _customerRepo;
    
    public CustomerService(IRepository<Customer> customerRepo)
    {
        _customerRepo = customerRepo;
    }
    
    public async Task<Customer> GetByIdAsync(Guid id)
    {
        return await _customerRepo.GetByIdAsync(id);
    }
}
```

```typescript
// Frontend: src/app/core/services/customer-facade.service.ts
@Injectable({ providedIn: 'root' })
export class CustomerFacadeService {
  constructor(private customerRepo: CustomerRepository) {}
  
  getCustomerById(id: string): Observable<Customer> {
    return this.customerRepo.getById(id);
  }
}
```

---

### 4. Infrastructure Layer (Adapters)

| Concept | Backend (.NET) | Frontend (Angular) |
|---------|----------------|-------------------|
| **Location** | `Firmness.Infrastructure/` | `src/app/infrastructure/` |
| **Repositories** | `CustomerRepository` (EF Core) | `CustomerHttpRepository` (HttpClient) |
| **External Services** | `GmailEmailService` | `LocalStorageService` |
| **Data Access** | Entity Framework | HttpClient |
| **Purpose** | Concrete implementations | HTTP adapters |

**Example - Repository Implementation:**

```csharp
// Backend: Firmness.Infrastructure/Repositories/CustomerRepository.cs
public class CustomerRepository : IRepository<Customer>
{
    private readonly ApplicationDbContext _context;
    
    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }
}
```

```typescript
// Frontend: src/app/infrastructure/http/customer-http.repository.ts
@Injectable({ providedIn: 'root' })
export class CustomerHttpRepository implements CustomerRepository {
  private apiUrl = `${environment.apiUrl}/customers`;
  
  constructor(private http: HttpClient) {}
  
  getAll(): Observable<Customer[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(dtos => dtos.map(dto => this.mapper.toDomain(dto)))
    );
  }
}
```

---

### 5. Presentation Layer

| Concept | Backend (.NET) | Frontend (Angular) |
|---------|----------------|-------------------|
| **Location** | `Firmness.Api/Controllers/` | `src/app/features/` |
| **Controllers/Components** | `CustomersApiController` | `CustomerListComponent` |
| **Routing** | ASP.NET Core Routing | Angular Router |
| **Purpose** | HTTP endpoints | User interface |

**Example - Presentation:**

```csharp
// Backend: Firmness.Api/Controllers/CustomersApiController.cs
[ApiController]
[Route("api/[controller]")]
public class CustomersApiController : ControllerBase
{
    private readonly CustomerService _customerService;
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }
}
```

```typescript
// Frontend: src/app/features/customers/pages/customer-list.component.ts
@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html'
})
export class CustomerListComponent implements OnInit {
  customers: Customer[] = [];
  
  constructor(private customerFacade: CustomerFacadeService) {}
  
  ngOnInit(): void {
    this.customerFacade.getAllCustomers().subscribe({
      next: (customers) => this.customers = customers
    });
  }
}
```

---

## 🔄 Data Flow Example: Get All Customers

### Backend Flow

```
1. HTTP GET /api/customers
   ↓
2. CustomersApiController.GetAll()
   ↓
3. CustomerService.GetAllAsync()
   ↓
4. IRepository<Customer>.GetAllAsync() [Interface]
   ↓
5. CustomerRepository.GetAllAsync() [EF Core Implementation]
   ↓
6. ApplicationDbContext → PostgreSQL
   ↓
7. Customer entities returned
   ↓
8. Mapped to DTOs
   ↓
9. JSON response sent
```

### Frontend Flow

```
1. CustomerListComponent.ngOnInit()
   ↓
2. CustomerFacadeService.getAllCustomers()
   ↓
3. CustomerRepository.getAll() [Abstract Class]
   ↓
4. CustomerHttpRepository.getAll() [HTTP Implementation]
   ↓
5. HttpClient.get() → Backend API
   ↓
6. Receive JSON response
   ↓
7. CustomerMapper.toDomain() converts DTO to model
   ↓
8. Observable<Customer[]> returned
   ↓
9. Component receives domain models
   ↓
10. Display in template
```

---

## 🎯 Dependency Injection Setup

### Backend DI Configuration

```csharp
// Program.cs
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddTransient<IEmailService, GmailEmailService>();
```

### Frontend DI Configuration

```typescript
// app.config.ts
export const appConfig: ApplicationConfig = {
  providers: [
    // Port → Adapter binding
    { provide: CustomerRepository, useClass: CustomerHttpRepository },
    { provide: StorageService, useClass: LocalStorageService },
    
    // Singletons
    CustomerFacadeService,
    AuthService
  ]
};
```

---

## 📦 Project Structure Comparison

### Backend Structure

```
src/
├── Firmness.Api/           # Presentation (REST API)
├── Firmness.Web/           # Presentation (MVC)
├── Firmness.Application/   # Application Layer
├── Firmness.Domain/        # Domain Layer
└── Firmness.Infrastructure/# Infrastructure Layer
```

### Frontend Structure

```
src/app/
├── features/       # Presentation (UI Components)
├── core/           # Application Layer (Facades)
├── domain/         # Domain Layer (Models & Ports)
├── infrastructure/ # Infrastructure Layer (HTTP)
├── shared/         # Shared UI Components
└── layout/         # Layout Components
```

---

## ✅ Benefits of Aligned Architecture

| Benefit | Description |
|---------|-------------|
| **Consistency** | Same patterns across frontend and backend |
| **Communication** | Teams speak the same architectural language |
| **Learning Curve** | Understanding one helps understand the other |
| **Testability** | Both layers highly testable with mocks |
| **Maintainability** | Clear separation of concerns everywhere |
| **Flexibility** | Easy to change implementations without breaking domain |

---

## 🔐 Authentication Flow (Full Stack)

### Backend

```
1. POST /api/auth/login
   ↓
2. AuthController.Login()
   ↓
3. Validate credentials with ASP.NET Identity
   ↓
4. Generate JWT token
   ↓
5. Return token + user info
```

### Frontend

```
1. LoginComponent submits form
   ↓
2. AuthService.login()
   ↓
3. AuthRepository.login() [Port]
   ↓
4. AuthHttpRepository.login() [Adapter]
   ↓
5. HTTP POST to backend
   ↓
6. Receive token
   ↓
7. StorageService.setItem('auth_token')
   ↓
8. authInterceptor adds token to subsequent requests
```

---

## 🧪 Testing Strategy

### Backend Tests

```csharp
// Mock repository for testing service
var mockRepo = new Mock<IRepository<Customer>>();
mockRepo.Setup(r => r.GetAllAsync())
    .ReturnsAsync(mockCustomers);

var service = new CustomerService(mockRepo.Object);
var result = await service.GetAllAsync();

Assert.Equal(2, result.Count());
```

### Frontend Tests

```typescript
// Mock repository for testing service
const mockRepo = jasmine.createSpyObj<CustomerRepository>(['getAll']);
mockRepo.getAll.and.returnValue(of(mockCustomers));

const service = new CustomerFacadeService(mockRepo);
service.getAllCustomers().subscribe(customers => {
  expect(customers.length).toBe(2);
});
```

---

## 📚 Documentation Alignment

| Topic | Backend Docs | Frontend Docs |
|-------|-------------|---------------|
| Architecture | [Backend Architecture](../../docs/development/ARCHITECTURE.md) | [Frontend Architecture](./ARCHITECTURE.md) |
| Implementation | [Migrations Guide](../../docs/development/MIGRATIONS.md) | [Implementation Guide](./IMPLEMENTATION_GUIDE.md) |
| API Reference | [API Endpoints](../../docs/api/ENDPOINTS.md) | [Frontend README](../README.md) |
| Project Structure | Backend README | [Project Structure](./STRUCTURE.md) |

---

## 🎓 Key Takeaways

1. ✅ **Both use Hexagonal Architecture** - Same design pattern
2. ✅ **Domain is pure** - No framework dependencies in either
3. ✅ **Ports define contracts** - Interfaces/abstract classes
4. ✅ **Adapters implement ports** - Concrete implementations
5. ✅ **Dependency injection** - Both use DI containers
6. ✅ **Clear layer separation** - Easy to test and maintain
7. ✅ **Consistent naming** - Similar class names and concepts

---

## 🔗 Related Documentation

- [Backend Architecture](../../docs/development/ARCHITECTURE.md)
- [Frontend Architecture](./ARCHITECTURE.md)
- [Frontend Implementation Guide](./IMPLEMENTATION_GUIDE.md)
- [Project Structure](./STRUCTURE.md)

---

**Architecture:** Hexagonal (Ports & Adapters)  
**Backend:** .NET 8  
**Frontend:** Angular 19  
**Database:** PostgreSQL


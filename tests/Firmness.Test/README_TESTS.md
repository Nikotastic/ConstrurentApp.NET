# Unit Tests - Firmness.Test

> [⬅️ Back to Main README](../../README.md) | [📚 Documentation Hub](../../docs/README.md)

## ✅ Current Status

```
Total Tests: 90
✅ Passing: 90 (100%)
❌ Failed: 0
⏱️ Duration: ~2 seconds
```

## 🧪 Test Categories

### **Services** (60 tests) - Business Logic

- **CustomerService** (10) - CRUD, validations, email notifications
- **ProductService** (11) - CRUD, stock management, validations
- **SaleService** (11) - Sales with stock validation
- **VehicleService** (3) - Vehicle management
- **VehicleRentalService** (7) - Rental operations with availability checks
- **CategoryService** (6) - Category CRUD operations
- **NotificationService** (4) - Email services (welcome, confirmations)
- **DashboardService** (2) - Metrics and aggregations

### **Controllers** (29 tests) - HTTP API Endpoints

- **ProductsController** (11) - Product API endpoints
- **SalesController** (18) - Sales API with role-based authorization

### **Integration** (1 test) - Database Operations

- **ProductRepository** - Real database operations with EF Core

## 🚀 Quick Start

```bash
# Run all tests
dotnet test

# Run only service tests (most important)
dotnet test --filter "FullyQualifiedName~Services"

# Run only integration tests
dotnet test --filter "FullyQualifiedName~Integration"

# Detailed output
dotnet test --logger "console;verbosity=detailed"
```

## 📁 Project Structure

```
Firmness.Test/
├── Services/              # Service layer tests (business logic)
├── Controllers/           # API controller tests
├── Integration/           # Database integration tests
├── Mocks/                 # Mock services (e.g., MockEmailService)
└── TestHelper.cs          # Test utilities
```

## 🛠️ Technologies

- **xUnit 2.9.3** - Testing framework
- **Moq** - Mocking library
- **EntityFrameworkCore.InMemory** - In-memory database for integration tests
- **AspNetCore.Mvc.Testing** - Controller testing utilities

## 📝 Test Naming Convention

```
MethodName_Scenario_ExpectedBehavior
```

Examples:

- `GetByIdAsync_ValidId_ReturnsCustomer`
- `CreateSaleAsync_InsufficientStock_ThrowsException`
- `AddAsync_NullProduct_ReturnsFailure`

## 🎯 AAA Pattern

All tests follow the **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public async Task GetByIdAsync_ValidId_ReturnsProduct()
{
    // Arrange - Setup test scenario
    var mockRepo = new Mock<IProductRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);
    var service = new ProductService(mockRepo.Object);

    // Act - Execute the operation
    var result = await service.GetByIdAsync(id);

    // Assert - Verify the result
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
}
```

## 📊 What's Tested?

✅ **Business Logic** - Services contain core application logic  
✅ **Validations** - Null checks, required fields, business rules  
✅ **Error Handling** - Proper error codes and exception management  
✅ **Database Operations** - Integration tests with EF Core  
✅ **API Endpoints** - HTTP status codes and responses  
✅ **Authorization** - Role-based access control

## 📖 Documentation

- 📗 **[Architecture Guide](/docs/development/ARCHITECTURE.md)** - System architecture
- 📙 **[API Documentation](/docs/api/ENDPOINTS.md)** - REST API reference

---

<div align="center">
  <a href="../../README.md">⬅️ Back to Main README</a> | 
  <a href="../../docs/README.md">📚 Documentation Hub</a>
</div>

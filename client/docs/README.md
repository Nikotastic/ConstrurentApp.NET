# Angular Frontend - Documentation

Complete documentation for the Firmness Angular frontend application.

---

## 📚 Documentation Index

### 🏗️ Architecture & Design

| Document | Description |
|----------|-------------|
| **[Architecture Overview](./ARCHITECTURE.md)** | Complete explanation of Hexagonal Architecture (Ports & Adapters) |
| **[Project Structure](./STRUCTURE.md)** | Visual guide of the folder structure and file organization |
| **[Hexagonal Alignment](./HEXAGONAL_ALIGNMENT.md)** | Backend ↔️ Frontend architecture comparison |

### 🚀 Implementation

| Document | Description |
|----------|-------------|
| **[Implementation Guide](./IMPLEMENTATION_GUIDE.md)** | Step-by-step guide to implement the architecture |

### 🔗 Backend Integration

| Document | Description |
|----------|-------------|
| **[API Endpoints](../../docs/api/ENDPOINTS.md)** | Backend REST API reference |
| **[Authentication](../../docs/api/AUTHENTICATION.md)** | JWT authentication guide |
| **[Backend Architecture](../../docs/development/ARCHITECTURE.md)** | Backend hexagonal architecture |

---

## 🎯 Quick Navigation

### For New Developers

Start here to understand the project:

1. 📖 [Architecture Overview](./ARCHITECTURE.md) - Understand hexagonal architecture
2. 📁 [Project Structure](./STRUCTURE.md) - Learn the folder organization
3. 🚀 [Implementation Guide](./IMPLEMENTATION_GUIDE.md) - Start implementing

### For Existing Developers

Quick references:

- 🔍 [Project Structure](./STRUCTURE.md) - Find where to add code
- 🔌 [API Endpoints](../../docs/api/ENDPOINTS.md) - Backend API reference
- 🏗️ [Architecture Overview](./ARCHITECTURE.md) - Review design patterns

### For Backend Developers

Integrate with frontend:

- 🔐 [Authentication](../../docs/api/AUTHENTICATION.md) - JWT flow
- 🔌 [API Endpoints](../../docs/api/ENDPOINTS.md) - Available endpoints
- 🏗️ [Backend Architecture](../../docs/development/ARCHITECTURE.md) - Backend structure

---

## 🏗️ Architecture Summary

The frontend uses **Hexagonal Architecture** (Ports & Adapters) with the following layers:

```
src/app/
├── domain/         # Business logic (pure TypeScript)
├── infrastructure/ # External adapters (HTTP, Storage)
├── core/           # Application services & guards
├── features/       # Feature modules (UI)
├── shared/         # Reusable UI components
└── layout/         # Layout structure
```

### Key Principles

1. **Domain Independence** - Business logic has no dependencies
2. **Dependency Inversion** - Infrastructure implements domain interfaces
3. **Testability** - Easy to mock external dependencies
4. **Backend Alignment** - Mirrors backend hexagonal architecture

---

## 📖 Documentation Details

### 1. Architecture Overview
**File:** [ARCHITECTURE.md](./ARCHITECTURE.md)

**Contents:**
- Hexagonal architecture explanation
- Layer responsibilities
- Dependency injection setup
- Port and adapter pattern
- Benefits and trade-offs
- Complete code examples

**Best for:** Understanding the overall design

---

### 2. Project Structure
**File:** [STRUCTURE.md](./STRUCTURE.md)

**Contents:**
- Complete file tree
- Folder organization
- Naming conventions
- Layer responsibilities
- Data flow examples
- Import aliases

**Best for:** Finding where to add new code

---

### 3. Implementation Guide
**File:** [IMPLEMENTATION_GUIDE.md](./IMPLEMENTATION_GUIDE.md)

**Contents:**
- Step-by-step setup instructions
- Domain model creation
- Repository implementation
- Service facade creation
- Component development
- Testing examples

**Best for:** Implementing features

---

## 🔄 Typical Workflows

### Adding a New Feature

1. **Create domain models** (`domain/models/`)
   ```typescript
   export class NewEntity { ... }
   ```

2. **Define repository port** (`domain/ports/repositories/`)
   ```typescript
   export abstract class NewEntityRepository { ... }
   ```

3. **Implement HTTP adapter** (`infrastructure/http/`)
   ```typescript
   export class NewEntityHttpRepository implements NewEntityRepository { ... }
   ```

4. **Create facade service** (`core/services/`)
   ```typescript
   export class NewEntityFacadeService { ... }
   ```

5. **Build feature components** (`features/new-entity/`)
   ```typescript
   export class NewEntityListComponent { ... }
   ```

6. **Configure DI** (`app.config.ts`)
   ```typescript
   { provide: NewEntityRepository, useClass: NewEntityHttpRepository }
   ```

### Integrating with Backend API

1. Check [API Endpoints](../../docs/api/ENDPOINTS.md) documentation
2. Create domain model matching the API response
3. Create mapper in `infrastructure/mappers/`
4. Implement HTTP repository in `infrastructure/http/`
5. Test with backend running

### Testing a Feature

1. Mock the repository port
2. Test domain logic independently
3. Test HTTP adapter with `HttpTestingController`
4. Test components with mocked services

---

## 🧩 Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **Angular** | 19.2+ | Frontend framework |
| **TypeScript** | 5.7+ | Programming language |
| **RxJS** | 7.8+ | Reactive programming |
| **Angular Router** | 19.2+ | Routing & navigation |
| **HttpClient** | 19.2+ | HTTP communication |

---

## 📦 Dependencies

### Core Dependencies
- `@angular/core` - Angular framework
- `@angular/common` - Common Angular utilities
- `@angular/router` - Routing
- `@angular/forms` - Form handling
- `rxjs` - Reactive programming

### No Additional Libraries Required
The architecture is framework-agnostic and doesn't require additional state management libraries (though you can add NgRx Signals if needed).

---

## 🎯 Design Patterns Used

| Pattern | Location | Purpose |
|---------|----------|---------|
| **Hexagonal Architecture** | Overall | Separation of concerns |
| **Repository Pattern** | `domain/ports/` | Data access abstraction |
| **Facade Pattern** | `core/services/` | Simplified API for features |
| **Dependency Injection** | `app.config.ts` | Loose coupling |
| **Interceptor Pattern** | `infrastructure/http/interceptors/` | Cross-cutting concerns |
| **Mapper Pattern** | `infrastructure/mappers/` | DTO transformation |
| **Guard Pattern** | `core/guards/` | Route protection |

---

## 🔗 External Resources

### Angular Documentation
- [Angular Official Docs](https://angular.dev/)
- [Angular Router](https://angular.dev/guide/routing)
- [Angular Forms](https://angular.dev/guide/forms)
- [RxJS Documentation](https://rxjs.dev/)

### Architecture Resources
- [Hexagonal Architecture (Ports & Adapters)](https://alistair.cockburn.us/hexagonal-architecture/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## 🆘 Troubleshooting

### Common Issues

**Issue:** Path aliases not working (`@domain/*`, etc.)
- **Solution:** Check `tsconfig.json` has correct paths configuration

**Issue:** HTTP requests failing
- **Solution:** Verify backend is running and `environment.ts` has correct API URL

**Issue:** Authentication not persisting
- **Solution:** Check `LocalStorageService` is properly injected as `StorageService`

**Issue:** Dependency injection errors
- **Solution:** Verify `app.config.ts` has correct port → adapter bindings

---

## 📝 Contributing Guidelines

When contributing to the frontend:

1. ✅ Follow the hexagonal architecture layers
2. ✅ Keep domain layer pure (no Angular dependencies)
3. ✅ Use dependency injection for repositories
4. ✅ Add tests for new features
5. ✅ Update documentation when adding features
6. ✅ Use TypeScript strict mode
7. ✅ Follow Angular style guide

---

## 🔄 Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | Nov 2025 | Initial hexagonal architecture documentation |

---

## 📞 Support

For questions or issues:

1. Check this documentation first
2. Review [Backend Architecture](../../docs/development/ARCHITECTURE.md)
3. Check [API Documentation](../../docs/api/ENDPOINTS.md)
4. Review [Implementation Guide](./IMPLEMENTATION_GUIDE.md)

---

## 📄 License

See [LICENSE](../../LICENSE) file in root directory.

---

**Architecture:** Hexagonal (Ports & Adapters)  
**Framework:** Angular 19 (Standalone Components)  
**Backend:** .NET 8 REST API  
**Last Updated:** November 2025


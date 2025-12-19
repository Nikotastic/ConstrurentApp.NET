# 🏗️ Frontend Architecture - Clean Architecture

Este proyecto sigue los principios de **Clean Architecture** adaptados para Angular.

## 📁 Estructura de Carpetas

```
src/app/
├── core/                          # Configuración central de la aplicación
│   ├── config/                    # Constantes y configuración
│   │   └── app.config.ts         # API endpoints, rutas, roles, validaciones
│   └── interceptors/              # HTTP Interceptors
│       └── auth.interceptor.ts   # Interceptor para agregar JWT a requests
│
├── domain/                        # Capa de Dominio (Entidades y Reglas de Negocio)
│   ├── entities/                  # Entidades del dominio
│   ├── value-objects/             # Value Objects
│   └── repositories/              # Interfaces de repositorios (Ports)
│
├── application/                   # Capa de Aplicación (Casos de Uso)
│   ├── use-cases/                 # Casos de uso específicos
│   ├── services/                  # Servicios de aplicación
│   ├── guards/                    # Guards de autenticación y autorización
│   └── ports/                     # Interfaces de servicios
│
├── infrastructure/                # Capa de Infraestructura (Implementaciones)
│   ├── repositories/              # Implementaciones de repositorios (Adapters)
│   ├── services/                  # Implementaciones de servicios
│   └── http/                      # Configuración HTTP
│       └── interceptors/          # Interceptores de errores, etc.
│
└── presentation/                  # Capa de Presentación (UI)
    ├── shared/                    # Componentes, pipes y directivas reutilizables
    │   ├── components/            # Componentes compartidos
    │   │   ├── loading-spinner/
    │   │   └── alert/
    │   ├── pipes/                 # Pipes personalizados
    │   │   └── currency.pipe.ts
    │   └── directives/            # Directivas personalizadas
    ├── layout/                    # Layouts de la aplicación
    │   └── main-layout/
    └── features/                  # Features/Módulos de la aplicación
        ├── auth/
        ├── dashboard/
        ├── products/
        └── cart/
```

## 🎯 Principios de Clean Architecture

### 1. **Independencia de Frameworks**

- El dominio no depende de Angular
- Las entidades son POJOs (Plain Old JavaScript Objects)

### 2. **Testabilidad**

- Cada capa puede ser testeada independientemente
- Los casos de uso no dependen de la UI

### 3. **Independencia de la UI**

- La lógica de negocio está separada de la presentación
- Puedes cambiar la UI sin afectar la lógica

### 4. **Independencia de la Base de Datos**

- Los repositorios son interfaces (ports)
- Las implementaciones (adapters) pueden cambiar

### 5. **Regla de Dependencia**

```
Presentation → Application → Domain
Infrastructure → Application → Domain
```

## 📦 Capas Explicadas

### Core

Configuración central y utilidades transversales:

- **config/**: Constantes de la aplicación (API, rutas, roles)
- **interceptors/**: Interceptores HTTP (auth, logging, etc.)

### Domain

El corazón de la aplicación, sin dependencias externas:

- **entities/**: Modelos de negocio (Product, Customer, Sale)
- **value-objects/**: Objetos de valor inmutables
- **repositories/**: Interfaces que definen cómo acceder a datos

### Application

Orquesta los casos de uso:

- **use-cases/**: Lógica de negocio específica (LoginUseCase, CreateSaleUseCase)
- **services/**: Servicios de aplicación (AuthService, CartService)
- **guards/**: Protección de rutas (authGuard, roleGuard)
- **ports/**: Interfaces de servicios externos

### Infrastructure

Implementaciones concretas:

- **repositories/**: Implementaciones HTTP de repositorios
- **services/**: Servicios de infraestructura (LocalStorage, HTTP)
- **http/**: Configuración y interceptores HTTP

### Presentation

Todo lo relacionado con la UI:

- **shared/**: Componentes reutilizables en toda la app
- **layout/**: Layouts (header, sidebar, footer)
- **features/**: Módulos funcionales de la aplicación

## 🔄 Flujo de Datos

```
User Action (UI)
    ↓
Component (Presentation)
    ↓
Service (Application)
    ↓
Use Case (Application)
    ↓
Repository Interface (Domain)
    ↓
Repository Implementation (Infrastructure)
    ↓
HTTP Client (Infrastructure)
    ↓
API
```

## 🛠️ Uso de Shared Components

### Loading Spinner

```typescript
import { LoadingSpinnerComponent } from '@presentation/shared';

// En tu template
<app-loading-spinner
  [size]="50"
  [color]="'#007bff'"
  [message]="'Loading...'"
  [fullscreen]="true">
</app-loading-spinner>
```

### Alert

```typescript
import { AlertComponent } from '@presentation/shared';

// En tu template
<app-alert
  [type]="'success'"
  [message]="'Operation successful!'"
  [dismissible]="true"
  (dismissed)="onAlertDismissed()">
</app-alert>
```

### Currency Pipe

```typescript
import { CurrencyPipe } from '@presentation/shared';

// En tu template
{{ product.price | appCurrency }}
{{ product.price | appCurrency:'EUR' }}
```

## 📝 Convenciones de Código

### Naming

- **Components**: `[feature]-[name].component.ts`
- **Services**: `[name].service.ts`
- **Guards**: `[name].guard.ts`
- **Use Cases**: `[action]-[entity].use-case.ts`

### Imports

Usa path aliases para imports limpios:

```typescript
import { Product } from "@domain/entities/product.entity";
import { AuthService } from "@application/services/auth.service";
import { LoadingSpinnerComponent } from "@presentation/shared";
```

## 🚀 Mejores Prácticas

1. **Mantén las capas separadas**: No mezcles lógica de presentación con lógica de negocio
2. **Usa interfaces**: Define contratos claros entre capas
3. **Componentes pequeños**: Cada componente debe tener una sola responsabilidad
4. **Reutiliza**: Usa shared components en lugar de duplicar código
5. **Testea**: Escribe tests para cada capa independientemente

## 📚 Recursos

- [Clean Architecture (Robert C. Martin)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Angular Architecture Best Practices](https://angular.io/guide/architecture)
- [Hexagonal Architecture](https://alistair.cockburn.us/hexagonal-architecture/)

import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';

// Domain Interfaces (Ports)
import { CUSTOMER_REPOSITORY_TOKEN } from './domain/repositories/customer.repository.interface';
import { PRODUCT_REPOSITORY_TOKEN } from './domain/repositories/product.repository.interface';
import { AUTH_REPOSITORY_TOKEN } from './domain/repositories/auth.repository.interface';

// Application Ports
import { STORAGE_SERVICE_TOKEN } from './application/ports/storage.service.interface';

// Infrastructure Implementations (Adapters)
import { CustomerHttpRepository } from './infrastructure/repositories/customer-http.repository';
import { AuthHttpRepository } from './infrastructure/repositories/auth-http.repository';
import { ProductHttpRepository } from './infrastructure/repositories/product-http.repository';
import { LocalStorageService } from './infrastructure/services/local-storage.service';

// HTTP Interceptors
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './infrastructure/http/interceptors/error.interceptor';

/**
 * Application Configuration - Clean Architecture
 *
 * Dependency Injection Setup:
 * - Binds domain interfaces (ports) to infrastructure implementations (adapters)
 * - Follows the Dependency Inversion Principle
 * - Infrastructure depends on Domain, not vice versa
 */
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimations(), // Enable animations
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),

    // CLEAN ARCHITECTURE: Port → Adapter Bindings
    // Domain Layer Ports → Infrastructure Layer Implementations

    // Repositories (Data Access)
    { provide: CUSTOMER_REPOSITORY_TOKEN, useClass: CustomerHttpRepository },
    { provide: AUTH_REPOSITORY_TOKEN, useClass: AuthHttpRepository },
    { provide: PRODUCT_REPOSITORY_TOKEN, useClass: ProductHttpRepository },

    // Application Services
    { provide: STORAGE_SERVICE_TOKEN, useClass: LocalStorageService },
  ],
};

import { Routes } from '@angular/router';
import { authGuard } from './application/guards/auth.guard';
import { roleGuard } from './application/guards/role.guard';
import { noAuthGuard } from './application/guards/no-auth.guard';

/**
 * Application Routes - Client View
 * Lazy-loaded feature modules
 */
export const routes: Routes = [
  // Public Routes
  {
    path: '',
    loadComponent: () =>
      import('./presentation/features/home/home.component').then(
        (m) => m.HomeComponent
      ),
  },
  {
    path: 'login',
    canActivate: [noAuthGuard], // Prevent authenticated users from accessing login
    loadComponent: () =>
      import('./presentation/features/auth/pages/login/login.component').then(
        (m) => m.LoginComponent
      ),
  },
  {
    path: 'register',
    canActivate: [noAuthGuard], // Prevent authenticated users from accessing register
    loadComponent: () =>
      import(
        './presentation/features/auth/pages/register/register.component'
      ).then((m) => m.RegisterComponent),
  },

  // Protected Routes (with Main Layout) - Client Role Only
  {
    path: '',
    canActivate: [authGuard, roleGuard],
    data: { expectedRole: 'Client' }, // Only clients can access
    loadComponent: () =>
      import('./presentation/layout/main-layout/main-layout.component').then(
        (m) => m.MainLayoutComponent
      ),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./presentation/features/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent
          ),
      },
      {
        path: 'products',
        loadComponent: () =>
          import(
            './presentation/features/products/product-list/product-list.component'
          ).then((m) => m.ProductListComponent),
      },
      {
        path: 'cart',
        loadComponent: () =>
          import('./presentation/features/cart/cart.component').then(
            (m) => m.CartComponent
          ),
      },
      {
        path: 'orders',
        loadComponent: () =>
          import('./presentation/features/my-orders/my-orders.component').then(
            (m) => m.MyOrdersComponent
          ),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./presentation/features/profile/profile.component').then(
            (m) => m.ProfileComponent
          ),
      },
    ],
  },

  // Wildcard
  {
    path: '**',
    redirectTo: '',
    pathMatch: 'full',
  },
];

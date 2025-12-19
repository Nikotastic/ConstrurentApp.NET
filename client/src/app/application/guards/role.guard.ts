import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const expectedRole = route.data['expectedRole'];
  const userRole = authService.getUserRole();

  if (!authService.isAuthenticated()) {
    router.navigate(['/auth/login']);
    return false;
  }

  if (expectedRole && userRole !== expectedRole) {
    // If the user has a different role, redirect according to the role
    if (userRole === 'Admin') {
      // Admin cannot access the client, redirect to the admin panel
      window.location.href = 'http://localhost:5001';
      return false;
    }

    // Client attempting to access admin routes
    router.navigate(['/dashboard']);
    return false;
  }

  return true;
};

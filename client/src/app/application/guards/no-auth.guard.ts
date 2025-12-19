import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';


// Guard to prevent authenticated users from accessing login/register pages

export const noAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    // User is already logged in, redirect to dashboard
    router.navigate(['/dashboard']);
    return false;
  }

  return true;
};

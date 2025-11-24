import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokenService } from '../services/token.service';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // Check if user is authenticated
  if (tokenService.isTokenValid()) {
    // Check for required roles if specified in route data
    const requiredRoles = route.data?.['roles'] as Array<string>;

    if (requiredRoles && requiredRoles.length > 0) {
      const userRoles = tokenService.getUserRoles();
      const hasRole = requiredRoles.some((role) => userRoles.includes(role));

      if (!hasRole) {
        // User doesn't have permission
        console.warn('User does not have required role');
        // Redirect to dashboard or home if they don't have permission
        router.navigate(['/dashboard']);
        return false;
      }
    }

    return true;
  }

  // Not authenticated, redirect to login with return url
  console.log('User not authenticated, redirecting to login');
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};

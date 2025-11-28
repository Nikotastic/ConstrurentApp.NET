import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const authService = inject(AuthService);
  const router = inject(Router);

  // Check if user is authenticated
  if (!tokenService.isTokenValid()) {
    // Not authenticated, redirect to login with return url
    console.log('User not authenticated, redirecting to login');
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  // Verify user still exists in the backend
  // This will trigger the interceptor if the user has been deleted
  return authService.currentUser$.pipe(
    map((user) => {
      if (!user) {
        // Try to get current user from backend
        // If user is deleted, the interceptor will handle the redirect
        return true; // Allow navigation, interceptor will handle if user is deleted
      }

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
    }),
    catchError(() => {
      // If there's an error, the interceptor should have already handled it
      return of(false);
    })
  );
};

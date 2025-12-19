import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TokenService } from '../services/token.service';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // Get token from service
  const token = tokenService.getToken();

  // Clone request and add Authorization header if token exists
  let authReq = req;
  if (token && tokenService.isTokenValid()) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  // Handle response and errors
  return next(authReq).pipe(
    catchError((error) => {
      // If 401 Unauthorized, token is invalid or expired
      if (error.status === 401) {
        console.warn('Unauthorized request - redirecting to login');
        tokenService.removeToken();
        router.navigate(['/login'], {
          queryParams: { returnUrl: router.url },
        });
        return throwError(() => error);
      }

      // If 403 Forbidden or 404 Not Found on customers endpoints, user might have been deleted
      // This handles /customers/me, /customers/{id}, etc.
      if (
        (error.status === 403 || error.status === 404) &&
        req.url.includes('/customers')
      ) {
        console.warn('User account not found or deleted - logging out');
        tokenService.removeToken();

        // Navigate to home with account deleted message
        router.navigate(['/'], {
          queryParams: { message: 'account_deleted' },
        });

        // Don't propagate the error further to prevent component error handlers from showing messages
        return throwError(() => new Error('Account deleted'));
      }

      return throwError(() => error);
    })
  );
};

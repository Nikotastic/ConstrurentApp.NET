import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { STORAGE_SERVICE_TOKEN } from '../../../application/ports/storage.service.interface';

/**
 * Auth Interceptor - Infrastructure Layer
 * Adds JWT token to all HTTP requests
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const storage = inject(STORAGE_SERVICE_TOKEN);
  const token = storage.get('auth_token');

  if (token && !req.url.includes('/auth/')) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  return next(req);
};

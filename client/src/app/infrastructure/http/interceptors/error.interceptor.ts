import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

/**
 * Error Interceptor - Infrastructure Layer
 * Handles HTTP errors globally
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Log the error for debugging
      console.error('HTTP Error:', {
        status: error.status,
        message: error.error?.message || error.message,
        url: error.url,
        error: error.error,
      });

      // Return the original HttpErrorResponse so components can access status, error.message, etc.
      return throwError(() => error);
    })
  );
};

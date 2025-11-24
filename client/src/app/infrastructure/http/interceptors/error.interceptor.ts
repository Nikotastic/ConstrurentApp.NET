import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

/**
 * Error Interceptor - Infrastructure Layer
 * Handles HTTP errors globally
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unexpected error occurred';

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Error: ${error.error.message}`;
      } else {
        // Server-side error
        errorMessage = error.error?.message || error.message || `Error Code: ${error.status}`;
      }

      console.error('HTTP Error:', {
        status: error.status,
        message: errorMessage,
        url: error.url
      });

      return throwError(() => new Error(errorMessage));
    })
  );
};


import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  IAuthRepository,
  AUTH_REPOSITORY_TOKEN,
} from '@domain/repositories/auth.repository.interface';
import { LoginCredentials } from '@domain/value-objects/login-credentials.vo';

/**
 * Use Case: Login
 * Handles user authentication
 */
@Injectable({
  providedIn: 'root',
})
export class LoginUseCase {
  private readonly authRepository = inject(AUTH_REPOSITORY_TOKEN);

  /**
   * Execute login use case
   * @param credentials Login credentials
   * @returns Observable with authentication token
   */
  execute(
    credentials: LoginCredentials
  ): Observable<{ token: string; expiresInSeconds: number }> {
    // Business logic: validate credentials format
    // The LoginCredentials value object already validates the format

    return this.authRepository.login(credentials);
  }
}

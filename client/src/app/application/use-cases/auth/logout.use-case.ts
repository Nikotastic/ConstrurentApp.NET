import { inject, Injectable } from '@angular/core';
import {
  IAuthRepository,
  AUTH_REPOSITORY_TOKEN,
} from '@domain/repositories/auth.repository.interface';

/**
 * Use Case: Logout
 * Handles user logout
 */
@Injectable({
  providedIn: 'root',
})
export class LogoutUseCase {
  private readonly authRepository = inject(AUTH_REPOSITORY_TOKEN);

  /**
   * Execute logout use case
   */
  execute(): void {
    this.authRepository.logout();
  }
}

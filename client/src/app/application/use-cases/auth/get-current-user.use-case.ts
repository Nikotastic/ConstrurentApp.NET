import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  IAuthRepository,
  AUTH_REPOSITORY_TOKEN,
} from '@domain/repositories/auth.repository.interface';
import { AuthUser } from '@domain/entities/auth-user.entity';

/**
 * Use Case: Get Current User
 * Retrieves the currently authenticated user
 */
@Injectable({
  providedIn: 'root',
})
export class GetCurrentUserUseCase {
  private readonly authRepository = inject(AUTH_REPOSITORY_TOKEN);

  /**
   * Execute get current user use case
   * @returns Observable with current user or null if not authenticated
   */
  execute(): Observable<AuthUser | null> {
    return this.authRepository.getCurrentUser();
  }
}

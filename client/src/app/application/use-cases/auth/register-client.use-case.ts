import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  IAuthRepository,
  AUTH_REPOSITORY_TOKEN,
} from '@domain/repositories/auth.repository.interface';
import { RegisterData } from '@domain/value-objects/register-data.vo';

/**
 * Use Case: Register Client
 * Handles new client registration
 */
@Injectable({
  providedIn: 'root',
})
export class RegisterClientUseCase {
  private readonly authRepository = inject(AUTH_REPOSITORY_TOKEN);

  /**
   * Execute register client use case
   * @param data Registration data
   * @returns Observable with authentication token
   */
  execute(
    data: RegisterData
  ): Observable<{ token: string; expiresInSeconds: number }> {
    // Business logic: validate registration data
    // The RegisterData value object already validates all fields

    return this.authRepository.registerClient(data);
  }
}

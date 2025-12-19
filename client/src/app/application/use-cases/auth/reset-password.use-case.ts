import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AUTH_REPOSITORY_TOKEN } from '@domain/repositories/auth.repository.interface';

@Injectable({
  providedIn: 'root',
})
export class ResetPasswordUseCase {
  private readonly authRepository = inject(AUTH_REPOSITORY_TOKEN);

  execute(
    userId: string,
    code: string,
    newPassword: string
  ): Observable<{ message: string }> {
    return this.authRepository.resetPassword(userId, code, newPassword);
  }
}


import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AUTH_REPOSITORY_TOKEN } from '@domain/repositories/auth.repository.interface';

@Injectable({
  providedIn: 'root',
})
export class ForgotPasswordUseCase {
  private readonly authRepository = inject(AUTH_REPOSITORY_TOKEN);

  execute(email: string): Observable<{ message: string }> {
    return this.authRepository.forgotPassword(email);
  }
}

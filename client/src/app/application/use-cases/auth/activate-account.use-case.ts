import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AUTH_REPOSITORY_TOKEN } from '@domain/repositories/auth.repository.interface';

@Injectable({
  providedIn: 'root',
})
export class ActivateAccountUseCase {
  private readonly authRepository = inject(AUTH_REPOSITORY_TOKEN);

  execute(userId: string, code: string, password: string): Observable<void> {
    return this.authRepository.activateAccount(userId, code, password);
  }
}

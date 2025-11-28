import { Injectable, inject } from '@angular/core';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { LoginUseCase } from '../use-cases/auth/login.use-case';
import { RegisterClientUseCase } from '../use-cases/auth/register-client.use-case';
import { GetCurrentUserUseCase } from '../use-cases/auth/get-current-user.use-case';
import { LogoutUseCase } from '../use-cases/auth/logout.use-case';
import { ActivateAccountUseCase } from '../use-cases/auth/activate-account.use-case';
import { ForgotPasswordUseCase } from '../use-cases/auth/forgot-password.use-case';
import { ResetPasswordUseCase } from '../use-cases/auth/reset-password.use-case';
import { LoginCredentials } from '@domain/value-objects/login-credentials.vo';
import { RegisterData } from '@domain/value-objects/register-data.vo';
import { AuthUser } from '@domain/entities/auth-user.entity';
import { TokenService } from './token.service';
import { CartService } from './cart.service';

// Application Service: AuthService
// Orchestrates authentication use cases and manages auth state

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly loginUseCase = inject(LoginUseCase);
  private readonly registerClientUseCase = inject(RegisterClientUseCase);
  private readonly getCurrentUserUseCase = inject(GetCurrentUserUseCase);
  private readonly logoutUseCase = inject(LogoutUseCase);
  private readonly tokenService = inject(TokenService);
  private readonly cartService = inject(CartService);
  private readonly activateAccountUseCase = inject(ActivateAccountUseCase);
  private readonly forgotPasswordUseCase = inject(ForgotPasswordUseCase);
  private readonly resetPasswordUseCase = inject(ResetPasswordUseCase);

  // Auth state management
  private currentUserSubject = new BehaviorSubject<AuthUser | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    // Initialize current user from storage on service creation
    this.initializeCurrentUser();
  }

  /**
   * Initialize current user from stored token
   */
  private initializeCurrentUser(): void {
    if (this.tokenService.isTokenValid()) {
      const username = this.tokenService.getUsername();
      this.cartService.setUser(username); // Set cart user context

      this.getCurrentUserUseCase.execute().subscribe({
        next: (user) => this.currentUserSubject.next(user),
        error: () => {
          this.tokenService.removeToken();
          this.currentUserSubject.next(null);
          this.cartService.setUser(null); // Reset to guest cart
        },
      });
    } else {
      this.currentUserSubject.next(null);
      this.cartService.setUser(null); // Reset to guest cart
    }
  }

  /**
   * Login with credentials
   */
  login(
    email: string,
    password: string
  ): Observable<{ token: string; expiresInSeconds: number }> {
    const credentials = new LoginCredentials(email, password);

    return this.loginUseCase.execute(credentials).pipe(
      tap((response) => {
        // Store token using TokenService
        this.tokenService.saveToken(response.token);

        // Update current user
        this.initializeCurrentUser();
      })
    );
  }

  /**
   * Register new client
   */
  registerClient(data: {
    username: string;
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    document?: string;
    phone?: string;
    address?: string;
  }): Observable<{ token: string; expiresInSeconds: number }> {
    const registerData = new RegisterData(
      data.username,
      data.email,
      data.password,
      data.firstName,
      data.lastName,
      data.document,
      data.phone,
      data.address
    );

    return this.registerClientUseCase.execute(registerData).pipe(
      tap((response) => {
        // Store token using TokenService
        this.tokenService.saveToken(response.token);

        // Update current user
        this.initializeCurrentUser();
      })
    );
  }

  /**
   * Logout current user
   */
  logout(): void {
    this.logoutUseCase.execute();
    this.tokenService.removeToken();
    this.currentUserSubject.next(null);
    this.cartService.setUser(null); // Switch back to guest cart
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return this.tokenService.isTokenValid();
  }

  /**
   * Get current user (synchronous)
   */
  getCurrentUser(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  /**
   * Check if current user has a specific role
   */
  hasRole(role: string): boolean {
    return this.tokenService.hasRole(role);
  }

  /**
   * Get current user role from token
   */
  getUserRole(): string | null {
    return this.tokenService.getUserRole();
  }

  /**
   * Activate account
   */
  activateAccount(
    userId: string,
    code: string,
    password: string
  ): Observable<void> {
    return this.activateAccountUseCase.execute(userId, code, password);
  }

  /**
   * Request password reset
   */
  forgotPassword(email: string): Observable<{ message: string }> {
    return this.forgotPasswordUseCase.execute(email);
  }

  /**
   * Reset password with token
   */
  resetPassword(
    userId: string,
    code: string,
    newPassword: string
  ): Observable<{ message: string }> {
    return this.resetPasswordUseCase.execute(userId, code, newPassword);
  }
}

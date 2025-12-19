import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthUser } from '../entities/auth-user.entity';
import { LoginCredentials } from '../value-objects/login-credentials.vo';
import { RegisterData } from '../value-objects/register-data.vo';

/**
 * Port: IAuthRepository
 * Defines the contract for authentication operations
 * Infrastructure layer will implement this interface
 */
export interface IAuthRepository {
  /**
   * Authenticate user with credentials
   * @returns Observable with token and user data
   */
  login(
    credentials: LoginCredentials
  ): Observable<{ token: string; expiresInSeconds: number }>;

  /**
   * Register a new client user
   * @returns Observable with token and user data
   */
  registerClient(
    data: RegisterData
  ): Observable<{ token: string; expiresInSeconds: number }>;

  /**
   * Get current authenticated user from token
   * @returns Observable with user data or null if not authenticated
   */
  getCurrentUser(): Observable<AuthUser | null>;

  /**
   * Logout current user
   */
  logout(): void;

  /**
   * Activate account with token and new password
   */
  activateAccount(
    userId: string,
    code: string,
    password: string
  ): Observable<void>;

  /**
   * Request password reset for email
   */
  forgotPassword(email: string): Observable<{ message: string }>;

  /**
   * Reset password with token
   */
  resetPassword(
    userId: string,
    code: string,
    newPassword: string
  ): Observable<{ message: string }>;
}

/**
 * Injection Token for IAuthRepository
 * Used for Dependency Injection in Angular
 */
export const AUTH_REPOSITORY_TOKEN = new InjectionToken<IAuthRepository>(
  'IAuthRepository'
);

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, map, catchError } from 'rxjs';
import { IAuthRepository } from '@domain/repositories/auth.repository.interface';
import { AuthUser } from '@domain/entities/auth-user.entity';
import { LoginCredentials } from '@domain/value-objects/login-credentials.vo';
import { RegisterData } from '@domain/value-objects/register-data.vo';
import { environment } from '@env/environment';
import {
  IStorageService,
  STORAGE_SERVICE_TOKEN,
} from '@application/ports/storage.service.interface';

/**
 * DTOs for API communication
 */
interface LoginRequestDTO {
  email: string;
  password: string;
}

interface RegisterRequestDTO {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  document?: string;
  phone?: string;
  address?: string;
}

interface LoginResponseDTO {
  token: string;
  expiresInSeconds: number;
}

/**
 * Auth HTTP Repository - Infrastructure Layer
 * Implements IAuthRepository using HTTP calls to backend API
 */
@Injectable({
  providedIn: 'root',
})
export class AuthHttpRepository implements IAuthRepository {
  private readonly http = inject(HttpClient);
  private readonly storageService = inject(STORAGE_SERVICE_TOKEN);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  // Login user with credentials

  login(
    credentials: LoginCredentials
  ): Observable<{ token: string; expiresInSeconds: number }> {
    const dto: LoginRequestDTO = {
      email: credentials.email,
      password: credentials.password,
    };

    return this.http.post<LoginResponseDTO>(`${this.baseUrl}/login`, dto);
  }

  // Register new client

  registerClient(
    data: RegisterData
  ): Observable<{ token: string; expiresInSeconds: number }> {
    const dto: RegisterRequestDTO = {
      username: data.username,
      email: data.email,
      password: data.password,
      firstName: data.firstName,
      lastName: data.lastName,
      document: data.document,
      phone: data.phone,
      address: data.address,
    };

    return this.http.post<LoginResponseDTO>(
      `${this.baseUrl}/register-client`,
      dto
    );
  }

  // Get current authenticated user from stored token

  getCurrentUser(): Observable<AuthUser | null> {
    const token = this.storageService.get('auth_token');

    if (!token) {
      return of(null);
    }

    try {
      // Decode JWT token to get user info
      const payload = this.decodeToken(token);

      if (!payload || this.isTokenExpired(payload)) {
        this.logout();
        return of(null);
      }

      const user = new AuthUser(
        payload.sub || '',
        payload.name || payload.email || '',
        payload.email || '',
        payload.role
          ? Array.isArray(payload.role)
            ? payload.role
            : [payload.role]
          : []
      );

      return of(user);
    } catch (error) {
      console.error('Error decoding token:', error);
      this.logout();
      return of(null);
    }
  }

  // Logout current user

  logout(): void {
    this.storageService.remove('auth_token');
  }

  activateAccount(
    userId: string,
    code: string,
    password: string
  ): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/activate`, {
      userId,
      code,
      password,
    });
  }

  forgotPassword(email: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.baseUrl}/forgot-password`,
      { email }
    );
  }

  resetPassword(
    userId: string,
    code: string,
    newPassword: string
  ): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.baseUrl}/reset-password`,
      {
        userId,
        code,
        newPassword,
      }
    );
  }

  // Decode JWT token

  private decodeToken(token: string): any {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error('Error decoding JWT token:', error);
      return null;
    }
  }

  // Check if token is expired

  private isTokenExpired(payload: any): boolean {
    if (!payload.exp) {
      return false;
    }

    const expirationDate = new Date(payload.exp * 1000);
    return expirationDate < new Date();
  }
}

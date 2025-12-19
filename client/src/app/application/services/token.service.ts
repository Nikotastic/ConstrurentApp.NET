import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  sub: string; // User ID
  email: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role':
    | string
    | string[];
  exp: number;
  iss: string;
  aud: string;
}

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private readonly TOKEN_KEY = 'firmness_auth_token';
  private readonly TOKEN_EXPIRY_KEY = 'firmness_token_expiry';

  constructor() {}

  /**
   * Save JWT token to localStorage
   */
  saveToken(token: string): void {
    try {
      const decoded = this.decodeToken(token);
      if (decoded && decoded.exp) {
        localStorage.setItem(this.TOKEN_KEY, token);
        localStorage.setItem(this.TOKEN_EXPIRY_KEY, decoded.exp.toString());
      }
    } catch (error) {
      console.error('Error saving token:', error);
    }
  }

  /**
   * Get JWT token from localStorage
   */
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Remove token from localStorage
   */
  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
  }

  /**
   * Check if token exists and is valid
   */
  isTokenValid(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    try {
      const decoded = this.decodeToken(token);
      if (!decoded || !decoded.exp) {
        return false;
      }

      // Check if token is expired (exp is in seconds, Date.now() is in milliseconds)
      const currentTime = Math.floor(Date.now() / 1000);
      return decoded.exp > currentTime;
    } catch (error) {
      console.error('Error validating token:', error);
      return false;
    }
  }

  /**
   * Decode JWT token
   */
  decodeToken(token?: string): JwtPayload | null {
    try {
      const tokenToUse = token || this.getToken();
      if (!tokenToUse) {
        return null;
      }
      return jwtDecode<JwtPayload>(tokenToUse);
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }


   // Get user ID from token

  getUserId(): string | null {
    const decoded = this.decodeToken();
    return decoded?.sub || null;
  }


  // Get user email from token

  getUserEmail(): string | null {
    const decoded = this.decodeToken();
    return decoded?.email || null;
  }


   // Get username from token

  getUsername(): string | null {
    const decoded = this.decodeToken();
    return (
      decoded?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
      null
    );
  }


  // Get user roles from token

  getUserRoles(): string[] {
    const decoded = this.decodeToken();
    if (!decoded) {
      return [];
    }

    const roles =
      decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    if (Array.isArray(roles)) {
      return roles;
    }
    return roles ? [roles] : [];
  }


  // Get primary user role from token (first role)

  getUserRole(): string | null {
    const roles = this.getUserRoles();
    return roles.length > 0 ? roles[0] : null;
  }


   // Check if user has a specific role
  hasRole(role: string): boolean {
    const roles = this.getUserRoles();
    return roles.includes(role);
  }


  // Check if user is a client

  isClient(): boolean {
    return this.hasRole('Client');
  }


  // Check if user is an admin

  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

   // Get token expiry time

  getTokenExpiry(): Date | null {
    const decoded = this.decodeToken();
    if (!decoded || !decoded.exp) {
      return null;
    }
    return new Date(decoded.exp * 1000);
  }

  // Get time until token expires (in seconds)

  getTimeUntilExpiry(): number {
    const expiry = this.getTokenExpiry();
    if (!expiry) {
      return 0;
    }
    return Math.max(0, Math.floor((expiry.getTime() - Date.now()) / 1000));
  }
}

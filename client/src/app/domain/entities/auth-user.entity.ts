/**
 * Domain Entity: AuthUser
 * Represents an authenticated user in the system
 * Pure TypeScript - No framework dependencies
 */
export class AuthUser {
  constructor(
    public readonly id: string,
    public readonly username: string,
    public readonly email: string,
    public readonly roles: string[]
  ) {}


  // Check if user has a specific role

  hasRole(role: string): boolean {
    return this.roles.includes(role);
  }

  // Check if user is admin

  isAdmin(): boolean {
    return this.hasRole('Admin');
  }


  // Check if user is client

  isClient(): boolean {
    return this.hasRole('Client');
  }


  // Get display name

  getDisplayName(): string {
    return this.username || this.email;
  }
}

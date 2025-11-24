/**
 * Value Object: LoginCredentials
 * Represents login credentials with validation
 */
export class LoginCredentials {
  constructor(public readonly email: string, public readonly password: string) {
    this.validate();
  }

  private validate(): void {
    if (!this.email || !this.email.trim()) {
      throw new Error('Email is required');
    }

    if (!this.isValidEmail(this.email)) {
      throw new Error('Invalid email format');
    }

    if (!this.password || this.password.length < 6) {
      throw new Error('Password must be at least 6 characters');
    }
  }

  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
}

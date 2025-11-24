/**
 * Value Object: RegisterData
 * Represents registration data with validation
 */
export class RegisterData {
  constructor(
    public readonly username: string,
    public readonly email: string,
    public readonly password: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly document?: string,
    public readonly phone?: string,
    public readonly address?: string
  ) {
    this.validate();
  }

  private validate(): void {
    // Username validation
    if (!this.username || this.username.trim().length < 3) {
      throw new Error('Username must be at least 3 characters');
    }

    // Email validation
    if (!this.email || !this.email.trim()) {
      throw new Error('Email is required');
    }

    if (!this.isValidEmail(this.email)) {
      throw new Error('Invalid email format');
    }

    // Password validation
    if (!this.password || this.password.length < 6) {
      throw new Error('Password must be at least 6 characters');
    }

    // Name validation
    if (!this.firstName || this.firstName.trim().length < 2) {
      throw new Error('First name must be at least 2 characters');
    }

    if (!this.lastName || this.lastName.trim().length < 2) {
      throw new Error('Last name must be at least 2 characters');
    }

    // Phone validation (if provided)
    if (this.phone && this.phone.trim() && !this.isValidPhone(this.phone)) {
      throw new Error('Invalid phone format');
    }
  }

  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  private isValidPhone(phone: string): boolean {
    // Basic phone validation (digits, spaces, dashes, parentheses)
    const phoneRegex = /^[\d\s\-()]+$/;
    return phoneRegex.test(phone) && phone.replace(/\D/g, '').length >= 7;
  }
}

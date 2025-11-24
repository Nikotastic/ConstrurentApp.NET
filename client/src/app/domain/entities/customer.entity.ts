/**
 * Customer Entity - Domain Layer
 * Pure business logic, no framework dependencies
 */
export class Customer {
  constructor(
    public readonly id: string,
    public email: string,
    public firstName: string,
    public lastName: string,
    public document: string,
    public phone: string,
    public address: string,
    public isActive: boolean = true,
    public createdAt?: Date
  ) {}


  // Business rule: Get full name

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }

   // Business rule: Validate document format

  isDocumentValid(): boolean {
    return this.document.length >= 6 && this.document.length <= 20;
  }


   // Business rule: Validate email format

  isEmailValid(): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(this.email);
  }

   // Business rule: Check if customer can be deleted

  canBeDeleted(): boolean {
    return this.isActive;
  }
}


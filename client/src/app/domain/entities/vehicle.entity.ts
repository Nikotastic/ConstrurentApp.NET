/**
 * Vehicle Entity - Domain Layer
 */
export class Vehicle {
  constructor(
    public readonly id: string,
    public brand: string,
    public model: string,
    public year: number,
    public licensePlate: string,
    public vehicleType: string,
    public dailyRate: number,
    public hourlyRate?: number,
    public weeklyRate?: number,
    public monthlyRate?: number,
    public imageUrl?: string,
    public status: string = 'Available',
    public isActive: boolean = true
  ) {}


  // Business rule: Check if vehicle is available for rent

  isAvailable(): boolean {
    return this.isActive && this.status === 'Available';
  }

   // Get display name for vehicle
     getDisplayName(): string {
    return `${this.brand} ${this.model} (${this.year})`;
  }
}

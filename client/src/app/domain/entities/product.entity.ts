/**
 * Product Entity - Domain Layer
 */
export class Product {
  constructor(
    public readonly id: string,
    public name: string,
    public description: string,
    public price: number,
    public stock: number,
    public categoryId: string,
    public categoryName?: string,
    public photoUrl?: string,
    public isActive: boolean = true
  ) {}


  // Business rule: Check if product is available for purchase

  isAvailable(): boolean {
    return this.isActive && this.stock > 0;
  }


  // Business rule: Calculate total inventory value

  getTotalValue(): number {
    return this.price * this.stock;
  }

  // Business rule: Check if price is valid
  isPriceValid(): boolean {
    return this.price > 0;
  }


  // Business rule: Check if stock is sufficient

  hasStock(quantity: number): boolean {
    return this.stock >= quantity;
  }
}


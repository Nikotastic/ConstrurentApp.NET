import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product } from '../../domain/entities/product.entity';

export interface CartItem {
  product: Product;
  quantity: number;
  days: number; // For rentals
  isRental: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private readonly BASE_STORAGE_KEY = 'firmness_cart';
  private userId: string | null = null;

  private itemsSubject = new BehaviorSubject<CartItem[]>([]);
  items$ = this.itemsSubject.asObservable();

  constructor() {
    // Initial load (will be guest cart initially)
    this.itemsSubject.next(this.loadFromStorage());

    // Subscribe to changes and save to localStorage
    this.items$.subscribe((items) => {
      this.saveToStorage(items);
    });
  }

  /**
   * Sets the current user context for the cart.
   * Merges guest cart items into user cart if applicable.
   */
  setUser(userId: string | null) {
    // 1. Capture current items (guest items) before switching user
    const guestItems = this.userId === null ? this.itemsSubject.value : [];

    // 2. Switch user context
    this.userId = userId;

    // 3. Load user's stored items
    const userStoredItems = this.loadFromStorage();

    // 4. Merge guest items into user items if we are logging in (userId is not null)
    let finalItems = userStoredItems;

    if (userId !== null && guestItems.length > 0) {
      // Merge logic: Add guest items to user items
      guestItems.forEach((guestItem) => {
        const existingItem = finalItems.find(
          (uItem) => uItem.product.id === guestItem.product.id
        );
        if (existingItem) {
          existingItem.quantity += guestItem.quantity;
        } else {
          finalItems.push(guestItem);
        }
      });

      // Clear guest storage since we moved them
      localStorage.removeItem(`${this.BASE_STORAGE_KEY}_guest`);
    }

    // 5. Update state and save to new user storage
    this.itemsSubject.next(finalItems);
    this.saveToStorage(finalItems);
  }

  private getStorageKey(): string {
    return this.userId
      ? `${this.BASE_STORAGE_KEY}_${this.userId}`
      : `${this.BASE_STORAGE_KEY}_guest`;
  }

  private loadFromStorage(): CartItem[] {
    try {
      const stored = localStorage.getItem(this.getStorageKey());
      return stored ? JSON.parse(stored) : [];
    } catch (error) {
      console.error('Error loading cart from storage:', error);
      return [];
    }
  }

  private saveToStorage(items: CartItem[]): void {
    try {
      localStorage.setItem(this.getStorageKey(), JSON.stringify(items));
    } catch (error) {
      console.error('Error saving cart to storage:', error);
    }
  }

  addToCart(
    product: Product,
    quantity: number = 1,
    isRental: boolean = false,
    days: number = 1
  ) {
    const currentItems = this.itemsSubject.value;
    const existingItem = currentItems.find(
      (item) => item.product.id === product.id
    );

    if (existingItem) {
      existingItem.quantity += quantity;
      this.itemsSubject.next([...currentItems]);
    } else {
      this.itemsSubject.next([
        ...currentItems,
        { product, quantity, isRental, days },
      ]);
    }
  }

  removeFromCart(productId: string) {
    const currentItems = this.itemsSubject.value;
    this.itemsSubject.next(
      currentItems.filter((item) => item.product.id !== productId)
    );
  }

  updateQuantity(productId: string, quantity: number) {
    const currentItems = this.itemsSubject.value;
    const item = currentItems.find((item) => item.product.id === productId);
    if (item) {
      item.quantity = quantity;
      this.itemsSubject.next([...currentItems]);
    }
  }

  updateDays(productId: string, days: number) {
    const currentItems = this.itemsSubject.value;
    const item = currentItems.find((item) => item.product.id === productId);
    if (item) {
      item.days = days;
      this.itemsSubject.next([...currentItems]);
    }
  }

  clearCart() {
    this.itemsSubject.next([]);
  }

  getTotal(): number {
    return this.itemsSubject.value.reduce((total, item) => {
      const itemTotal =
        item.product.price * item.quantity * (item.isRental ? item.days : 1);
      return total + itemTotal;
    }, 0);
  }

  getItemCount(): number {
    return this.itemsSubject.value.reduce(
      (count, item) => count + item.quantity,
      0
    );
  }
}

import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartService } from '@application/services/cart.service';
import { ToastService } from '@application/services/toast.service';
import { ConfirmationService } from '@application/services/confirmation.service';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-4xl font-bold mb-2 text-gray-800">🛒 Shopping Cart</h1>
        <p class="text-gray-600">
          Review and confirm your products before proceeding to payment
        </p>
      </div>

      <!-- Empty Cart State -->
      <div
        *ngIf="(cartService.items$ | async)?.length === 0"
        class="text-center py-16 bg-white rounded-lg shadow-lg"
      >
        <div class="text-8xl mb-4">🛒</div>
        <h2 class="text-3xl font-semibold text-gray-700 mb-3">
          Your cart is empty
        </h2>
        <p class="text-gray-500 mb-8 text-lg">
          Explore our catalog and find what you need!
        </p>
        <a
          routerLink="/products"
          class="inline-flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-bold py-4 px-8 rounded-lg transition-colors shadow-lg"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-6 w-6"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"
            />
          </svg>
          View Products
        </a>
      </div>

      <!-- Cart with Items -->
      <div
        *ngIf="(cartService.items$ | async)?.length! > 0"
        class="grid grid-cols-1 lg:grid-cols-3 gap-8"
      >
        <!-- Cart Items List -->
        <div class="lg:col-span-2 space-y-4">
          <!-- Items Count -->
          <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-4">
            <p class="text-blue-800 font-medium">
              📦 {{ cartService.getItemCount() }}
              {{ cartService.getItemCount() === 1 ? 'product' : 'products' }}
              in your cart
            </p>
          </div>

          <!-- Item Cards -->
          <div
            *ngFor="let item of cartService.items$ | async; let i = index"
            class="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow"
          >
            <div class="flex flex-col sm:flex-row gap-4">
              <!-- Product Image -->
              <div class="flex-shrink-0">
                <img
                  [src]="
                    item.product.photoUrl ||
                    'https://via.placeholder.com/150/e5e7eb/6b7280?text=Sin+Imagen'
                  "
                  [alt]="item.product.name"
                  class="w-32 h-32 object-cover rounded-lg"
                  (error)="onImageError($event)"
                />
              </div>

              <!-- Product Info -->
              <div class="flex-1 min-w-0">
                <h3 class="text-xl font-semibold text-gray-800 mb-2">
                  {{ item.product.name }}
                </h3>
                <p class="text-sm text-gray-600 mb-3 line-clamp-2">
                  {{ item.product.description }}
                </p>

                <!-- Type Badge -->
                <div class="flex items-center gap-2 mb-3">
                  <span
                    class="px-3 py-1 text-xs font-bold rounded-full"
                    [ngClass]="
                      item.isRental
                        ? 'bg-blue-100 text-blue-800'
                        : 'bg-green-100 text-green-800'
                    "
                  >
                    {{ item.isRental ? '📅 Rental' : '💰 Purchase' }}
                  </span>
                  <span class="text-sm text-gray-500">
                    Unit price:
                    <strong
                      >\${{ item.product.price | number : '1.2-2' }}</strong
                    >
                  </span>
                </div>

                <!-- Quantity and Days Controls -->
                <div class="flex flex-wrap gap-4 items-center">
                  <!-- Quantity -->
                  <div class="flex items-center gap-2">
                    <label class="text-sm font-medium text-gray-700"
                      >Quantity:</label
                    >
                    <div class="flex items-center border rounded-lg">
                      <button
                        (click)="
                          decreaseQuantity(item.product.id, item.quantity)
                        "
                        class="px-3 py-2 hover:bg-gray-100 transition-colors"
                      >
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          class="h-4 w-4"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M20 12H4"
                          />
                        </svg>
                      </button>
                      <input
                        type="number"
                        min="1"
                        [max]="item.product.stock"
                        [ngModel]="item.quantity"
                        (ngModelChange)="
                          cartService.updateQuantity(item.product.id, $event)
                        "
                        class="w-16 px-2 py-2 text-center border-x focus:outline-none"
                      />
                      <button
                        (click)="
                          increaseQuantity(
                            item.product.id,
                            item.quantity,
                            item.product.stock
                          )
                        "
                        class="px-3 py-2 hover:bg-gray-100 transition-colors"
                      >
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          class="h-4 w-4"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M12 4v16m8-8H4"
                          />
                        </svg>
                      </button>
                    </div>
                  </div>

                  <!-- Days (for rental) -->
                  <div *ngIf="item.isRental" class="flex items-center gap-2">
                    <label class="text-sm font-medium text-gray-700"
                      >Days:</label
                    >
                    <div class="flex items-center border rounded-lg">
                      <button
                        (click)="decreaseDays(item.product.id, item.days)"
                        class="px-3 py-2 hover:bg-gray-100 transition-colors"
                      >
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          class="h-4 w-4"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M20 12H4"
                          />
                        </svg>
                      </button>
                      <input
                        type="number"
                        min="1"
                        [ngModel]="item.days"
                        (ngModelChange)="
                          cartService.updateDays(item.product.id, $event)
                        "
                        class="w-16 px-2 py-2 text-center border-x focus:outline-none"
                      />
                      <button
                        (click)="increaseDays(item.product.id, item.days)"
                        class="px-3 py-2 hover:bg-gray-100 transition-colors"
                      >
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          class="h-4 w-4"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M12 4v16m8-8H4"
                          />
                        </svg>
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Price and Remove -->
              <div
                class="flex flex-col items-end justify-between min-w-[140px]"
              >
                <div class="text-right">
                  <div class="text-2xl font-bold text-blue-600">
                    \${{ getItemTotal(item) | number : '1.2-2' }}
                  </div>
                  <div class="text-xs text-gray-500 mt-1">
                    {{ item.quantity }} × \${{
                      item.product.price | number : '1.2-2'
                    }}
                    <span *ngIf="item.isRental"> × {{ item.days }} days</span>
                  </div>
                </div>

                <button
                  (click)="cartService.removeFromCart(item.product.id)"
                  class="flex items-center gap-1 text-red-500 hover:text-red-700 text-sm font-medium transition-colors"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-4 w-4"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                    />
                  </svg>
                  Remove
                </button>
              </div>
            </div>
          </div>

          <!-- Continue Shopping -->
          <div class="mt-6">
            <a
              routerLink="/products"
              class="inline-flex items-center gap-2 text-blue-600 hover:text-blue-800 font-medium transition-colors"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M10 19l-7-7m0 0l7-7m-7 7h18"
                />
              </svg>
              Continue Shopping
            </a>
          </div>
        </div>

        <!-- Order Summary Sidebar -->
        <div class="lg:col-span-1">
          <div class="bg-white p-6 rounded-lg shadow-lg sticky top-4">
            <h2 class="text-2xl font-bold mb-6 text-gray-800 border-b pb-3">
              📋 Order Summary
            </h2>

            <!-- Items Breakdown -->
            <div class="space-y-3 mb-6">
              <div class="flex justify-between text-gray-600">
                <span>Subtotal</span>
                <span class="font-medium"
                  >\${{ cartService.getTotal() | number : '1.2-2' }}</span
                >
              </div>
              <div class="flex justify-between text-gray-600">
                <span>IVA (19%)</span>
                <span class="font-medium"
                  >\${{
                    cartService.getTotal() * 0.19 | number : '1.2-2'
                  }}</span
                >
              </div>
              <div class="flex justify-between text-sm text-gray-500">
                <span>Shipping</span>
                <span>To be calculated</span>
              </div>
            </div>

            <!-- Total -->
            <div class="border-t pt-4 mb-6">
              <div class="flex justify-between items-center">
                <span class="text-xl font-bold text-gray-800">Total</span>
                <span class="text-3xl font-bold text-blue-600">
                  \${{ cartService.getTotal() * 1.19 | number : '1.2-2' }}
                </span>
              </div>
            </div>

            <!-- Checkout Button -->
            <button
              (click)="proceedToCheckout()"
              class="w-full bg-green-600 hover:bg-green-700 text-white font-bold py-4 px-6 rounded-lg transition-colors shadow-lg flex items-center justify-center gap-2 mb-3"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-6 w-6"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
                />
              </svg>
              Proceed to Payment
            </button>

            <!-- Clear Cart -->
            <button
              (click)="clearCart()"
              class="w-full bg-red-50 hover:bg-red-100 text-red-600 font-medium py-2 px-4 rounded-lg transition-colors text-sm"
            >
              Clear
            </button>

            <!-- Additional Info -->
            <div class="mt-6 p-4 bg-blue-50 rounded-lg">
              <div class="flex items-start gap-2">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5 text-blue-600 flex-shrink-0 mt-0.5"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"
                  />
                </svg>
                <div class="text-xs text-blue-800">
                  <p class="font-semibold mb-1">Safe Purchase</p>
                  <p>Your data is protected with SSL encryption</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class CartComponent {
  cartService = inject(CartService);
  private router = inject(Router);
  private toastService = inject(ToastService);
  private confirmationService = inject(ConfirmationService);

  getItemTotal(item: any): number {
    return item.product.price * item.quantity * (item.isRental ? item.days : 1);
  }

  async clearCart() {
    const confirmed = await this.confirmationService.confirm({
      title: 'Clear Cart?',
      message:
        'Are you sure you want to clear your cart? This action cannot be undone.',
      confirmText: 'Yes, clear',
      cancelText: 'Cancel',
      type: 'warning',
      icon: '🗑️',
    });

    if (confirmed) {
      this.cartService.clearCart();
      this.toastService.success('Cart cleared successfully');
    }
  }

  increaseQuantity(
    productId: string,
    currentQuantity: number,
    maxStock: number
  ) {
    if (currentQuantity < maxStock) {
      this.cartService.updateQuantity(productId, currentQuantity + 1);
    } else {
      this.toastService.warning('⚠️ This product is out of stock.');
    }
  }

  decreaseQuantity(productId: string, currentQuantity: number) {
    if (currentQuantity > 1) {
      this.cartService.updateQuantity(productId, currentQuantity - 1);
    }
  }

  increaseDays(productId: string, currentDays: number) {
    this.cartService.updateDays(productId, currentDays + 1);
  }

  decreaseDays(productId: string, currentDays: number) {
    if (currentDays > 1) {
      this.cartService.updateDays(productId, currentDays - 1);
    }
  }

  proceedToCheckout() {
    this.router.navigate(['/checkout']);
  }

  onImageError(event: any) {
    event.target.src =
      'https://via.placeholder.com/150/e5e7eb/6b7280?text=Sin+Imagen';
  }
}

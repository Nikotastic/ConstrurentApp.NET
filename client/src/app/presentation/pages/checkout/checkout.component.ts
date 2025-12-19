import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  PaymentService,
  PurchaseItem,
} from '../../../infrastructure/services/payment.service';
import { Router } from '@angular/router';
import { AuthService } from '../../../application/services/auth.service';
import {
  CartService,
  CartItem,
} from '../../../application/services/cart.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div
      class="min-h-screen bg-[#0f172a] text-white flex items-center justify-center p-4 lg:p-8 relative overflow-hidden"
    >
      <!-- Background Blobs -->
      <div
        class="absolute top-[-10%] left-[-10%] w-[500px] h-[500px] bg-blue-600/20 rounded-full blur-[100px] pointer-events-none"
      ></div>
      <div
        class="absolute bottom-[-10%] right-[-10%] w-[500px] h-[500px] bg-purple-600/20 rounded-full blur-[100px] pointer-events-none"
      ></div>

      <!-- Loading Overlay -->
      <div
        *ngIf="isLoading"
        class="fixed inset-0 bg-black/80 backdrop-blur-md z-50 flex flex-col items-center justify-center transition-opacity duration-300"
      >
        <div class="relative">
          <div
            class="animate-spin rounded-full h-20 w-20 border-t-2 border-b-2 border-blue-500"
          ></div>
          <div
            class="absolute top-0 left-0 h-20 w-20 rounded-full border-t-2 border-b-2 border-purple-500 animate-spin-reverse opacity-70"
          ></div>
        </div>
        <h2 class="text-white text-2xl font-light tracking-widest mt-8">
          PROCESSING
        </h2>
        <p class="text-gray-400 text-sm mt-2 tracking-wide">
          Securing your transaction...
        </p>
      </div>

      <!-- Success Modal -->
      <div
        *ngIf="successData"
        class="fixed inset-0 bg-black/90 backdrop-blur-xl z-50 flex items-center justify-center p-4 animate-fade-in"
      >
        <div
          class="bg-gray-900 border border-gray-800 rounded-3xl p-8 max-w-md w-full text-center shadow-2xl relative overflow-hidden group"
        >
          <div
            class="absolute inset-0 bg-gradient-to-br from-blue-500/10 to-purple-500/10 opacity-0 group-hover:opacity-100 transition-opacity duration-500"
          ></div>

          <div
            class="w-24 h-24 bg-gradient-to-tr from-green-400 to-emerald-600 rounded-full flex items-center justify-center mx-auto mb-8 shadow-lg shadow-green-500/30 animate-bounce-small"
          >
            <svg
              class="w-12 h-12 text-white"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              stroke-width="3"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M5 13l4 4L19 7"
              ></path>
            </svg>
          </div>

          <h2 class="text-3xl font-bold text-white mb-2 tracking-tight">
            Payment Successful
          </h2>
          <p class="text-gray-400 mb-8">Your order has been confirmed.</p>

          <div
            class="bg-white/5 rounded-2xl p-6 mb-8 text-left border border-white/10 backdrop-blur-sm"
          >
            <div class="flex justify-between mb-3">
              <span class="text-gray-400 text-sm">Amount Paid</span>
              <span class="text-white font-bold text-lg"
                >\${{ successData.amount | number : '1.2-2' }}</span
              >
            </div>
            <div class="flex justify-between mb-3">
              <span class="text-gray-400 text-sm">Transaction ID</span>
              <span class="text-blue-400 text-sm font-mono tracking-wider">{{
                successData.transactionId
              }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-400 text-sm">Invoice</span>
              <span class="text-gray-300 text-sm font-medium">{{
                successData.invoiceNumber
              }}</span>
            </div>
          </div>

          <div
            class="bg-blue-500/10 border border-blue-500/20 rounded-xl p-4 mb-8 flex items-start gap-4"
          >
            <div class="p-2 bg-blue-500/20 rounded-lg">
              <svg
                class="w-5 h-5 text-blue-400"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                ></path>
              </svg>
            </div>
            <div class="text-left">
              <p class="text-blue-200 text-xs mb-1">Receipt sent to</p>
              <p class="font-semibold text-white text-sm break-all">
                {{ checkoutForm.get('email')?.value }}
              </p>
            </div>
          </div>

          <button
            (click)="closeSuccess()"
            class="w-full bg-white text-black font-bold py-4 px-6 rounded-xl hover:bg-gray-200 transition-all transform hover:scale-[1.02] active:scale-[0.98]"
          >
            Continue Shopping
          </button>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-12 gap-8 max-w-7xl w-full z-10">
        <!-- Left Column: Payment Form -->
        <div class="lg:col-span-7 space-y-8">
          <!-- Visual Credit Card -->
          <div class="perspective-1000 w-full max-w-md mx-auto lg:mx-0">
            <div
              class="relative h-56 w-full bg-gradient-to-br from-gray-800 to-black rounded-2xl shadow-2xl border border-white/10 overflow-hidden transform transition-transform hover:scale-[1.02] duration-300"
            >
              <!-- Card Background Design -->
              <div
                class="absolute inset-0 bg-gradient-to-tr from-blue-600/30 to-purple-600/30"
              ></div>
              <div
                class="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full blur-3xl -mr-16 -mt-16"
              ></div>

              <div
                class="relative p-6 h-full flex flex-col justify-between z-10"
              >
                <div class="flex justify-between items-start">
                  <!-- Chip -->
                  <div
                    class="w-12 h-9 bg-gradient-to-br from-yellow-200 to-yellow-500 rounded-md border border-yellow-600/50 shadow-inner flex items-center justify-center overflow-hidden"
                  >
                    <div
                      class="w-full h-[1px] bg-yellow-700/30 absolute top-1/3"
                    ></div>
                    <div
                      class="w-full h-[1px] bg-yellow-700/30 absolute bottom-1/3"
                    ></div>
                    <div
                      class="h-full w-[1px] bg-yellow-700/30 absolute left-1/3"
                    ></div>
                    <div
                      class="h-full w-[1px] bg-yellow-700/30 absolute right-1/3"
                    ></div>
                  </div>
                  <!-- Visa Logo (Simple SVG) -->
                  <svg
                    class="h-8 text-white opacity-80"
                    viewBox="0 0 48 48"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path
                      d="M18.182 36H12L15.636 12H21.818L18.182 36Z"
                      fill="currentColor"
                    />
                    <path
                      d="M28.364 12L24.727 30.545L23.636 36H18.182L21.818 17.455L18.909 12H25.455L28.364 12Z"
                      fill="currentColor"
                    />
                    <path
                      d="M43.636 12H38.182C37.455 12 36.727 12.364 36.364 13.091L30.909 26.182L28.364 12H22.545L29.455 36H36L43.636 12Z"
                      fill="currentColor"
                    />
                  </svg>
                </div>

                <div class="space-y-6">
                  <!-- Card Number -->
                  <div>
                    <p class="text-xs text-gray-400 mb-1 tracking-wider">
                      CARD NUMBER
                    </p>
                    <p
                      class="text-2xl font-mono tracking-widest text-white drop-shadow-md"
                    >
                      {{
                        checkoutForm.get('cardNumber')?.value ||
                          '0000 0000 0000 0000'
                      }}
                    </p>
                  </div>

                  <div class="flex justify-between">
                    <!-- Card Holder -->
                    <div>
                      <p
                        class="text-[10px] text-gray-400 mb-0.5 tracking-wider"
                      >
                        CARD HOLDER
                      </p>
                      <p
                        class="text-sm font-medium tracking-wide text-white uppercase truncate max-w-[200px]"
                      >
                        {{
                          checkoutForm.get('cardHolder')?.value || 'YOUR NAME'
                        }}
                      </p>
                    </div>
                    <!-- Expiry -->
                    <div class="text-right">
                      <p
                        class="text-[10px] text-gray-400 mb-0.5 tracking-wider"
                      >
                        EXPIRES
                      </p>
                      <p class="text-sm font-medium tracking-wide text-white">
                        {{ checkoutForm.get('expiry')?.value || 'MM/YY' }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Form -->
          <div
            class="bg-gray-900/50 backdrop-blur-sm border border-white/5 rounded-3xl p-8 shadow-xl"
          >
            <h2 class="text-xl font-bold mb-6 flex items-center gap-2">
              <span class="w-1 h-6 bg-blue-500 rounded-full"></span>
              Payment Details
            </h2>

            <form
              [formGroup]="checkoutForm"
              (ngSubmit)="onSubmit()"
              class="space-y-6"
            >
              <!-- Email -->
              <div class="group">
                <label
                  class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 group-focus-within:text-blue-400 transition-colors"
                  >Email for Receipt</label
                >
                <div class="relative">
                  <input
                    type="email"
                    formControlName="email"
                    class="w-full bg-black/20 border border-white/10 rounded-xl px-4 py-4 focus:outline-none focus:border-blue-500/50 focus:ring-1 focus:ring-blue-500/50 transition-all text-white placeholder-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
                    placeholder="name@example.com"
                  />
                  <div
                    class="absolute right-4 top-4 text-green-500"
                    *ngIf="
                      checkoutForm.get('email')?.valid &&
                      checkoutForm.get('email')?.value
                    "
                  >
                    <svg
                      class="w-5 h-5"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M5 13l4 4L19 7"
                      ></path>
                    </svg>
                  </div>
                </div>
              </div>

              <!-- Card Holder -->
              <div class="group">
                <label
                  class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 group-focus-within:text-blue-400 transition-colors"
                  >Card Holder Name</label
                >
                <input
                  type="text"
                  formControlName="cardHolder"
                  class="w-full bg-black/20 border border-white/10 rounded-xl px-4 py-4 focus:outline-none focus:border-blue-500/50 focus:ring-1 focus:ring-blue-500/50 transition-all text-white placeholder-gray-600 uppercase"
                  placeholder="JOHN DOE"
                />
              </div>

              <!-- Card Number -->
              <div class="group">
                <label
                  class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 group-focus-within:text-blue-400 transition-colors"
                  >Card Number</label
                >
                <div class="relative">
                  <input
                    type="text"
                    formControlName="cardNumber"
                    maxlength="19"
                    (input)="onCardNumberInput($event)"
                    class="w-full bg-black/20 border border-white/10 rounded-xl px-4 py-4 pl-12 font-mono focus:outline-none focus:border-blue-500/50 focus:ring-1 focus:ring-blue-500/50 transition-all text-white placeholder-gray-600"
                    placeholder="0000 0000 0000 0000"
                  />
                  <svg
                    class="w-6 h-6 text-gray-500 absolute left-4 top-4"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
                    ></path>
                  </svg>
                </div>
              </div>

              <div class="grid grid-cols-2 gap-6">
                <!-- Expiry -->
                <div class="group">
                  <label
                    class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 group-focus-within:text-blue-400 transition-colors"
                    >Expiry Date</label
                  >
                  <input
                    type="text"
                    formControlName="expiry"
                    maxlength="5"
                    (input)="onExpiryInput($event)"
                    class="w-full bg-black/20 border border-white/10 rounded-xl px-4 py-4 font-mono focus:outline-none focus:border-blue-500/50 focus:ring-1 focus:ring-blue-500/50 transition-all text-white placeholder-gray-600"
                    placeholder="MM/YY"
                  />
                </div>
                <!-- CVC -->
                <div class="group">
                  <label
                    class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 group-focus-within:text-blue-400 transition-colors"
                    >CVC</label
                  >
                  <div class="relative">
                    <input
                      type="text"
                      formControlName="cvc"
                      maxlength="3"
                      class="w-full bg-black/20 border border-white/10 rounded-xl px-4 py-4 font-mono focus:outline-none focus:border-blue-500/50 focus:ring-1 focus:ring-blue-500/50 transition-all text-white placeholder-gray-600"
                      placeholder="123"
                    />
                    <svg
                      class="w-5 h-5 text-gray-500 absolute right-4 top-4"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
                      ></path>
                    </svg>
                  </div>
                </div>
              </div>

              <button
                type="submit"
                [disabled]="
                  checkoutForm.invalid || isLoading || cartItems.length === 0
                "
                class="w-full bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-500 hover:to-blue-600 text-white font-bold py-5 rounded-xl shadow-lg shadow-blue-500/20 transform transition-all hover:-translate-y-1 hover:shadow-blue-500/40 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none mt-4"
              >
                <span class="flex items-center justify-center gap-2">
                  Pay Securely \${{ getSubtotal() * 1.19 | number : '1.2-2' }}
                  <svg
                    class="w-5 h-5"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M17 8l4 4m0 0l-4 4m4-4H3"
                    ></path>
                  </svg>
                </span>
              </button>

              <div
                class="flex items-center justify-center gap-4 text-gray-500 text-xs mt-4"
              >
                <span class="flex items-center gap-1"
                  ><svg class="w-3 h-3" fill="currentColor" viewBox="0 0 24 24">
                    <path
                      d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14h-2v-2h2v2zm0-4h-2V7h2v5z"
                    />
                  </svg>
                  SSL Encrypted</span
                >
                <span>•</span>
                <span>PCI DSS Compliant</span>
              </div>
            </form>
          </div>
        </div>

        <!-- Right Column: Order Summary -->
        <div class="lg:col-span-5">
          <div
            class="bg-white/5 backdrop-blur-xl border border-white/10 rounded-3xl p-8 sticky top-8"
          >
            <h2
              class="text-xl font-bold mb-6 text-white flex justify-between items-center"
            >
              Order Summary
              <span
                class="text-sm font-normal text-gray-400 bg-white/5 px-3 py-1 rounded-full"
                >{{ cartItems.length }} Items</span
              >
            </h2>

            <div
              class="space-y-4 mb-8 max-h-[500px] overflow-y-auto pr-2 custom-scrollbar"
            >
              <div
                *ngFor="let item of cartItems"
                class="group flex gap-4 p-3 rounded-xl hover:bg-white/5 transition-colors"
              >
                <!-- Product Image -->
                <div
                  class="w-20 h-20 bg-gray-800 rounded-lg overflow-hidden flex-shrink-0 border border-white/5 relative"
                >
                  <img
                    [src]="
                      item.product.photoUrl || 'https://via.placeholder.com/150'
                    "
                    alt="Product"
                    class="w-full h-full object-cover transform group-hover:scale-110 transition-transform duration-500"
                    onerror="this.src='https://via.placeholder.com/150'"
                  />
                </div>

                <!-- Details -->
                <div class="flex-1 min-w-0">
                  <h3 class="font-medium text-sm text-white truncate">
                    {{ item.product.name }}
                  </h3>
                  <div class="flex items-center gap-2 mt-1">
                    <span
                      class="text-xs text-gray-400 bg-white/5 px-2 py-0.5 rounded"
                      >Qty: {{ item.quantity }}</span
                    >
                    <span
                      *ngIf="item.isRental"
                      class="text-xs text-blue-300 bg-blue-500/10 px-2 py-0.5 rounded"
                      >{{ item.days }} days</span
                    >
                  </div>
                  <p class="text-sm font-bold text-white mt-2">
                    \${{ getItemTotal(item) | number : '1.2-2' }}
                  </p>
                </div>
              </div>

              <div
                *ngIf="cartItems.length === 0"
                class="text-center text-gray-500 py-12 border-2 border-dashed border-white/5 rounded-xl"
              >
                <svg
                  class="w-12 h-12 mx-auto mb-3 opacity-50"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"
                  ></path>
                </svg>
                <p>Your cart is empty.</p>
              </div>
            </div>

            <!-- Totals -->
            <div class="space-y-3 pt-6 border-t border-white/10">
              <div class="flex justify-between text-gray-400 text-sm">
                <span>Subtotal</span>
                <span>\${{ getSubtotal() | number : '1.2-2' }}</span>
              </div>
              <div class="flex justify-between text-gray-400 text-sm">
                <span>Tax (19%)</span>
                <span>\${{ getSubtotal() * 0.19 | number : '1.2-2' }}</span>
              </div>
              <div class="flex justify-between text-gray-400 text-sm">
                <span>Shipping</span>
                <span class="text-green-400">Free</span>
              </div>
            </div>

            <div class="mt-6 pt-6 border-t border-white/10">
              <div class="flex justify-between items-end">
                <span class="text-gray-400 text-sm mb-1">Total Amount</span>
                <span class="text-3xl font-bold text-white tracking-tight"
                  >\${{ getSubtotal() * 1.19 | number : '1.2-2' }}</span
                >
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .custom-scrollbar::-webkit-scrollbar {
        width: 4px;
      }
      .custom-scrollbar::-webkit-scrollbar-track {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 4px;
      }
      .custom-scrollbar::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.2);
        border-radius: 4px;
      }
      .custom-scrollbar::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.3);
      }

      .perspective-1000 {
        perspective: 1000px;
      }

      .animate-spin-reverse {
        animation: spin-reverse 1s linear infinite;
      }

      @keyframes spin-reverse {
        from {
          transform: rotate(360deg);
        }
        to {
          transform: rotate(0deg);
        }
      }

      .animate-fade-in {
        animation: fadeIn 0.3s ease-out forwards;
      }

      @keyframes fadeIn {
        from {
          opacity: 0;
          transform: scale(0.95);
        }
        to {
          opacity: 1;
          transform: scale(1);
        }
      }

      .animate-bounce-small {
        animation: bounce-small 2s ease-in-out infinite;
      }

      @keyframes bounce-small {
        0%,
        100% {
          transform: translateY(0);
        }
        50% {
          transform: translateY(-10px);
        }
      }
    `,
  ],
})
export class CheckoutComponent implements OnInit, OnDestroy {
  checkoutForm: FormGroup;
  isLoading = false;
  successData: any = null;
  cartItems: CartItem[] = [];
  private cartSubscription: Subscription | null = null;
  private authSubscription: Subscription | null = null;

  constructor(
    private fb: FormBuilder,
    private paymentService: PaymentService,
    private router: Router,
    private authService: AuthService,
    private cartService: CartService
  ) {
    this.checkoutForm = this.fb.group({
      email: [
        { value: '', disabled: true },
        [Validators.required, Validators.email],
      ],
      cardHolder: ['', Validators.required],
      cardNumber: ['', [Validators.required, Validators.minLength(19)]], // 16 digits + 3 spaces
      expiry: ['', Validators.required],
      cvc: ['', [Validators.required, Validators.minLength(3)]],
    });
  }

  ngOnInit(): void {
    // 1. Subscribe to User Changes
    this.authSubscription = this.authService.currentUser$.subscribe((user) => {
      if (user && user.email) {
        this.checkoutForm.patchValue({ email: user.email });
        this.checkoutForm.get('email')?.disable();
      } else {
        this.checkoutForm.get('email')?.enable();
      }
    });

    // 2. Subscribe to Cart Items
    this.cartSubscription = this.cartService.items$.subscribe((items) => {
      this.cartItems = items;
    });
  }

  ngOnDestroy(): void {
    if (this.cartSubscription) {
      this.cartSubscription.unsubscribe();
    }
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  // Formateo de Tarjeta
  onCardNumberInput(event: any) {
    let value = event.target.value.replace(/\D/g, '');
    if (value.length > 16) value = value.substring(0, 16);
    // Agregar espacios cada 4 dígitos
    const formatted = value.match(/.{1,4}/g)?.join(' ') || value;
    this.checkoutForm
      .get('cardNumber')
      ?.setValue(formatted, { emitEvent: false });
  }

  // Formateo de Expiración
  onExpiryInput(event: any) {
    let value = event.target.value.replace(/\D/g, '');
    if (value.length > 4) value = value.substring(0, 4);
    if (value.length >= 2) {
      value = value.substring(0, 2) + '/' + value.substring(2);
    }
    this.checkoutForm.get('expiry')?.setValue(value, { emitEvent: false });
  }

  getItemTotal(item: CartItem): number {
    return item.product.price * item.quantity * (item.isRental ? item.days : 1);
  }

  getSubtotal(): number {
    return this.cartService.getTotal();
  }

  onSubmit() {
    if (this.checkoutForm.invalid) return;
    if (this.cartItems.length === 0) {
      alert('Your cart is empty');
      return;
    }

    const formValues = this.checkoutForm.getRawValue();

    if (!formValues.email) {
      alert('Email is required to send the receipt.');
      return;
    }

    this.isLoading = true;
    const total = this.getSubtotal() * 1.19;

    // Limpiar número de tarjeta para envío (quitar espacios)
    const cleanCardNumber = formValues.cardNumber.replace(/\s/g, '');

    const purchaseItems: PurchaseItem[] = this.cartItems.map((item) => ({
      description: `${item.product.name} ${
        item.isRental ? '(Rental ' + item.days + ' days)' : ''
      }`,
      quantity: item.quantity,
      unitPrice: item.product.price * (item.isRental ? item.days : 1),
    }));

    const request = {
      customerName: formValues.cardHolder,
      customerEmail: formValues.email,
      totalAmount: total,
      items: purchaseItems,
      simulateFailure: false,
    };

    this.paymentService.simulatePurchase(request).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.successData = response.data;
          this.cartService.clearCart();
        }
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Payment failed', err);
        alert('Payment failed. Please try again.');
      },
    });
  }

  closeSuccess() {
    this.router.navigate(['/products']);
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderService, Sale } from '@application/services/order.service';
import {
  RentalService,
  VehicleRental,
} from '@application/services/rental.service';
import { ProfileService } from '@application/services/profile.service';
import { ToastService } from '@application/services/toast.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-7xl mx-auto">
        <!-- Header -->
        <div class="mb-8">
          <h1 class="text-3xl font-bold text-gray-800">📦 My Orders</h1>
          <p class="text-gray-600">
            View your purchase history and vehicle rentals
          </p>
        </div>

        <!-- Loading State -->
        <div *ngIf="loading" class="flex justify-center py-12">
          <div
            class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"
          ></div>
        </div>

        <!-- Content -->
        <div *ngIf="!loading" class="space-y-8">
          <!-- Product Purchases Section -->
          <div class="bg-white rounded-2xl shadow-xl overflow-hidden">
            <div class="bg-gradient-to-r from-blue-600 to-indigo-700 px-6 py-4">
              <h2 class="text-xl font-bold text-white">🛒 Product Purchases</h2>
            </div>
            <div class="p-6">
              <div
                *ngIf="sales.length === 0"
                class="text-center py-12 bg-gray-50 rounded-lg border border-dashed border-gray-300"
              >
                <p class="text-gray-500 text-lg">No purchases yet</p>
              </div>

              <div *ngIf="sales.length > 0" class="overflow-x-auto">
                <table class="min-w-full divide-y divide-gray-200">
                  <thead class="bg-gray-50">
                    <tr>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Order #
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Date
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Items
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Total
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Payment
                      </th>
                    </tr>
                  </thead>
                  <tbody class="bg-white divide-y divide-gray-200">
                    <tr
                      *ngFor="let sale of paginatedSales"
                      class="hover:bg-gray-50"
                    >
                      <td class="px-6 py-4 whitespace-nowrap">
                        <div class="text-sm font-medium text-gray-900">
                          #{{ sale.id.substring(0, 8).toUpperCase() }}
                        </div>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap">
                        <div class="text-sm text-gray-900">
                          {{ sale.saleDate | date : 'MMM d, y' }}
                        </div>
                        <div class="text-xs text-gray-500">
                          {{ sale.saleDate | date : 'h:mm a' }}
                        </div>
                      </td>
                      <td class="px-6 py-4">
                        <div class="text-sm text-gray-900">
                          <div *ngFor="let item of sale.items" class="mb-1">
                            {{ item.productName }} (x{{ item.quantity }})
                          </div>
                        </div>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap">
                        <div class="text-sm font-bold text-gray-900">
                          {{ sale.totalAmount | currency }}
                        </div>
                        <div class="text-xs text-gray-500">
                          Subtotal: {{ sale.subtotal | currency }}
                        </div>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap">
                        <span
                          class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800"
                        >
                          {{ sale.paymentMethod }}
                        </span>
                      </td>
                    </tr>
                  </tbody>
                </table>

                <!-- Sales Pagination -->
                <div
                  *ngIf="sales.length > salesPerPage"
                  class="mt-4 flex justify-between items-center"
                >
                  <div class="text-sm text-gray-600">
                    Showing {{ salesStartIndex + 1 }} to {{ salesEndIndex }} of
                    {{ sales.length }} orders
                  </div>
                  <div class="flex gap-2">
                    <button
                      (click)="previousSalesPage()"
                      [disabled]="currentSalesPage === 1"
                      class="px-3 py-1 bg-gray-200 hover:bg-gray-300 disabled:opacity-50 disabled:cursor-not-allowed rounded transition-colors text-sm"
                    >
                      ← Prev
                    </button>
                    <span class="px-3 py-1 text-sm text-gray-700">
                      Page {{ currentSalesPage }} of {{ totalSalesPages }}
                    </span>
                    <button
                      (click)="nextSalesPage()"
                      [disabled]="currentSalesPage === totalSalesPages"
                      class="px-3 py-1 bg-gray-200 hover:bg-gray-300 disabled:opacity-50 disabled:cursor-not-allowed rounded transition-colors text-sm"
                    >
                      Next →
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Vehicle Rentals Section -->
          <div class="bg-white rounded-2xl shadow-xl overflow-hidden">
            <div class="bg-gradient-to-r from-green-600 to-teal-700 px-6 py-4">
              <h2 class="text-xl font-bold text-white">🚜 Vehicle Rentals</h2>
            </div>
            <div class="p-6">
              <div
                *ngIf="rentals.length === 0"
                class="text-center py-12 bg-gray-50 rounded-lg border border-dashed border-gray-300"
              >
                <p class="text-gray-500 text-lg">No vehicle rentals yet</p>
              </div>

              <div *ngIf="rentals.length > 0" class="overflow-x-auto">
                <table class="min-w-full divide-y divide-gray-200">
                  <thead class="bg-gray-50">
                    <tr>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Vehicle
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Rental Period
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Status
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Total
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Pending
                      </th>
                      <th
                        class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                      >
                        Actions
                      </th>
                    </tr>
                  </thead>
                  <tbody class="bg-white divide-y divide-gray-200">
                    <tr
                      *ngFor="let rental of paginatedRentals"
                      class="hover:bg-gray-50"
                    >
                      <td class="px-6 py-4">
                        <div class="text-sm font-medium text-gray-900">
                          {{ rental.vehicleDisplayName }}
                        </div>
                        <div class="text-xs text-gray-500">
                          {{ rental.pickupLocation || 'Main Office' }}
                        </div>
                      </td>
                      <td class="px-6 py-4">
                        <div class="text-sm text-gray-900">
                          From: {{ rental.startDate | date : 'MMM d, y' }}
                        </div>
                        <div class="text-sm text-gray-500">
                          To:
                          {{ rental.estimatedReturnDate | date : 'MMM d, y' }}
                        </div>
                        <div class="text-xs text-gray-400">
                          {{ rental.durationInDays }} days
                        </div>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap">
                        <span
                          class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                          [ngClass]="{
                            'bg-green-100 text-green-800':
                              rental.status === 'Active',
                            'bg-yellow-100 text-yellow-800':
                              rental.status === 'Pending',
                            'bg-gray-100 text-gray-800':
                              rental.status === 'Completed',
                            'bg-red-100 text-red-800':
                              rental.status === 'Cancelled'
                          }"
                        >
                          {{ rental.status }}
                        </span>
                        <div
                          *ngIf="rental.isOverdue"
                          class="text-xs text-red-600 font-bold mt-1"
                        >
                          OVERDUE
                        </div>
                      </td>
                      <td
                        class="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-900"
                      >
                        {{ rental.totalAmount | currency }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm">
                        <span
                          [class.text-red-600]="rental.pendingAmount > 0"
                          [class.text-green-600]="rental.pendingAmount <= 0"
                        >
                          {{
                            rental.pendingAmount > 0
                              ? (rental.pendingAmount | currency)
                              : 'Paid'
                          }}
                        </span>
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm">
                        <button
                          *ngIf="
                            rental.status === 'Pending' ||
                            rental.status === 'Active'
                          "
                          (click)="openCancelModal(rental)"
                          class="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded-lg text-xs font-semibold transition-colors"
                        >
                          Cancel
                        </button>
                        <span
                          *ngIf="
                            rental.status === 'Cancelled' ||
                            rental.status === 'Completed'
                          "
                          class="text-gray-400 text-xs"
                        >
                          —
                        </span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <!-- Rentals Pagination -->
              <div
                *ngIf="rentals.length > rentalsPerPage"
                class="mt-4 flex justify-between items-center px-6 pb-4"
              >
                <div class="text-sm text-gray-600">
                  Showing {{ rentalsStartIndex + 1 }} to
                  {{ rentalsEndIndex }} of {{ rentals.length }} rentals
                </div>
                <div class="flex gap-2">
                  <button
                    (click)="previousRentalsPage()"
                    [disabled]="currentRentalsPage === 1"
                    class="px-3 py-1 bg-gray-200 hover:bg-gray-300 disabled:opacity-50 disabled:cursor-not-allowed rounded transition-colors text-sm"
                  >
                    ← Prev
                  </button>
                  <span class="px-3 py-1 text-sm text-gray-700">
                    Page {{ currentRentalsPage }} of {{ totalRentalsPages }}
                  </span>
                  <button
                    (click)="nextRentalsPage()"
                    [disabled]="currentRentalsPage === totalRentalsPages"
                    class="px-3 py-1 bg-gray-200 hover:bg-gray-300 disabled:opacity-50 disabled:cursor-not-allowed rounded transition-colors text-sm"
                  >
                    Next →
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Cancel Rental Modal -->
      <div
        *ngIf="showCancelModal"
        class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
        (click)="closeCancelModal()"
      >
        <div
          class="bg-white rounded-lg shadow-xl max-w-md w-full p-6"
          (click)="$event.stopPropagation()"
        >
          <h3 class="text-2xl font-bold mb-4 text-red-600">Cancel Rental</h3>
          <p class="text-gray-700 mb-4">
            Are you sure you want to cancel the rental for
            <strong>{{ selectedRental?.vehicleDisplayName }}</strong
            >?
          </p>

          <div class="mb-4">
            <label class="block text-sm font-medium text-gray-700 mb-2"
              >Reason for cancellation (optional)</label
            >
            <textarea
              [(ngModel)]="cancellationReason"
              rows="3"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-red-500 focus:border-transparent"
              placeholder="Enter reason..."
            ></textarea>
          </div>

          <div class="flex gap-4">
            <button
              (click)="closeCancelModal()"
              class="flex-1 bg-gray-200 hover:bg-gray-300 text-gray-700 font-bold py-2 px-4 rounded-lg transition-colors"
            >
              No, Keep It
            </button>
            <button
              (click)="confirmCancelRental()"
              [disabled]="cancelling"
              class="flex-1 bg-red-600 hover:bg-red-700 text-white font-bold py-2 px-4 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
            >
              <span *ngIf="cancelling" class="mr-2">
                <div
                  class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"
                ></div>
              </span>
              {{ cancelling ? 'Cancelling...' : 'Yes, Cancel' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class MyOrdersComponent implements OnInit {
  private orderService = inject(OrderService);
  private rentalService = inject(RentalService);
  private profileService = inject(ProfileService);
  private toastService = inject(ToastService);

  sales: Sale[] = [];
  rentals: VehicleRental[] = [];
  loading = false;

  // Cancel modal
  showCancelModal = false;
  selectedRental: VehicleRental | null = null;
  cancellationReason = '';
  cancelling = false;

  // Sales Pagination
  currentSalesPage = 1;
  salesPerPage = 10;
  paginatedSales: Sale[] = [];
  totalSalesPages = 0;
  salesStartIndex = 0;
  salesEndIndex = 0;

  // Rentals Pagination
  currentRentalsPage = 1;
  rentalsPerPage = 10;
  paginatedRentals: VehicleRental[] = [];
  totalRentalsPages = 0;
  rentalsStartIndex = 0;
  rentalsEndIndex = 0;

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.loading = true;

    this.profileService.getProfile().subscribe({
      next: (profile) => {
        if (profile) {
          // Load both sales and rentals in parallel
          this.loadSales(profile.id);
          this.loadRentals(profile.id);
        }
      },
      error: (err) => {
        console.error('Error loading profile', err);
        this.loading = false;
      },
    });
  }

  // Load sales by customer
  loadSales(customerId: string) {
    this.orderService.getSalesByCustomer(customerId).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.sales = response.data;
          this.updateSalesPagination();
        }
        this.checkLoadingComplete();
      },
      error: (err) => {
        console.error('Error loading sales', err);
        this.checkLoadingComplete();
      },
    });
  }

  // Load rentals by customer
  loadRentals(customerId: string) {
    this.rentalService.getRentalsByCustomer(customerId).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.rentals = response.data;
          this.updateRentalsPagination();
        }
        this.checkLoadingComplete();
      },
      error: (err) => {
        console.error('Error loading rentals', err);
        this.checkLoadingComplete();
      },
    });
  }

  // Check if both sales and rentals have loaded
  private loadedCount = 0;
  checkLoadingComplete() {
    this.loadedCount++;
    if (this.loadedCount >= 2) {
      this.loading = false;
    }
  }

  // Open cancel modal
  openCancelModal(rental: VehicleRental) {
    this.selectedRental = rental;
    this.cancellationReason = '';
    this.showCancelModal = true;
  }

  // Close cancel modal
  closeCancelModal() {
    this.showCancelModal = false;
    this.selectedRental = null;
    this.cancellationReason = '';
  }

  // Confirm cancel rental
  confirmCancelRental() {
    if (!this.selectedRental) return;

    this.cancelling = true;
    const reason = this.cancellationReason || 'Cancelled by customer';

    this.rentalService.cancelRental(this.selectedRental.id, reason).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          // Update the rental status in the list
          const index = this.rentals.findIndex(
            (r) => r.id === this.selectedRental!.id
          );
          if (index !== -1 && response.data) {
            this.rentals[index] = response.data;
          }
          this.toastService.success('Rental cancelled successfully! 🚫');
          this.closeCancelModal();
        } else {
          this.toastService.error(
            response.message || 'Could not cancel rental'
          );
        }
        this.cancelling = false;
      },
      error: (err) => {
        console.error('Error cancelling rental', err);
        const errorMessage =
          err.error?.message || 'Error cancelling rental. Please try again.';
        this.toastService.error(errorMessage);
        this.cancelling = false;
      },
    });
  }

  // Sales Pagination Methods
  updateSalesPagination() {
    this.totalSalesPages = Math.ceil(this.sales.length / this.salesPerPage);
    if (
      this.currentSalesPage > this.totalSalesPages &&
      this.totalSalesPages > 0
    ) {
      this.currentSalesPage = this.totalSalesPages;
    }
    if (this.currentSalesPage < 1) {
      this.currentSalesPage = 1;
    }

    this.salesStartIndex = (this.currentSalesPage - 1) * this.salesPerPage;
    this.salesEndIndex = Math.min(
      this.salesStartIndex + this.salesPerPage,
      this.sales.length
    );
    this.paginatedSales = this.sales.slice(
      this.salesStartIndex,
      this.salesEndIndex
    );
  }

  previousSalesPage() {
    if (this.currentSalesPage > 1) {
      this.currentSalesPage--;
      this.updateSalesPagination();
    }
  }

  nextSalesPage() {
    if (this.currentSalesPage < this.totalSalesPages) {
      this.currentSalesPage++;
      this.updateSalesPagination();
    }
  }

  // Rentals Pagination Methods
  updateRentalsPagination() {
    this.totalRentalsPages = Math.ceil(
      this.rentals.length / this.rentalsPerPage
    );
    if (
      this.currentRentalsPage > this.totalRentalsPages &&
      this.totalRentalsPages > 0
    ) {
      this.currentRentalsPage = this.totalRentalsPages;
    }
    if (this.currentRentalsPage < 1) {
      this.currentRentalsPage = 1;
    }

    this.rentalsStartIndex =
      (this.currentRentalsPage - 1) * this.rentalsPerPage;
    this.rentalsEndIndex = Math.min(
      this.rentalsStartIndex + this.rentalsPerPage,
      this.rentals.length
    );
    this.paginatedRentals = this.rentals.slice(
      this.rentalsStartIndex,
      this.rentalsEndIndex
    );
  }

  previousRentalsPage() {
    if (this.currentRentalsPage > 1) {
      this.currentRentalsPage--;
      this.updateRentalsPagination();
    }
  }

  nextRentalsPage() {
    if (this.currentRentalsPage < this.totalRentalsPages) {
      this.currentRentalsPage++;
      this.updateRentalsPagination();
    }
  }
}

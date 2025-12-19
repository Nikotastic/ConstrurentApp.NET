import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '@application/services/product.service';
import { VehicleService } from '@application/services/vehicle.service';
import { CartService } from '@application/services/cart.service';
import { ToastService } from '@application/services/toast.service';
import { Product } from '@domain/entities/product.entity';
import { Vehicle } from '@domain/entities/vehicle.entity';
import { Observable, of, forkJoin } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CatalogItem } from './catalog-item.interface';
import { RentalService } from '@application/services/rental.service';
import { ProfileService } from '@application/services/profile.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <!-- Header with Search -->
      <div class="mb-8">
        <h1 class="text-4xl font-bold mb-4 text-gray-800">
          🛒 Product Catalog
        </h1>
        <p class="text-gray-600 mb-6">
          Explore our catalog of construction materials and industrial machinery
        </p>

        <!-- Search Bar -->
        <div class="flex gap-4 mb-6">
          <div class="flex-1">
            <input
              type="text"
              [(ngModel)]="searchTerm"
              (input)="onSearch()"
              placeholder="🔍 Search for products or vehicles..."
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        <!-- Filter Options -->
        <div class="flex gap-4 flex-wrap">
          <button
            (click)="filterByType('all')"
            [class]="
              selectedFilter === 'all'
                ? 'bg-blue-600 text-white'
                : 'bg-white text-gray-700 hover:bg-gray-100'
            "
            class="px-4 py-2 rounded-lg border transition-colors"
          >
            📦 All
          </button>
          <button
            (click)="filterByType('products')"
            [class]="
              selectedFilter === 'products'
                ? 'bg-blue-600 text-white'
                : 'bg-white text-gray-700 hover:bg-gray-100'
            "
            class="px-4 py-2 rounded-lg border transition-colors"
          >
            🏗️ Products
          </button>
          <button
            (click)="filterByType('vehicles')"
            [class]="
              selectedFilter === 'vehicles'
                ? 'bg-blue-600 text-white'
                : 'bg-white text-gray-700 hover:bg-gray-100'
            "
            class="px-4 py-2 rounded-lg border transition-colors"
          >
            🚜 Vehicles
          </button>
        </div>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading" class="text-center py-12">
        <div
          class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-blue-500 border-t-transparent"
        ></div>
        <p class="mt-4 text-gray-600">Loading catalog...</p>
      </div>

      <!-- Error State -->
      <div
        *ngIf="error && !loading"
        class="bg-red-50 border border-red-200 rounded-lg p-6 text-center"
      >
        <div class="text-4xl mb-2">⚠️</div>
        <h3 class="text-xl font-semibold text-red-800 mb-2">
          Error loading catalog
        </h3>
        <p class="text-red-600 mb-4">{{ error }}</p>
        <button
          (click)="loadAll()"
          class="bg-red-600 hover:bg-red-700 text-white px-6 py-2 rounded-lg transition-colors"
        >
          Reintentar
        </button>
      </div>

      <!-- Content -->
      <ng-container *ngIf="!loading && !error">
        <div *ngIf="filteredProducts$ | async as items">
          <!-- Empty State -->
          <div
            *ngIf="items.length === 0"
            class="text-center py-12 bg-white rounded-lg shadow"
          >
            <div class="text-6xl mb-4">📦</div>
            <h2 class="text-2xl font-semibold text-gray-600 mb-2">
              No results found
            </h2>
            <p class="text-gray-500">Try adjusting the search filters</p>
          </div>

          <!-- Grid -->
          <div
            class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
          >
            <div
              *ngFor="let item of paginatedItems"
              class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1"
            >
              <!-- Image -->
              <div
                class="h-48 bg-gradient-to-br from-gray-100 to-gray-200 relative overflow-hidden"
              >
                <img
                  [src]="
                    item.photoUrl ||
                    'https://via.placeholder.com/400x300/e5e7eb/6b7280?text=Sin+Imagen'
                  "
                  [alt]="item.name"
                  class="w-full h-full object-cover"
                  (error)="onImageError($event)"
                />

                <!-- Stock/Status Badge -->
                <div class="absolute top-2 left-2">
                  <span
                    [class]="getStockBadgeClass(item)"
                    class="text-white text-xs font-bold px-2 py-1 rounded-full"
                  >
                    {{ getStockLabel(item) }}
                  </span>
                </div>

                <!-- Category Badge (Interactive) -->
                <div
                  *ngIf="item.categoryName"
                  class="absolute top-2 right-2 group z-10"
                >
                  <div
                    class="bg-blue-600/90 backdrop-blur-sm text-white text-xs font-bold px-2 py-1.5 rounded-full shadow-lg cursor-help transition-all duration-300 max-w-[32px] group-hover:max-w-[250px] overflow-hidden whitespace-nowrap flex items-center"
                    [title]="item.categoryName"
                  >
                    <!-- Icon -->
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="h-4 w-4 flex-shrink-0"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"
                      />
                    </svg>
                    <!-- Text (Revealed on hover) -->
                    <span
                      class="opacity-0 group-hover:opacity-100 ml-2 transition-opacity duration-300"
                    >
                      {{ item.categoryName }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Info -->
              <div class="p-4">
                <h3
                  class="text-lg font-semibold text-gray-800 mb-2 line-clamp-1"
                  [title]="item.name"
                >
                  {{ item.name }}
                </h3>
                <p
                  class="text-gray-600 text-sm mb-4 line-clamp-2"
                  [title]="item.description"
                >
                  {{ item.description }}
                </p>

                <!-- Price and Action -->
                <div
                  class="flex justify-between items-center mt-4 pt-4 border-t"
                >
                  <div>
                    <span class="text-2xl font-bold text-blue-600">
                      $ {{ item.price | number : '1.2-2' }}
                    </span>
                    <span class="text-xs text-gray-500 block">
                      {{ item.itemType === 'vehicle' ? 'per day' : 'per unit' }}
                    </span>
                  </div>
                  <button
                    (click)="showAddToCartModal(item)"
                    [disabled]="item.stock === 0"
                    [class]="
                      item.stock === 0
                        ? 'bg-gray-400 cursor-not-allowed'
                        : 'bg-blue-600 hover:bg-blue-700'
                    "
                    class="text-white font-bold py-2 px-4 rounded-lg transition-colors duration-200 flex items-center gap-2"
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
                        d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z"
                      />
                    </svg>
                    {{
                      item.stock === 0
                        ? 'Out of stock'
                        : item.itemType === 'vehicle'
                        ? 'Reserve'
                        : 'Add'
                    }}
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Pagination Controls -->
          <div
            *ngIf="paginatedItems.length > 0"
            class="mt-8 flex flex-col sm:flex-row justify-between items-center gap-4 bg-white p-4 rounded-lg shadow"
          >
            <div class="text-sm text-gray-600">
              Showing {{ startIndex + 1 }} to {{ endIndex }} of
              {{ items.length }} items
            </div>
            <div class="flex items-center gap-2">
              <button
                (click)="previousPage()"
                [disabled]="currentPage === 1"
                class="px-4 py-2 bg-gray-200 hover:bg-gray-300 disabled:opacity-50 disabled:cursor-not-allowed rounded-lg transition-colors"
              >
                ← Previous
              </button>
              <div class="flex gap-1">
                <button
                  *ngFor="let page of visiblePages"
                  (click)="goToPage(page)"
                  [class]="
                    currentPage === page
                      ? 'bg-blue-600 text-white'
                      : 'bg-gray-200 hover:bg-gray-300 text-gray-700'
                  "
                  class="px-3 py-2 rounded-lg transition-colors min-w-[40px]"
                >
                  {{ page }}
                </button>
              </div>
              <button
                (click)="nextPage()"
                [disabled]="currentPage === totalPages"
                class="px-4 py-2 bg-gray-200 hover:bg-gray-300 disabled:opacity-50 disabled:cursor-not-allowed rounded-lg transition-colors"
              >
                Next →
              </button>
            </div>
            <div class="text-sm text-gray-600">
              Page {{ currentPage }} of {{ totalPages }}
            </div>
          </div>
        </div>
      </ng-container>

      <!-- Add to Cart Modal -->
      <div
        *ngIf="selectedItem"
        class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
        (click)="closeModal()"
      >
        <div
          class="bg-white rounded-lg shadow-xl max-w-md w-full p-6"
          (click)="$event.stopPropagation()"
        >
          <h3 class="text-2xl font-bold mb-4">Add to Cart</h3>
          <p class="text-gray-700 mb-4">{{ selectedItem.name }}</p>

          <div class="space-y-4">
            <!-- Type: Sale or Rental (Only for products) -->
            <div *ngIf="selectedItem.itemType === 'product'">
              <label class="block text-sm font-medium text-gray-700 mb-2"
                >Type of operation</label
              >
              <div class="flex gap-4">
                <button
                  (click)="isRental = false"
                  [class]="
                    !isRental
                      ? 'bg-green-600 text-white'
                      : 'bg-gray-200 text-gray-700'
                  "
                  class="flex-1 py-2 px-4 rounded-lg transition-colors"
                >
                  💰 Purchase
                </button>
                <button
                  (click)="isRental = true"
                  [class]="
                    isRental
                      ? 'bg-blue-600 text-white'
                      : 'bg-gray-200 text-gray-700'
                  "
                  class="flex-1 py-2 px-4 rounded-lg transition-colors"
                >
                  📅 Rental
                </button>
              </div>
            </div>

            <!-- Quantity -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2"
                >Quantity</label
              >
              <input
                type="number"
                [(ngModel)]="quantity"
                min="1"
                [max]="selectedItem.stock"
                class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <!-- Days (for rental product) -->
            <div *ngIf="isRental && selectedItem.itemType === 'product'">
              <label class="block text-sm font-medium text-gray-700 mb-2"
                >Days of rental</label
              >
              <input
                type="number"
                [(ngModel)]="days"
                min="1"
                class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <!-- Date Range (for vehicle rental) -->
            <div *ngIf="selectedItem.itemType === 'vehicle'" class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2"
                  >Start Date</label
                >
                <input
                  type="date"
                  [(ngModel)]="startDate"
                  (change)="calculateDays()"
                  class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2"
                  >Return Date</label
                >
                <input
                  type="date"
                  [(ngModel)]="endDate"
                  (change)="calculateDays()"
                  class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <div class="text-sm text-gray-600">
                Duration: <span class="font-bold">{{ days }} days</span>
              </div>
            </div>

            <!-- Total -->
            <div class="bg-gray-100 p-4 rounded-lg">
              <div class="flex justify-between text-lg font-semibold">
                <span>Total:</span>
                <span class="text-blue-600">
                  $ {{ calculateTotal() | number : '1.2-2' }}
                </span>
              </div>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex gap-4 mt-6">
            <button
              (click)="closeModal()"
              class="flex-1 bg-gray-200 hover:bg-gray-300 text-gray-700 font-bold py-2 px-4 rounded-lg transition-colors"
            >
              Cancel
            </button>
            <button
              (click)="addToCart()"
              class="flex-1 bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-lg transition-colors"
            >
              {{
                selectedItem.itemType === 'vehicle' ? 'Reserve' : 'Add to Cart'
              }}
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class ProductListComponent implements OnInit {
  private productService = inject(ProductService);
  private vehicleService = inject(VehicleService);
  private cartService = inject(CartService);
  private toastService = inject(ToastService);
  private rentalService = inject(RentalService);
  private profileService = inject(ProfileService);
  private router = inject(Router);

  allItems: CatalogItem[] = [];
  filteredProducts$: Observable<CatalogItem[]> = of([]);
  loading = true;
  error: string | null = null;
  searchTerm = '';
  selectedFilter = 'all';

  selectedItem: CatalogItem | null = null;
  quantity = 1;
  days = 1;
  isRental = false;

  // Rental dates
  startDate: string = '';
  endDate: string = '';
  currentUserId: string | null = null;

  // Pagination
  currentPage = 1;
  itemsPerPage = 12;
  paginatedItems: CatalogItem[] = [];
  totalPages = 0;
  startIndex = 0;
  endIndex = 0;
  visiblePages: number[] = [];

  ngOnInit() {
    this.loadAll();
    this.loadUserProfile();
  }

  loadUserProfile() {
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        if (profile) this.currentUserId = profile.id;
      },
      error: () => console.log('User not logged in or error loading profile'),
    });
  }

  loadAll() {
    this.loading = true;
    this.error = null;

    forkJoin({
      products: this.productService.getAll().pipe(catchError(() => of([]))),
      vehicles: this.vehicleService.getAll().pipe(catchError(() => of([]))),
    }).subscribe({
      next: ({ products, vehicles }) => {
        const productItems: CatalogItem[] = products.map((p: Product) => ({
          id: p.id,
          name: p.name,
          description: p.description,
          price: p.price,
          photoUrl: p.photoUrl || '',
          stock: p.stock,
          categoryName: p.categoryName,
          itemType: 'product',
          originalItem: p,
        }));

        const vehicleItems: CatalogItem[] = vehicles.map((v: Vehicle) => ({
          id: v.id,
          name: `${v.brand} ${v.model} ${v.year}`,
          description: `${v.vehicleType} - ${v.licensePlate}`,
          price: v.dailyRate,
          photoUrl: v.imageUrl || '',
          stock: v.isAvailable() ? 1 : 0,
          categoryName: 'vehicles',
          itemType: 'vehicle',
          originalItem: v,
        }));

        this.allItems = [...productItems, ...vehicleItems];
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading catalog:', err);
        this.error = 'Could not load products and vehicles. Please try again.';
        this.loading = false;
      },
    });
  }

  onSearch() {
    this.applyFilters();
  }

  filterByType(type: string) {
    this.selectedFilter = type;
    this.applyFilters();
  }

  applyFilters() {
    let filtered = [...this.allItems];

    if (this.selectedFilter === 'products') {
      filtered = filtered.filter((item) => item.itemType === 'product');
    } else if (this.selectedFilter === 'vehicles') {
      filtered = filtered.filter((item) => item.itemType === 'vehicle');
    }

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(
        (item) =>
          item.name.toLowerCase().includes(term) ||
          item.description.toLowerCase().includes(term)
      );
    }

    this.filteredProducts$ = of(filtered);
    this.updatePagination(filtered);
  }

  updatePagination(items: CatalogItem[]) {
    this.totalPages = Math.ceil(items.length / this.itemsPerPage);
    if (this.currentPage > this.totalPages && this.totalPages > 0) {
      this.currentPage = this.totalPages;
    }
    if (this.currentPage < 1) {
      this.currentPage = 1;
    }

    this.startIndex = (this.currentPage - 1) * this.itemsPerPage;
    this.endIndex = Math.min(this.startIndex + this.itemsPerPage, items.length);
    this.paginatedItems = items.slice(this.startIndex, this.endIndex);

    // Calculate visible pages (show max 5 page numbers)
    this.visiblePages = [];
    const maxVisible = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let endPage = Math.min(this.totalPages, startPage + maxVisible - 1);

    if (endPage - startPage + 1 < maxVisible) {
      startPage = Math.max(1, endPage - maxVisible + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      this.visiblePages.push(i);
    }
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.applyFilters();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.applyFilters();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  goToPage(page: number) {
    this.currentPage = page;
    this.applyFilters();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  showAddToCartModal(item: CatalogItem) {
    this.selectedItem = item;
    this.quantity = 1;
    this.days = 1;
    this.isRental = false;

    if (item.itemType === 'vehicle') {
      // Initialize dates for vehicle rental (today and tomorrow)
      const today = new Date();
      const tomorrow = new Date(today);
      tomorrow.setDate(tomorrow.getDate() + 1);

      this.startDate = today.toISOString().split('T')[0];
      this.endDate = tomorrow.toISOString().split('T')[0];
      this.calculateDays();
    }
  }

  calculateDays() {
    if (!this.startDate || !this.endDate) return;

    const start = new Date(this.startDate);
    const end = new Date(this.endDate);

    // Calculate difference in milliseconds
    const diffTime = Math.abs(end.getTime() - start.getTime());
    // Convert to days (ceil to ensure at least 1 day if same day)
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    this.days = diffDays > 0 ? diffDays : 1;

    // Ensure end date is not before start date
    if (end < start) {
      this.days = 1;
      // Optionally reset end date to start date
    }
  }

  closeModal() {
    this.selectedItem = null;
  }

  calculateTotal(): number {
    if (!this.selectedItem) return 0;
    const multiplier =
      this.isRental || this.selectedItem.itemType === 'vehicle' ? this.days : 1;
    return this.selectedItem.price * this.quantity * multiplier;
  }

  addToCart() {
    if (!this.selectedItem) return;

    if (this.selectedItem.itemType === 'product') {
      this.cartService.addToCart(
        this.selectedItem.originalItem as Product,
        this.quantity,
        this.isRental,
        this.days
      );
      this.toastService.success(`${this.selectedItem.name} added to cart`);
    } else {
      // Vehicle Rental Logic
      if (!this.currentUserId) {
        this.toastService.error('You must be logged in to rent a vehicle');
        return;
      }

      const rentalData = {
        customerId: this.currentUserId,
        vehicleId: this.selectedItem.id,
        startDate: new Date(this.startDate).toISOString(),
        estimatedReturnDate: new Date(this.endDate).toISOString(),
        rentalPeriodType: 'Daily',
        rentalRate: this.selectedItem.price,
        deposit: 0,
        paymentMethod: 1, // Card
        pickupLocation: 'Main Office',
        notes: 'Online reservation via Web App',
      };

      this.rentalService.createRental(rentalData).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.toastService.success('Vehicle reserved successfully! 🚜');
            this.closeModal();
            // Redirect to orders page
            this.router.navigate(['/orders']);
          } else {
            this.toastService.error('Could not reserve vehicle');
          }
        },
        error: (err) => {
          console.error(err);
          this.toastService.error(
            err.error?.message || 'Error creating rental'
          );
        },
      });
      return; // Don't close modal immediately, wait for response
    }

    this.closeModal();
  }

  getStockLabel(item: CatalogItem): string {
    if (item.itemType === 'vehicle') {
      return item.stock > 0 ? 'Available' : 'Not available';
    }
    return item.stock > 0 ? `Stock: ${item.stock}` : 'Out of stock';
  }

  getStockBadgeClass(item: CatalogItem): string {
    if (item.stock === 0) return 'bg-red-500';
    if (item.itemType === 'vehicle') return 'bg-green-500';
    return item.stock > 10 ? 'bg-green-500' : 'bg-yellow-500';
  }

  onImageError(event: any) {
    event.target.src =
      'https://via.placeholder.com/400x300/e5e7eb/6b7280?text=Sin+Imagen';
  }
}

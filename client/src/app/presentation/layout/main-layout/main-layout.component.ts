import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  Router,
  RouterLink,
  RouterOutlet,
  RouterLinkActive,
} from '@angular/router';
import { AuthService } from '@application/services/auth.service';
import { TokenService } from '@application/services/token.service';
import { CartService } from '@application/services/cart.service';
import { ProfileService } from '@application/services/profile.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet, RouterLinkActive],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Sidebar/Navigation -->
      <div class="flex h-screen overflow-hidden">
        <!-- Sidebar -->
        <aside
          class="hidden md:flex md:flex-col w-64 bg-slate-800 text-white transition-all duration-300"
        >
          <div
            class="p-6 flex items-center justify-center border-b border-slate-700"
          >
            <div
              class="text-2xl font-bold text-orange-500 flex items-center gap-2"
            >
              <span>🏗️</span> Firmness
            </div>
          </div>

          <nav class="flex-1 px-4 py-6 space-y-2">
            <a
              routerLink="/dashboard"
              routerLinkActive="bg-slate-700 text-white"
              [routerLinkActiveOptions]="{ exact: true }"
              class="flex items-center px-4 py-3 text-slate-300 hover:bg-slate-700 hover:text-white rounded-lg transition-colors"
            >
              <span class="mr-3">📊</span> Dashboard
            </a>
            <a
              routerLink="/products"
              routerLinkActive="bg-slate-700 text-white"
              class="flex items-center px-4 py-3 text-slate-300 hover:bg-slate-700 hover:text-white rounded-lg transition-colors"
            >
              <span class="mr-3">🏗️</span> Catalog
            </a>
            <a
              routerLink="/cart"
              routerLinkActive="bg-slate-700 text-white"
              class="flex items-center px-4 py-3 text-slate-300 hover:bg-slate-700 hover:text-white rounded-lg transition-colors"
            >
              <span class="mr-3">🛒</span> Shopping Cart
              <span
                *ngIf="cartItemCount > 0"
                class="ml-auto bg-orange-500 text-white text-xs font-bold px-2 py-1 rounded-full"
              >
                {{ cartItemCount }}
              </span>
            </a>
            <a
              routerLink="/orders"
              routerLinkActive="bg-slate-700 text-white"
              class="flex items-center px-4 py-3 text-slate-300 hover:bg-slate-700 hover:text-white rounded-lg transition-colors"
            >
              <span class="mr-3">📦</span> My Orders
            </a>
            <a
              routerLink="/profile"
              routerLinkActive="bg-slate-700 text-white"
              class="flex items-center px-4 py-3 text-slate-300 hover:bg-slate-700 hover:text-white rounded-lg transition-colors"
            >
              <span class="mr-3">👤</span> Profile
            </a>
          </nav>

          <div class="p-4 border-t border-slate-700">
            <button
              (click)="logout()"
              class="flex items-center w-full px-4 py-2 text-red-400 hover:bg-slate-700 hover:text-red-300 rounded-lg transition-colors"
            >
              <span class="mr-3">🚪</span> Logout 
            </button>
          </div>
        </aside>

        <!-- Main Content -->
        <div class="flex-1 flex flex-col overflow-hidden">
          <!-- Header -->
          <header class="bg-white shadow-sm z-10">
            <div class="px-6 py-4 flex justify-between items-center">
              <h2 class="text-xl font-semibold text-gray-800">
                Client Panel
              </h2>

              <div class="flex items-center space-x-4">
                <a
                  routerLink="/cart"
                  class="relative p-2 text-gray-500 hover:text-gray-700 transition-colors"
                >
                  <span class="text-2xl">🛒</span>
                  <span
                    *ngIf="cartItemCount > 0"
                    class="absolute top-0 right-0 h-5 w-5 bg-red-500 text-white text-xs flex items-center justify-center rounded-full"
                  >
                    {{ cartItemCount }}
                  </span>
                </a>

                <div class="relative">
                  <span
                    class="absolute top-0 right-0 h-2 w-2 bg-red-500 rounded-full"
                  ></span>
                  <button
                    class="p-2 text-gray-500 hover:text-gray-700 transition-colors"
                  >
                    🔔
                  </button>
                </div>
                <div
                  class="flex items-center space-x-3 pl-4 border-l border-gray-200"
                >
                  <div class="text-right hidden sm:block">
                    <div class="text-sm font-medium text-gray-900">
                      {{ userName }}
                    </div>
                    <div class="text-xs text-gray-500">Client</div>
                  </div>
                  <div
                    class="h-10 w-10 bg-gradient-to-br from-orange-100 to-orange-200 text-orange-600 rounded-full flex items-center justify-center font-bold shadow-sm border border-orange-100 overflow-hidden"
                  >
                    <img
                      *ngIf="userPhotoUrl; else initials"
                      [src]="userPhotoUrl"
                      alt="Profile"
                      class="h-full w-full object-cover"
                    />
                    <ng-template #initials>
                      {{ userInitials }}
                    </ng-template>
                  </div>
                </div>
              </div>
            </div>
          </header>

          <!-- Content Scrollable Area -->
          <main class="flex-1 overflow-x-hidden overflow-y-auto bg-gray-50 p-6">
            <router-outlet></router-outlet>
          </main>
        </div>
      </div>
    </div>
  `,
})
export class MainLayoutComponent implements OnInit {
  private authService = inject(AuthService);
  private tokenService = inject(TokenService);
  private cartService = inject(CartService);
  private profileService = inject(ProfileService); // Inject ProfileService
  private router = inject(Router);

  userName: string = 'Usuario';
  userInitials: string = 'U';
  userPhotoUrl: string | null = null; // Add userPhotoUrl property
  cartItemCount: number = 0;

  ngOnInit() {
    const name = this.tokenService.getUsername();
    if (name) {
      this.userName = name;
      this.userInitials = name.substring(0, 2).toUpperCase();
    }

    // Fetch profile to get photo
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        console.log('MainLayout: Profile loaded', profile);
        if (profile && profile.photoUrl) {
          console.log('MainLayout: Setting photoUrl', profile.photoUrl);
          this.userPhotoUrl = profile.photoUrl;
        } else {
          console.log('MainLayout: No photoUrl found');
        }
      },
      error: (err) => {
        console.error('MainLayout: Error loading profile', err);
      },
    });

    // Subscribe to cart items to update count
    this.cartService.items$.subscribe((items) => {
      this.cartItemCount = items.reduce(
        (count, item) => count + item.quantity,
        0
      );
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

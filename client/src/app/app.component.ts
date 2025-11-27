import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  Router,
  RouterOutlet,
  RouterLink,
  RouterLinkActive,
  NavigationEnd,
} from '@angular/router';
import { filter } from 'rxjs/operators';

import { ToastComponent } from './presentation/shared/components/toast/toast.component';
import { ConfirmationModalComponent } from './presentation/shared/components/confirmation-modal/confirmation-modal.component';
import { ChatbotComponent } from './presentation/shared/components/chatbot/chatbot.component';

/**
 * Root App Component
 * Handles conditional rendering of header/footer based on route
 */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    ToastComponent,
    ConfirmationModalComponent,
    ChatbotComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'Firmness - Construction Rental';
  showLayout = false;

  constructor(private router: Router) {
    // Listen to route changes to show/hide layout
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        // Show layout only for authenticated routes
        // Hide global layout for:
        // 1. Public pages that have their own layout (Home, Login, Register)
        // 2. Protected pages that use MainLayout (Dashboard, Products, etc.)
        const hiddenLayoutRoutes = ['/', '/login', '/register'];
        const protectedRoutesPrefixes = [
          '/dashboard',
          '/products',
          '/vehicles',
          '/cart',
          '/orders',
          '/profile',
        ];

        const url = event.urlAfterRedirects.split('?')[0]; // Ignore query params
        const isHiddenRoute = hiddenLayoutRoutes.includes(url);
        const isProtectedRoute = protectedRoutesPrefixes.some((prefix) =>
          url.startsWith(prefix)
        );

        this.showLayout = !isHiddenRoute && !isProtectedRoute;
      });
  }
}

import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from '@application/services/toast.service';
import { animate, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  animations: [
    trigger('toastAnimation', [
      transition(':enter', [
        style({ transform: 'translateX(100%)', opacity: 0 }),
        animate(
          '300ms ease-out',
          style({ transform: 'translateX(0)', opacity: 1 })
        ),
      ]),
      transition(':leave', [
        animate(
          '200ms ease-in',
          style({ transform: 'translateX(100%)', opacity: 0 })
        ),
      ]),
    ]),
  ],
  styles: [
    `
      @keyframes slideIn {
        from {
          transform: translateX(100%);
          opacity: 0;
        }
        to {
          transform: translateX(0);
          opacity: 1;
        }
      }

      @keyframes slideOut {
        from {
          transform: translateX(0);
          opacity: 1;
        }
        to {
          transform: translateX(100%);
          opacity: 0;
        }
      }

      .toast-enter {
        animation: slideIn 300ms ease-out forwards;
      }

      .toast-leave {
        animation: slideOut 200ms ease-in forwards;
      }
    `,
  ],
  template: `
    <div
      class="fixed top-4 right-4 z-50 flex flex-col gap-2 w-full max-w-sm pointer-events-none"
    >
      <div
        *ngFor="let toast of toasts$ | async"
        [@toastAnimation]
        class="toast-enter pointer-events-auto shadow-lg rounded-lg p-4 mb-2 border-l-4 flex items-start justify-between transform transition-all duration-300 hover:scale-105"
        [ngClass]="getToastClasses(toast.type)"
      >
        <div class="flex items-center">
          <!-- Icon based on type -->
          <span class="mr-3 text-xl">
            <ng-container [ngSwitch]="toast.type">
              <span *ngSwitchCase="'success'">✅</span>
              <span *ngSwitchCase="'error'">❌</span>
              <span *ngSwitchCase="'warning'">⚠️</span>
              <span *ngSwitchCase="'info'">ℹ️</span>
            </ng-container>
          </span>

          <p class="font-medium text-sm">{{ toast.message }}</p>
        </div>

        <button
          (click)="remove(toast.id)"
          class="ml-4 text-gray-400 hover:text-gray-600 focus:outline-none"
        >
          <span class="text-lg">&times;</span>
        </button>
      </div>
    </div>
  `,
})
export class ToastComponent {
  private toastService = inject(ToastService);
  toasts$ = this.toastService.toasts$;

  remove(id: number) {
    this.toastService.remove(id);
  }

  getToastClasses(type: string): string {
    switch (type) {
      case 'success':
        return 'bg-white border-green-500 text-gray-800';
      case 'error':
        return 'bg-white border-red-500 text-gray-800';
      case 'warning':
        return 'bg-white border-orange-500 text-gray-800';
      case 'info':
        return 'bg-white border-blue-500 text-gray-800';
      default:
        return 'bg-white border-gray-500 text-gray-800';
    }
  }
}

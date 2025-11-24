import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ConfirmationService,
  ConfirmationConfig,
} from '@application/services/confirmation.service';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-confirmation-modal',
  standalone: true,
  imports: [CommonModule],
  animations: [
    trigger('modalAnimation', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('200ms ease-out', style({ opacity: 1 })),
      ]),
      transition(':leave', [animate('150ms ease-in', style({ opacity: 0 }))]),
    ]),
    trigger('dialogAnimation', [
      transition(':enter', [
        style({ transform: 'scale(0.9)', opacity: 0 }),
        animate('200ms ease-out', style({ transform: 'scale(1)', opacity: 1 })),
      ]),
      transition(':leave', [
        animate(
          '150ms ease-in',
          style({ transform: 'scale(0.9)', opacity: 0 })
        ),
      ]),
    ]),
  ],
  template: `
    <div
      *ngIf="confirmation$ | async as config"
      @modalAnimation
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50 backdrop-blur-sm"
      (click)="onBackdropClick()"
    >
      <div
        @dialogAnimation
        class="bg-white rounded-2xl shadow-2xl max-w-md w-full overflow-hidden"
        (click)="$event.stopPropagation()"
      >
        <!-- Header with Icon -->
        <div [ngClass]="getHeaderClass(config.type)" class="p-6 text-center">
          <div class="text-6xl mb-3 animate-bounce">{{ config.icon }}</div>
          <h2 class="text-2xl font-bold text-white">{{ config.title }}</h2>
        </div>

        <!-- Content -->
        <div class="p-6">
          <p class="text-gray-700 text-center text-lg leading-relaxed">
            {{ config.message }}
          </p>
        </div>

        <!-- Actions -->
        <div class="px-6 pb-6 flex gap-3">
          <button
            (click)="onCancel()"
            class="flex-1 px-6 py-3 bg-gray-100 hover:bg-gray-200 text-gray-700 font-semibold rounded-lg transition-all duration-200 transform hover:scale-105"
          >
            {{ config.cancelText }}
          </button>
          <button
            (click)="onConfirm()"
            [ngClass]="getButtonClass(config.type)"
            class="flex-1 px-6 py-3 text-white font-semibold rounded-lg transition-all duration-200 transform hover:scale-105 shadow-lg"
          >
            {{ config.confirmText }}
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      :host {
        display: contents;
      }

      @keyframes bounce {
        0%,
        100% {
          transform: translateY(0);
        }
        50% {
          transform: translateY(-10px);
        }
      }

      .animate-bounce {
        animation: bounce 1s ease-in-out infinite;
      }
    `,
  ],
})
export class ConfirmationModalComponent {
  private confirmationService = inject(ConfirmationService);
  confirmation$ = this.confirmationService.confirmation$;

  onConfirm() {
    this.confirmationService.handleResponse(true);
  }

  onCancel() {
    this.confirmationService.handleResponse(false);
  }

  onBackdropClick() {
    this.onCancel();
  }

  getHeaderClass(type?: string): string {
    switch (type) {
      case 'danger':
        return 'bg-gradient-to-r from-red-500 to-red-600';
      case 'warning':
        return 'bg-gradient-to-r from-orange-500 to-orange-600';
      case 'info':
        return 'bg-gradient-to-r from-blue-500 to-blue-600';
      default:
        return 'bg-gradient-to-r from-orange-500 to-orange-600';
    }
  }

  getButtonClass(type?: string): string {
    switch (type) {
      case 'danger':
        return 'bg-gradient-to-r from-red-500 to-red-600 hover:from-red-600 hover:to-red-700';
      case 'warning':
        return 'bg-gradient-to-r from-orange-500 to-orange-600 hover:from-orange-600 hover:to-orange-700';
      case 'info':
        return 'bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700';
      default:
        return 'bg-gradient-to-r from-orange-500 to-orange-600 hover:from-orange-600 hover:to-orange-700';
    }
  }
}

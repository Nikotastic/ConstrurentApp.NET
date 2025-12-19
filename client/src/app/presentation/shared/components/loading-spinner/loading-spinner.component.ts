import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Loading Spinner Component
 * Reusable loading indicator
 */
@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="spinner-container" [class.fullscreen]="fullscreen">
      <div class="spinner" [style.width.px]="size" [style.height.px]="size">
        <div class="spinner-border" [style.color]="color" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
      @if (message) {
      <p class="spinner-message">{{ message }}</p>
      }
    </div>
  `,
  styles: [
    `
      .spinner-container {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 2rem;
      }

      .spinner-container.fullscreen {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(255, 255, 255, 0.9);
        z-index: 9999;
      }

      .spinner {
        display: flex;
        align-items: center;
        justify-content: center;
      }

      .spinner-border {
        border: 3px solid currentColor;
        border-right-color: transparent;
        border-radius: 50%;
        width: 100%;
        height: 100%;
        animation: spin 0.75s linear infinite;
      }

      @keyframes spin {
        to {
          transform: rotate(360deg);
        }
      }

      .spinner-message {
        margin-top: 1rem;
        color: #666;
        font-size: 0.9rem;
      }
    `,
  ],
})
export class LoadingSpinnerComponent {
  @Input() size: number = 40;
  @Input() color: string = '#007bff';
  @Input() message?: string;
  @Input() fullscreen: boolean = false;
}

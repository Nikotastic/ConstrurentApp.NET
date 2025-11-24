import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export type AlertType = 'success' | 'error' | 'warning' | 'info';

/**
 * Alert Component
 * Reusable alert/notification component
 */
@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="alert alert-{{ type }}"
      [class.alert-dismissible]="dismissible"
      role="alert"
    >
      <div class="alert-content">
        <span class="alert-icon">{{ getIcon() }}</span>
        <span class="alert-message">{{ message }}</span>
      </div>
      @if (dismissible) {
      <button
        type="button"
        class="btn-close"
        (click)="onDismiss()"
        aria-label="Close"
      ></button>
      }
    </div>
  `,
  styles: [
    `
      .alert {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 1rem 1.25rem;
        margin-bottom: 1rem;
        border: 1px solid transparent;
        border-radius: 0.375rem;
      }

      .alert-content {
        display: flex;
        align-items: center;
        gap: 0.75rem;
      }

      .alert-icon {
        font-size: 1.25rem;
      }

      .alert-success {
        color: #0f5132;
        background-color: #d1e7dd;
        border-color: #badbcc;
      }

      .alert-error {
        color: #842029;
        background-color: #f8d7da;
        border-color: #f5c2c7;
      }

      .alert-warning {
        color: #664d03;
        background-color: #fff3cd;
        border-color: #ffecb5;
      }

      .alert-info {
        color: #055160;
        background-color: #cff4fc;
        border-color: #b6effb;
      }

      .btn-close {
        background: transparent;
        border: none;
        font-size: 1.5rem;
        font-weight: 700;
        line-height: 1;
        color: #000;
        opacity: 0.5;
        cursor: pointer;
        padding: 0.25rem;
      }

      .btn-close:hover {
        opacity: 0.75;
      }
    `,
  ],
})
export class AlertComponent {
  @Input() type: AlertType = 'info';
  @Input() message: string = '';
  @Input() dismissible: boolean = true;
  @Output() dismissed = new EventEmitter<void>();

  getIcon(): string {
    const icons = {
      success: '✓',
      error: '✕',
      warning: '⚠',
      info: 'ℹ',
    };
    return icons[this.type];
  }

  onDismiss(): void {
    this.dismissed.emit();
  }
}

import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface ConfirmationConfig {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'danger' | 'warning' | 'info';
  icon?: string;
}

@Injectable({
  providedIn: 'root',
})
export class ConfirmationService {
  private confirmationSubject = new BehaviorSubject<ConfirmationConfig | null>(
    null
  );
  confirmation$ = this.confirmationSubject.asObservable();

  private resolveCallback: ((value: boolean) => void) | null = null;

  confirm(config: ConfirmationConfig): Promise<boolean> {
    return new Promise((resolve) => {
      this.resolveCallback = resolve;
      this.confirmationSubject.next({
        ...config,
        confirmText: config.confirmText || 'Confirmar',
        cancelText: config.cancelText || 'Cancelar',
        type: config.type || 'warning',
        icon: config.icon || '⚠️',
      });
    });
  }

  handleResponse(confirmed: boolean) {
    if (this.resolveCallback) {
      this.resolveCallback(confirmed);
      this.resolveCallback = null;
    }
    this.confirmationSubject.next(null);
  }

  close() {
    this.handleResponse(false);
  }
}

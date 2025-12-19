import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PurchaseItem {
  description: string;
  quantity: number;
  unitPrice: number;
}

export interface SimulatePurchaseRequest {
  customerName: string;
  customerEmail: string;
  totalAmount: number;
  items: PurchaseItem[];
  simulateFailure: boolean;
}

export interface PaymentResponse {
  success: boolean;
  message: string;
  data?: {
    invoiceNumber: string;
    transactionId: string;
    amount: number;
    processedAt: string;
    emailSent: boolean;
  };
  error?: string;
}

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private apiUrl = `${environment.apiUrl}/paymenttest`;

  constructor(private http: HttpClient) {}

  simulatePurchase(
    request: SimulatePurchaseRequest
  ): Observable<PaymentResponse> {
    return this.http.post<PaymentResponse>(
      `${this.apiUrl}/simulate-purchase`,
      request
    );
  }
}

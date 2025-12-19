// Order Service
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

// Sale Item Interface
export interface SaleItem {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineaTotal: number;
}

// Sale Interface
export interface Sale {
  id: string;
  customerId: string;
  customerName: string;
  saleDate: string;
  subtotal: number;
  discount: number;
  tax: number;
  totalAmount: number;
  paymentMethod: string;
  notes?: string;
  items: SaleItem[];
}

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/sales`;

  // Get sales by customer
  getSalesByCustomer(
    customerId: string
  ): Observable<{ isSuccess: boolean; data: Sale[] }> {
    return this.http.get<{ isSuccess: boolean; data: Sale[] }>(
      `${this.apiUrl}/customer/${customerId}`
    );
  }

  // Get sale by id
  getSaleById(id: string): Observable<{ isSuccess: boolean; data: Sale }> {
    return this.http.get<{ isSuccess: boolean; data: Sale }>(
      `${this.apiUrl}/${id}`
    );
  }
}

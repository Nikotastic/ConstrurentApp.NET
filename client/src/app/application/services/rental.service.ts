import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

// Vehicle Rental Interface
export interface VehicleRental {
  id: string;
  customerId: string;
  customerName: string;
  vehicleId: string;
  vehicleDisplayName: string;
  startDate: string;
  estimatedReturnDate: string;
  actualReturnDate?: string;
  status: string;
  totalAmount: number;
  pendingAmount: number;
  isOverdue: boolean;
  pickupLocation?: string;
  returnLocation?: string;
  durationInDays: number;
}

// Create Rental Data Transfer Object
export interface CreateRentalDto {
  customerId: string;
  vehicleId: string;
  startDate: string;
  estimatedReturnDate: string;
  rentalPeriodType: string; // "Daily", "Weekly", etc.
  rentalRate?: number;
  deposit?: number;
  paymentMethod?: number;
  pickupLocation?: string;
  returnLocation?: string;
  notes?: string;
}

// Rental Service
@Injectable({
  providedIn: 'root',
})
export class RentalService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/vehiclerentals`;

  // Get rentals by customer
  getRentalsByCustomer(
    customerId: string
  ): Observable<{ isSuccess: boolean; data: VehicleRental[] }> {
    return this.http.get<{ isSuccess: boolean; data: VehicleRental[] }>(
      `${this.apiUrl}/customer/${customerId}`
    );
  }

  // Get rental by id
  getRentalById(
    id: string
  ): Observable<{ isSuccess: boolean; data: VehicleRental }> {
    return this.http.get<{ isSuccess: boolean; data: VehicleRental }>(
      `${this.apiUrl}/${id}`
    );
  }

  // Create rental
  createRental(
    rentalData: CreateRentalDto
  ): Observable<{
    isSuccess: boolean;
    data?: VehicleRental;
    message?: string;
  }> {
    return this.http.post<{
      isSuccess: boolean;
      data?: VehicleRental;
      message?: string;
    }>(this.apiUrl, rentalData);
  }

  // Cancel rental
  cancelRental(
    rentalId: string,
    reason: string
  ): Observable<{
    isSuccess: boolean;
    data?: VehicleRental;
    message?: string;
  }> {
    return this.http.post<{
      isSuccess: boolean;
      data?: VehicleRental;
      message?: string;
    }>(`${this.apiUrl}/${rentalId}/cancel`, { cancellationReason: reason });
  }
}

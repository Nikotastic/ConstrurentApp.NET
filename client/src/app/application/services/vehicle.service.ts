import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, catchError, throwError } from 'rxjs';
import { Vehicle } from '../../domain/entities/vehicle.entity';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class VehicleService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/vehicles`;

  getAll(): Observable<Vehicle[]> {
    console.log('🚗 Fetching vehicles from:', this.apiUrl);
    return this.http.get<any>(this.apiUrl).pipe(
      map((response) => {
        console.log('📥 Raw vehicles response:', response);
        if (response && response.isSuccess && response.data) {
          const vehicles = Array.isArray(response.data)
            ? response.data.map((dto: any) => this.mapToEntity(dto))
            : [];
          console.log(' Vehicles mapped:', vehicles.length);
          return vehicles;
        }
        console.warn(' Vehicles response format invalid:', response);
        return [];
      }),
      catchError((error) => {
        console.error(' Error loading vehicles:', error);
        return throwError(() => error);
      })
    );
  }

  getAvailable(): Observable<Vehicle[]> {
    return this.http.get<any>(`${this.apiUrl}/available`).pipe(
      map((response) => {
        if (response && response.isSuccess && response.data) {
          return Array.isArray(response.data)
            ? response.data.map((dto: any) => this.mapToEntity(dto))
            : [];
        }
        return [];
      })
    );
  }

  private mapToEntity(dto: any): Vehicle {
    return new Vehicle(
      dto.id,
      dto.brand,
      dto.model,
      dto.year,
      dto.licensePlate,
      dto.vehicleType,
      dto.dailyRate,
      dto.hourlyRate,
      dto.weeklyRate,
      dto.monthlyRate,
      dto.imageUrl || '',
      dto.status || 'Available',
      dto.isActive ?? true
    );
  }
}

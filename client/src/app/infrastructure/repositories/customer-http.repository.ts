import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ICustomerRepository } from '../../domain/repositories/customer.repository.interface';
import { Customer } from '../../domain/entities/customer.entity';
import { CustomerMapper } from '../mappers/customer.mapper';
import { environment } from '../../../environments/environment';

/**
 * Customer HTTP Repository - Infrastructure Layer
 * Implements ICustomerRepository using HttpClient
 */
@Injectable({ providedIn: 'root' })
export class CustomerHttpRepository implements ICustomerRepository {
  private readonly apiUrl = `${environment.apiUrl}/customers`;

  constructor(
    private http: HttpClient,
    private mapper: CustomerMapper
  ) {}

  getAll(): Observable<Customer[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(dtos => dtos.map(dto => this.mapper.toEntity(dto)))
    );
  }

  getById(id: string): Observable<Customer> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(dto => this.mapper.toEntity(dto))
    );
  }

  create(customer: Customer): Observable<Customer> {
    const dto = this.mapper.toDTO(customer);
    return this.http.post<any>(this.apiUrl, dto).pipe(
      map(dto => this.mapper.toEntity(dto))
    );
  }

  update(id: string, customer: Customer): Observable<Customer> {
    const dto = this.mapper.toDTO(customer);
    return this.http.put<any>(`${this.apiUrl}/${id}`, dto).pipe(
      map(dto => this.mapper.toEntity(dto))
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}


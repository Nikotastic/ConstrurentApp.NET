import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer } from '../../../domain/entities/customer.entity';
import { ICustomerRepository, CUSTOMER_REPOSITORY_TOKEN } from '../../../domain/repositories/customer.repository.interface';

/**
 * Get Customers Use Case - Application Layer
 * Retrieves all customers from the system
 */
@Injectable({ providedIn: 'root' })
export class GetCustomersUseCase {
  constructor(
    @Inject(CUSTOMER_REPOSITORY_TOKEN) private repository: ICustomerRepository
  ) {}

  execute(): Observable<Customer[]> {
    return this.repository.getAll();
  }
}


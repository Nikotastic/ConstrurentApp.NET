import { Injectable, Inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { Customer } from '../../../domain/entities/customer.entity';
import {
  ICustomerRepository,
  CUSTOMER_REPOSITORY_TOKEN,
} from '../../../domain/repositories/customer.repository.interface';

/**
 * Create Customer Use Case - Application Layer
 * Creates a new customer with business validation
 */
@Injectable({ providedIn: 'root' })
export class CreateCustomerUseCase {
  constructor(
    @Inject(CUSTOMER_REPOSITORY_TOKEN) private repository: ICustomerRepository
  ) {}

  execute(customer: Customer): Observable<Customer> {
    // Business validation
    if (!customer.isDocumentValid()) {
      return throwError(
        () =>
          new Error(
            'Invalid document format. Must be between 6 and 20 characters.'
          )
      );
    }

    if (!customer.isEmailValid()) {
      return throwError(() => new Error('Invalid email format.'));
    }

    return this.repository.create(customer);
  }
}

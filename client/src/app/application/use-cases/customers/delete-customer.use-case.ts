import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ICustomerRepository, CUSTOMER_REPOSITORY_TOKEN } from '../../../domain/repositories/customer.repository.interface';

/**
 * Delete Customer Use Case - Application Layer
 * Deletes a customer from the system
 */
@Injectable({ providedIn: 'root' })
export class DeleteCustomerUseCase {
  constructor(
    @Inject(CUSTOMER_REPOSITORY_TOKEN) private repository: ICustomerRepository
  ) {}

  execute(id: string): Observable<void> {
    return this.repository.delete(id);
  }
}


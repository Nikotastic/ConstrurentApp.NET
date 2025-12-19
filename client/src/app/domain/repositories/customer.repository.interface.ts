import { Observable } from 'rxjs';
import { Customer } from '../entities/customer.entity';

/**
 * Customer Repository Interface (Port) - Domain Layer
 * Defines the contract for customer data access
 * Implementation will be in Infrastructure layer
 */
export abstract class ICustomerRepository {
  abstract getAll(): Observable<Customer[]>;
  abstract getById(id: string): Observable<Customer>;
  abstract create(customer: Customer): Observable<Customer>;
  abstract update(id: string, customer: Customer): Observable<Customer>;
  abstract delete(id: string): Observable<void>;
}

// Injection token for DI
export const CUSTOMER_REPOSITORY_TOKEN = 'ICustomerRepository';


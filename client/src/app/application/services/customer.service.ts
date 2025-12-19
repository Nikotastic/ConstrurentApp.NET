import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer } from '../../domain/entities/customer.entity';
import { GetCustomersUseCase } from '../use-cases/customers/get-customers.use-case';
import { CreateCustomerUseCase } from '../use-cases/customers/create-customer.use-case';
import { DeleteCustomerUseCase } from '../use-cases/customers/delete-customer.use-case';


 // Customer Service - Application Layer
 // Orchestrates customer-related use cases

@Injectable({ providedIn: 'root' })
export class CustomerService {
  constructor(
    private getCustomersUseCase: GetCustomersUseCase,
    private createCustomerUseCase: CreateCustomerUseCase,
    private deleteCustomerUseCase: DeleteCustomerUseCase
  ) {}

  /**
   * Get all customers
   */
  getAllCustomers(): Observable<Customer[]> {
    return this.getCustomersUseCase.execute();
  }

  /**
   * Create a new customer
   */
  createCustomer(customer: Customer): Observable<Customer> {
    return this.createCustomerUseCase.execute(customer);
  }

  /**
   * Delete a customer
   */
  deleteCustomer(id: string): Observable<void> {
    return this.deleteCustomerUseCase.execute(id);
  }
}


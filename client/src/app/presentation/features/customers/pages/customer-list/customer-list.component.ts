import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Customer } from '../../../../../domain/entities/customer.entity';
import { CustomerService } from '../../../../../application/services/customer.service';
import { ToastService } from '../../../../../application/services/toast.service';
import { ConfirmationService } from '../../../../../application/services/confirmation.service';

/**
 * Customer List Component - Presentation Layer
 * Displays list of customers
 */
@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss'],
})
export class CustomerListComponent implements OnInit {
  customers: Customer[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private customerService: CustomerService,
    private toastService: ToastService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.loading = true;
    this.error = null;

    this.customerService.getAllCustomers().subscribe({
      next: (customers) => {
        this.customers = customers;
        this.loading = false;
      },
      error: (error) => {
        this.error = error.message;
        this.loading = false;
        this.toastService.error(
          '❌ Error al cargar la lista de clientes. Por favor, recarga la página.'
        );
        console.error('Error loading customers:', error);
      },
    });
  }

  async deleteCustomer(id: string) {
    const confirmed = await this.confirmationService.confirm({
      title: '¿Eliminar Cliente?',
      message:
        '¿Estás seguro de que deseas eliminar este cliente? Esta acción es permanente y no se puede deshacer.',
      confirmText: 'Sí, eliminar',
      cancelText: 'Cancelar',
      type: 'danger',
      icon: '⚠️',
    });

    if (confirmed) {
      this.customerService.deleteCustomer(id).subscribe({
        next: () => {
          this.toastService.success('✅ Cliente eliminado correctamente');
          this.loadCustomers(); // Reload list
        },
        error: (error) => {
          this.error = error.message;
          this.toastService.error(
            '❌ Error al eliminar el cliente. Por favor, intenta de nuevo.'
          );
          console.error('Error deleting customer:', error);
        },
      });
    }
  }

  getActiveCustomersCount(): number {
    return this.customers.filter((c) => c.isActive).length;
  }

  getInactiveCustomersCount(): number {
    return this.customers.filter((c) => !c.isActive).length;
  }
}

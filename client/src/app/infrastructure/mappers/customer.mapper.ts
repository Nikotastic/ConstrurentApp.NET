import { Injectable } from '@angular/core';
import { Customer } from '../../domain/entities/customer.entity';

/**
 * Customer Mapper - Infrastructure Layer
 * Converts between API DTOs and Domain Entities
 */
@Injectable({ providedIn: 'root' })
export class CustomerMapper {

   // Convert API DTO to Domain Entity

  toEntity(dto: any): Customer {
    return new Customer(
      dto.id,
      dto.email,
      dto.firstName,
      dto.lastName,
      dto.document,
      dto.phone,
      dto.address || '',
      dto.isActive ?? true,
      dto.createdAt ? new Date(dto.createdAt) : undefined
    );
  }


  // Convert Domain Entity to API DTO

  toDTO(entity: Customer): any {
    return {
      email: entity.email,
      firstName: entity.firstName,
      lastName: entity.lastName,
      document: entity.document,
      phone: entity.phone,
      address: entity.address,
      isActive: entity.isActive
    };
  }
}


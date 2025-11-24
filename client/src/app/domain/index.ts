/**
 * Domain Layer Exports
 * Central export point for domain entities, interfaces, and value objects
 */

// Entities
export * from './entities/customer.entity';
export * from './entities/product.entity';
export * from './entities/auth-user.entity';

// Repository Interfaces (Ports)
export * from './repositories/customer.repository.interface';
export * from './repositories/product.repository.interface';
export * from './repositories/auth.repository.interface';

// Value Objects
export * from './value-objects/login-credentials.vo';
export * from './value-objects/register-data.vo';

// Enums
export * from './enums/role.enum';

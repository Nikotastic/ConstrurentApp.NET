import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../entities/product.entity';

/**
 * Product Repository Interface (Port) - Domain Layer
 */
export abstract class IProductRepository {
  abstract getAll(): Observable<Product[]>;
  abstract getById(id: string): Observable<Product>;
  abstract create(product: Product): Observable<Product>;
  abstract update(id: string, product: Product): Observable<Product>;
  abstract delete(id: string): Observable<void>;
}

export const PRODUCT_REPOSITORY_TOKEN = new InjectionToken<IProductRepository>(
  'IProductRepository'
);

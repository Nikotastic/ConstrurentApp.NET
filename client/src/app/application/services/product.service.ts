import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../../domain/entities/product.entity';
import {
  PRODUCT_REPOSITORY_TOKEN,
  IProductRepository,
} from '../../domain/repositories/product.repository.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private repository = inject<IProductRepository>(PRODUCT_REPOSITORY_TOKEN);

  getAll(): Observable<Product[]> {
    return this.repository.getAll();
  }

  getById(id: string): Observable<Product> {
    return this.repository.getById(id);
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, catchError, throwError } from 'rxjs';
import { IProductRepository } from '../../domain/repositories/product.repository.interface';
import { Product } from '../../domain/entities/product.entity';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ProductHttpRepository implements IProductRepository {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/products`;

  getAll(): Observable<Product[]> {
    console.log('🔍 Fetching products from:', this.apiUrl);
    return this.http
      .get<any>(this.apiUrl)
      .pipe(
        map((response) => {
          console.log('📥 Raw response:', response);
          // API returns { isSuccess, data: { items: [] }, message }
          if (response && response.isSuccess && response.data && response.data.items) {
            const products = response.data.items.map((dto: any) => this.mapToEntity(dto));
            console.log(' Mapped products:', products.length);
            return products;
          }
          console.warn('Response format invalid:', response);
          return [];
        }),
        catchError((error) => {
          console.error('Error loading products:', error);
          return throwError(() => error);
        })
      );
  }

  getById(id: string): Observable<Product> {
    return this.http
      .get<any>(`${this.apiUrl}/${id}`)
      .pipe(
        map((response) => {
          if (response && response.isSuccess && response.data) {
            return this.mapToEntity(response.data);
          }
          throw new Error('Product not found');
        })
      );
  }

  create(product: Product): Observable<Product> {
    return this.http
      .post<any>(this.apiUrl, product)
      .pipe(map((dto) => this.mapToEntity(dto)));
  }

  update(id: string, product: Product): Observable<Product> {
    return this.http
      .put<any>(`${this.apiUrl}/${id}`, product)
      .pipe(map((dto) => this.mapToEntity(dto)));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  private mapToEntity(dto: any): Product {
    return new Product(
      dto.id,
      dto.name,
      dto.description,
      dto.price,
      dto.stock,
      dto.categoryId || '',
      dto.categoryName || 'Uncategorized',
      dto.imageUrl || dto.photoUrl || '',
      dto.isActive ?? true
    );
  }
}

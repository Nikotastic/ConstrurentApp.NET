import { Product } from '@domain/entities/product.entity';
import { Vehicle } from '@domain/entities/vehicle.entity';

export interface CatalogItem {
  id: string;
  name: string;
  description: string;
  price: number;
  photoUrl: string;
  stock: number;
  categoryName?: string;
  itemType: 'product' | 'vehicle';
  originalItem: Product | Vehicle;
}

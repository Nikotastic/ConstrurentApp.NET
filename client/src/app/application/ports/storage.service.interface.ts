import { InjectionToken } from '@angular/core';

/**
 * Storage Service Interface (Port) - Application Layer
 */
export abstract class IStorageService {
  abstract set(key: string, value: string): void;
  abstract get(key: string): string | null;
  abstract remove(key: string): void;
  abstract clear(): void;
}

export const STORAGE_SERVICE_TOKEN = new InjectionToken<IStorageService>(
  'IStorageService'
);

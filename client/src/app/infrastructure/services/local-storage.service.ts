import { Injectable } from '@angular/core';
import { IStorageService } from '../../application/ports/storage.service.interface';

/**
 * Local Storage Service - Infrastructure Layer
 * Implements IStorageService using browser's localStorage
 */
@Injectable({ providedIn: 'root' })
export class LocalStorageService implements IStorageService {
  set(key: string, value: string): void {
    try {
      localStorage.setItem(key, value);
    } catch (error) {
      console.error('Error saving to localStorage', error);
    }
  }

  get(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch (error) {
      console.error('Error reading from localStorage', error);
      return null;
    }
  }

  remove(key: string): void {
    try {
      localStorage.removeItem(key);
    } catch (error) {
      console.error('Error removing from localStorage', error);
    }
  }

  clear(): void {
    try {
      localStorage.clear();
    } catch (error) {
      console.error('Error clearing localStorage', error);
    }
  }
}

import { Pipe, PipeTransform } from '@angular/core';

/**
 * Currency Pipe
 * Formats numbers as currency (USD by default)
 */
@Pipe({
  name: 'appCurrency',
  standalone: true,
})
export class CurrencyPipe implements PipeTransform {
  transform(
    value: number | null | undefined,
    currencyCode: string = 'USD'
  ): string {
    if (value === null || value === undefined) {
      return '$0.00';
    }

    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currencyCode,
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value);
  }
}

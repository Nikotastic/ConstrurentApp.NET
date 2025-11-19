﻿namespace Firmness.Application.Interfaces;

public interface IExportService
{
    // Products
    Task<byte[]> ExportProductsToExcelAsync(Guid? categoryId = null);
    Task<byte[]> ExportProductsToPdfAsync(Guid? categoryId = null);
    
    // Customers
    Task<byte[]> ExportCustomersToExcelAsync();
    Task<byte[]> ExportCustomersToPdfAsync();
    
    // Sales
    Task<byte[]> ExportSalesToExcelAsync();
    Task<byte[]> ExportSalesToPdfAsync();
    
    // Vehicles
    Task<byte[]> ExportVehiclesToExcelAsync();
    Task<byte[]> ExportVehiclesToPdfAsync();
    
    // Vehicle Rentals
    Task<byte[]> ExportVehicleRentalsToExcelAsync();
    Task<byte[]> ExportVehicleRentalsToPdfAsync();
}

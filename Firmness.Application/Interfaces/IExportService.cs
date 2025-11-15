namespace Firmness.Application.Interfaces;

public interface IExportService
{

    // Export a product list to Excel

    Task<byte[]> ExportProductsToExcelAsync(Guid? categoryId = null);
    Task<byte[]> ExportCustomersToExcelAsync();
    Task<byte[]> ExportSalesToExcelAsync();
    Task<byte[]> ExportProductsToPdfAsync(Guid? categoryId = null);
    

    // Export clients to PDF

    Task<byte[]> ExportCustomersToPdfAsync();
    
    // Export sales to PDF
  
    Task<byte[]> ExportSalesToPdfAsync();
}

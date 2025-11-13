namespace Firmness.Application.Interfaces;

public interface IExportService
{
    /// <summary>
    /// Exporta una lista de productos a Excel
    /// </summary>
    Task<byte[]> ExportProductsToExcelAsync();
    
    /// <summary>
    /// Exporta una lista de clientes a Excel
    /// </summary>
    Task<byte[]> ExportCustomersToExcelAsync();
    
    /// <summary>
    /// Exporta una lista de ventas a Excel
    /// </summary>
    Task<byte[]> ExportSalesToExcelAsync();
    
    /// <summary>
    /// Exporta productos a PDF
    /// </summary>
    Task<byte[]> ExportProductsToPdfAsync();
    
    /// <summary>
    /// Exporta clientes a PDF
    /// </summary>
    Task<byte[]> ExportCustomersToPdfAsync();
    
    /// <summary>
    /// Exporta ventas a PDF
    /// </summary>
    Task<byte[]> ExportSalesToPdfAsync();
}


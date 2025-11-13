namespace Firmness.Application.Interfaces;

public interface IReceiptService
{
    /// <summary>
    /// Genera un recibo en PDF para una venta
    /// </summary>
    /// <param name="saleId">ID de la venta</param>
    /// <returns>Ruta del archivo generado</returns>
    Task<string> GenerateReceiptAsync(Guid saleId);
    
    /// <summary>
    /// Obtiene el contenido del recibo como byte array
    /// </summary>
    Task<byte[]> GetReceiptBytesAsync(Guid saleId);
    
    /// <summary>
    /// Verifica si existe un recibo para una venta
    /// </summary>
    bool ReceiptExists(Guid saleId);
    
    /// <summary>
    /// Obtiene la ruta del recibo
    /// </summary>
    string GetReceiptPath(Guid saleId);
}

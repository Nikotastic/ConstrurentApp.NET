namespace Firmness.Application.Models;

public class BulkImportResult
{
    public int TotalRows { get; set; }
    public int SuccessfulRows { get; set; }
    public int FailedRows { get; set; }
    public int ProductsCreated { get; set; }
    public int ProductsUpdated { get; set; }
    public int CustomersCreated { get; set; }
    public int CustomersUpdated { get; set; }
    public int SalesCreated { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public bool HasErrors => Errors.Any();
    public string Summary => $"Total: {TotalRows} | Exitosos: {SuccessfulRows} | Fallidos: {FailedRows}";
}

public class ImportError
{
    public int RowNumber { get; set; }
    public string Field { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string RowData { get; set; } = string.Empty;
    
    public override string ToString() => $"Fila {RowNumber} - {Field}: {ErrorMessage}";
}


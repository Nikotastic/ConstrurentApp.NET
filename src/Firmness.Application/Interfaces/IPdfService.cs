namespace Firmness.Application.Interfaces;

public interface IPdfService
{
    byte[] GenerateReceiptPdf(ReceiptData data);
    byte[] GenerateContractPdf(ContractData data);
}

public class ReceiptData
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public List<ReceiptItem> Items { get; set; } = new();
}

public class ReceiptItem
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
}

public class ContractData
{
    public string ContractNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string VehicleName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
}

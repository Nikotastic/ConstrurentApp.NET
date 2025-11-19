using Firmness.Domain.Enums;
namespace Firmness.Domain.Entities;

// class for sales
public class Sale : BaseEntity
{
    // properties
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public string? Notes { get; set; }
    public string? InvoiceNumber { get; set; }

    
    // navegation 
    public Customer Customer { get; set; } = null!;
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    

    public Sale() { }
    
    public Sale(Guid customerId)
    {
        CustomerId = customerId;
        Status = SaleStatus.Pending;
        PaymentMethod = PaymentMethod.Cash;
    }
}
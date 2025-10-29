namespace Firmness.Core.Entities;

// class for sales
public class Sale
{
    // properties
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    
    // navegation 
    public Customer Customer { get; set; } = null!;
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    
    public Sale() { }
    
    public Sale(Guid customerId)
    {
        CustomerId = customerId;
    }
}
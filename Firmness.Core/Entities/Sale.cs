namespace Firmness.Core.Entities;

// class for sales
public class Sale : BaseEntity
{
    // properties
    public Guid CustomerId { get; set; }
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
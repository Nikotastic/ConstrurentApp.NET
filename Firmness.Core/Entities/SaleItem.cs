namespace Firmness.Core.Entities;

// class for sale items
public class SaleItem
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
   
    public decimal LineaTotal => UnitPrice * Quantity;
    
    // navegation
    public Sale Sale { get; set; } = null!;
    public Product Product { get; set; } = null!;
    
    public SaleItem() { }
    
    // constructor with parameters
    public SaleItem(Guid saleId, Guid productId, int quantity, int unitPrice)
    {
        SaleId = saleId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
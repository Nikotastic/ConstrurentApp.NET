namespace Firmness.Core.Entities;

// class for sale items
public class SaleItem
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    // Fk
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    // business 
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
   
    public decimal LineaTotal => UnitPrice * Quantity;
    
    // navegation
    public Sale Sale { get; set; } = null!;
    public Product Product { get; set; } = null!;
    
    public SaleItem() { }
    
    // constructor with parameters
    public SaleItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (productId == Guid.Empty) throw new ArgumentException("productId is required", nameof(productId));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero");
        if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "UnitPrice cannot be negative");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = decimal.Round(unitPrice, 2);
    }

    // helper methods to update the item (keeps invariants)
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0) throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be greater than zero");
        Quantity = newQuantity;
    }

    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (newUnitPrice < 0) throw new ArgumentOutOfRangeException(nameof(newUnitPrice), "UnitPrice cannot be negative");
        UnitPrice = decimal.Round(newUnitPrice, 2);
    }

    public void AssignToSale(Guid saleId)
    {
        if (saleId == Guid.Empty) throw new ArgumentException("saleId is required", nameof(saleId));
        SaleId = saleId;
    }
}
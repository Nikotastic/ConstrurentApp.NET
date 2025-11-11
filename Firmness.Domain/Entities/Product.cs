namespace Firmness.Domain.Entities;

// class for products
public class Product
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    // SKU for stock keeping unit
    public string SKU { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = String.Empty;
    public decimal Stock { get; set; }

    // navegation
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    protected Product() { }
    
    // constructor with parameters
    public Product(string sku, string name, string description, decimal price, string imageUrl, decimal stock)
    {
        SKU = sku;
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        Stock = stock;
    }
}
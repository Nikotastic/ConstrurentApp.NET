namespace Firmness.Domain.Entities;


// Category of products

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
    
    protected Category() { }
    
    public Category(string name, string description = "")
    {
        Name = name;
        Description = description;
        IsActive = true;
    }
}


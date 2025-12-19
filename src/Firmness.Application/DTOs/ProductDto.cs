namespace Firmness.Application.DTOs;

// DTO for reading products - API (limited information for security)
public class ProductDto
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsActive { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? Cost { get; set; }
    public string? Barcode { get; set; }
}


// DTO for creating products - Admin only via API

public class CreateProductDto
{
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? Cost { get; set; }
    public string? Barcode { get; set; }
}


// DTO for updating products - Admin only via API
public class UpdateProductDto
{
    public string? SKU { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Stock { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsActive { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? Cost { get; set; }
    public string? Barcode { get; set; }
}

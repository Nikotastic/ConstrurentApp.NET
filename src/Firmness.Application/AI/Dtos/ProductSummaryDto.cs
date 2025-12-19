namespace Firmness.Application.AI.DTOs;

// Simplified DTO for AI responses about products

public class ProductSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Stock { get; set; } 
    public bool IsAvailable { get; set; }
    public string? Description { get; set; }
    public string? CategoryName { get; set; }
}
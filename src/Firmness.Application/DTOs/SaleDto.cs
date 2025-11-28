using Firmness.Domain.Enums;

namespace Firmness.Application.DTOs;


// DTO for reading sales - API
public class SaleDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public string? Notes { get; set; }
    public string? InvoiceNumber { get; set; }
}


// DTO for reading sale items - API
public class SaleItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}


// DTO to create sale - API
public class CreateSaleDto
{
    public Guid CustomerId { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = new();
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal Tax { get; set; } = 0;
    public string? Notes { get; set; }
}


// DTO to create sale item - API
public class CreateSaleItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}


// DTO to update sale - API
public class UpdateSaleDto
{
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Tax { get; set; }
    public string? Notes { get; set; }
    public string? InvoiceNumber { get; set; }
    public List<UpdateSaleItemDto>? Items { get; set; }
}


// DTO to update sale item - API
public class UpdateSaleItemDto
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

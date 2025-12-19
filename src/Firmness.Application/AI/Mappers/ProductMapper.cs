using Firmness.Application.AI.DTOs;
using Firmness.Domain.Entities;

namespace Firmness.Application.AI.Mappers;


// Mapper to convert Product to ProductSummaryDto


public static class ProductMapper
{
    public static ProductSummaryDto ToSummary(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        return new ProductSummaryDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            IsAvailable = product.IsActive && product.Stock > 0,
            Description = product.Description,
            CategoryName = product.Category?.Name
        };
    }

    public static List<ProductSummaryDto> ToSummaryList(IEnumerable<Product> products)
    {
        return products?.Select(ToSummary).ToList() ?? new List<ProductSummaryDto>();
    }
}
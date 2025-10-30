using Firmness.Application.Interfaces;
using Firmness.Core.Common;
using Firmness.Core.Entities;
using Firmness.Core.Interfaces;

namespace Firmness.Application.Services;

// service for products
public class ProductService : IProductService 
{
    // Inject repository
    private readonly IProductRepository _productRepository;
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // Crud operations
    public Task<Product?> GetByIdAsync(Guid id) => _productRepository.GetByIdAsync(id);

    public async Task<PaginatedResult<Product>> GetAllAsync(int page = 1, int pageSize = 50)
    {
        var (items, total) = await _productRepository.GetPagedAsync(page, pageSize, null);
        return new PaginatedResult<Product>(items, page, pageSize, total);
    }

    // Search products using query and return paginated result
    public async Task<PaginatedResult<Product>> SearchAsync(string? query, int page = 1, int pageSize = 50)
    {
        var (items, total) = await _productRepository.GetPagedAsync(page, pageSize, query);
        return new PaginatedResult<Product>(items, page, pageSize, total);
    }

    public async Task AddAsync(Product product)
    {
        if (product.Price < 0) throw new ArgumentOutOfRangeException(nameof(product.Price));
        if (string.IsNullOrWhiteSpace(product.Name)) throw new ArgumentException("Name required", nameof(product.Name));
        await _productRepository.AddAsync(product);
    }

    public Task UpdateAsync(Product product)
    {
        if (product.Price < 0) throw new ArgumentOutOfRangeException(nameof(product.Price));
        return _productRepository.UpdateAsync(product);
    }

    public Task DeleteAsync(Guid id) => _productRepository.DeleteAsync(id);
}
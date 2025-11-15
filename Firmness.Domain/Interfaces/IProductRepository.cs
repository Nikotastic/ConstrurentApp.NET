using Firmness.Domain.Entities;

namespace Firmness.Domain.Interfaces;

// interface for product repository
public interface IProductRepository
{
    // CRUD operations
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetAllWithCategoryAsync(); // Include Category
    Task<IEnumerable<Product>> GetAllWithCategoryAsync(Guid? categoryId); // Include Category and filter
    Task<IEnumerable<Product>> SearchAsync(string? query);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid product);
    Task<(IEnumerable<Product> Items, long Total)> GetPagedAsync(int page, int pageSize, string? query = null);
    Task<(IEnumerable<Product> Items, long Total)> GetPagedWithCategoryAsync(int page, int pageSize, string? query = null, Guid? categoryId = null);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}
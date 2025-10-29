using Firmness.Core.Entities;

namespace Firmness.Core.Interfaces;

// interface for product repository
public interface IProductRepository
{
    // CRUD operations
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> SearchAsync(string? query);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid product);
}
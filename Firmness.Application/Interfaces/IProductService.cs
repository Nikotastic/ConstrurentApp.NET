using Firmness.Core.Common;
using Firmness.Core.Entities;

namespace Firmness.Application.Interfaces;

public interface IProductService
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<PaginatedResult<Product>> GetAllAsync(int page = 1, int pageSize = 50);
    Task<PaginatedResult<Product>> SearchAsync(string? query, int page = 1, int pageSize = 50);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}
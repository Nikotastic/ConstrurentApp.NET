using Firmness.Domain.Common;
using Firmness.Domain.Entities;

namespace Firmness.Application.Interfaces;

public interface IProductService
{
    Task<Result<Product>> GetByIdAsync(Guid id);
    Task<Result<PaginatedResult<Product>>> GetAllAsync(int page = 1, int pageSize = 50);
    Task<Result<PaginatedResult<Product>>> SearchAsync(string? query, int page = 1, int pageSize = 50);
    Task<Result<PaginatedResult<Product>>> GetPagedWithCategoryAsync(int page, int pageSize, string? query = null, Guid? categoryId = null);
    Task<Result> AddAsync(Product? product);
    Task<Result> UpdateAsync(Product? product);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<long>> CountAsync(CancellationToken cancellationToken = default);
}
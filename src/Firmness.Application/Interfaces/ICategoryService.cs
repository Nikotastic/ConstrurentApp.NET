using Firmness.Domain.Common;
using Firmness.Domain.Entities;

namespace Firmness.Application.Interfaces;


// Service for Category entity
public interface ICategoryService
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<IPaginatedResult<Category>> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<Category>> GetActiveAsync();
    Task<Result<Category>> AddAsync(Category category);
    Task<Result> UpdateAsync(Category category);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<long>> CountAsync();
}


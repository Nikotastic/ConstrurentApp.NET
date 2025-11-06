using Firmness.Core.Common;
using Firmness.Core.Entities;

namespace Firmness.Core.Interfaces;

// interface for customer repository
public interface ICustomerRepository
{
    // Crud operations
    Task<Customer?> GetByIdAsync(Guid id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<IPaginatedResult<Customer>> GetAllAsync(int page, int pageSize);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Guid customer);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}


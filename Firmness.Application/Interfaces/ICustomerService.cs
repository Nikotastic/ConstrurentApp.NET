using Firmness.Domain.Common;
using Firmness.Domain.Entities;

namespace Firmness.Application.Interfaces;

public interface ICustomerService
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<IPaginatedResult<Customer>> GetAllAsync(int page, int pageSize);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Guid id);
    Task<Result<long>> CountAsync(CancellationToken cancellationToken = default);
}
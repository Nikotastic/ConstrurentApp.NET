using Firmness.Core.Entities;

namespace Firmness.Core.Interfaces;

// interface for sale repository
public interface ISaleRepository
{
    // CRUD operations
    Task<Sale?> GetByIdAsync(Guid id);
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId);
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task DeleteAsync(Guid sale);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}
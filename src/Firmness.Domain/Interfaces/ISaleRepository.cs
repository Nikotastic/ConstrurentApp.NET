using Firmness.Domain.Entities;

namespace Firmness.Domain.Interfaces;

// interface for sale repository
public interface ISaleRepository
{
    // CRUD operations
    Task<Sale?> GetByIdAsync(Guid id);
    Task<Sale?> GetByIdWithDetailsAsync(Guid id); // Include Customer and Items with Products
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<IEnumerable<Sale>> GetAllWithDetailsAsync(); // Include Customer and Items with Products
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId);
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task DeleteAsync(Guid sale);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}
using Firmness.Core.Entities;

namespace Firmness.Application.Interfaces;

public interface ISaleService
{
    Task<Sale> CreateSaleAsync(Guid customerId, IEnumerable<(Guid productId, int quantity)> lines);
    Task<Sale?> GetByIdAsync(Guid id);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}
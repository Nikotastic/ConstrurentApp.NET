using Firmness.Core.Entities;
using Firmness.Core.Common;

namespace Firmness.Application.Interfaces;

public interface ISaleService
{
    Task<Sale> CreateSaleAsync(Guid customerId, IEnumerable<(Guid productId, int quantity)> lines, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Result<long>> CountAsync(CancellationToken cancellationToken = default);
}
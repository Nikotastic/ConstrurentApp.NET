﻿using Firmness.Domain.Entities;
using Firmness.Domain.Common;

namespace Firmness.Application.Interfaces;

public interface ISaleService
{
    Task<Sale> CreateSaleAsync(Guid customerId, IEnumerable<(Guid productId, int quantity)> lines, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<long>> CountAsync(CancellationToken cancellationToken = default);
}
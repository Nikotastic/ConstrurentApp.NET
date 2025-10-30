using Firmness.Core.Entities;

namespace Firmness.Core.Interfaces;

public interface ISaleItemRepository
{
    Task<SaleItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<SaleItem>> GetBySaleIdAsync(Guid saleId);
    Task<IEnumerable<SaleItem>> GetByProductIdAsync(Guid productId);
    Task AddAsync(SaleItem saleItem);
    Task UpdateAsync(SaleItem saleItem);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
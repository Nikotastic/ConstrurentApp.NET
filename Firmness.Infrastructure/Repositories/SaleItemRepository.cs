using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

public class SaleItemRepository(ApplicationDbContext db) : ISaleItemRepository
{
    public async Task AddAsync(SaleItem saleItem)
    {
        if (saleItem == null) throw new ArgumentNullException(nameof(saleItem));
        await db.SaleItems.AddAsync(saleItem);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await db.SaleItems.FindAsync(id);
        if (entity == null) return;
        db.SaleItems.Remove(entity);
    }

    public async Task<IEnumerable<SaleItem>> GetByProductIdAsync(Guid productId)
    {
        return await db.SaleItems
            .AsNoTracking()
            .Where(si => si.ProductId == productId)
            .Include(si => si.Product)
            .ToListAsync();
    }

    public async Task<IEnumerable<SaleItem>> GetBySaleIdAsync(Guid saleId)
    {
        return await db.SaleItems
            .AsNoTracking()
            .Where(si => si.SaleId == saleId)
            .Include(si => si.Product)
            .ToListAsync();
    }

    public async Task<SaleItem?> GetByIdAsync(Guid id)
    {
        return await db.SaleItems
            .AsNoTracking()
            .Include(si => si.Product)
            .Include(si => si.Sale)
            .FirstOrDefaultAsync(si => si.Id == id);
    }

    public Task UpdateAsync(SaleItem saleItem)
    {
        if (saleItem == null) throw new ArgumentNullException(nameof(saleItem));
        db.SaleItems.Update(saleItem);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await db.SaleItems.AnyAsync(si => si.Id == id);
    }
}
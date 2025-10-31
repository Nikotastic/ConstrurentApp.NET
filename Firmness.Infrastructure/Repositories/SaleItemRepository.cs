using Firmness.Core.Entities;
using Firmness.Core.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

public class SaleItemRepository(ApplicationDbContext db) : ISaleItemRepository
{
    public async Task AddAsync(SaleItem saleItem)
    {
        await db.SaleItems.AddAsync(saleItem);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await db.SaleItems.FindAsync(id);
        if (entity == null) return;
        db.SaleItems.Remove(entity);
        await db.SaveChangesAsync();
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

    public async Task UpdateAsync(SaleItem saleItem)
    {
        db.SaleItems.Update(saleItem);
        await db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await db.SaleItems.AnyAsync(si => si.Id == id);
    }
}
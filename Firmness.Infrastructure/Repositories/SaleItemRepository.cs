using Firmness.Core.Entities;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

public class SaleItemRepository
{
    private readonly ApplicationDbContext _db;
    public SaleItemRepository(ApplicationDbContext db) => _db = db;

    public async Task AddAsync(SaleItem saleItem)
    {
        await _db.SaleItems.AddAsync(saleItem);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.SaleItems.FindAsync(id);
        if (entity == null) return;
        _db.SaleItems.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<SaleItem>> GetByProductIdAsync(Guid productId)
    {
        return await _db.SaleItems
            .AsNoTracking()
            .Where(si => si.ProductId == productId)
            .Include(si => si.Product)
            .ToListAsync();
    }

    public async Task<IEnumerable<SaleItem>> GetBySaleIdAsync(Guid saleId)
    {
        return await _db.SaleItems
            .AsNoTracking()
            .Where(si => si.SaleId == saleId)
            .Include(si => si.Product)
            .ToListAsync();
    }

    public async Task<SaleItem?> GetByIdAsync(Guid id)
    {
        return await _db.SaleItems
            .AsNoTracking()
            .Include(si => si.Product)
            .Include(si => si.Sale)
            .FirstOrDefaultAsync(si => si.Id == id);
    }

    public async Task UpdateAsync(SaleItem saleItem)
    {
        _db.SaleItems.Update(saleItem);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _db.SaleItems.AnyAsync(si => si.Id == id);
    }
}
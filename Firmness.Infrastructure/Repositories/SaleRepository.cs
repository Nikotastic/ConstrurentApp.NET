using Firmness.Core.Entities;
using Firmness.Core.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

// Implementation of ISaleRepository
public class SaleRepository(ApplicationDbContext db) : ISaleRepository
{
    // Add a new sale
    public async Task AddAsync(Sale sale)
    {
        if (sale == null) throw new ArgumentNullException(nameof(sale));
        await db.Sales.AddAsync(sale);
        await db.SaveChangesAsync();
    }
    
    // Update an existing sale
    public async Task UpdateAsync(Sale sale)
    {
        if (sale == null) throw new ArgumentNullException(nameof(sale));
        db.Sales.Update(sale);
        await db.SaveChangesAsync();
    }
    
    // Delete a sale by id
    public async Task DeleteAsync(Guid id)
    {
        var sale = await db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return;

        db.Sales.Remove(sale);
        await db.SaveChangesAsync();
    }
    
    // Get all sales
    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await db.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .Include(s => s.Customer)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
    
    // Get a sale by id
    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await db.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    // Get all sales by customer id
    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId)
    {
        return await db.Sales
            .AsNoTracking()
            .Where(s => s.CustomerId == customerId)
            .Include(s => s.Items)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    // Get all sales by date range
    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await db.Sales
            .AsNoTracking()
            .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
            .Include(s => s.Items)
            .Include(s => s.Customer)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
    
}
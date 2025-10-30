using Firmness.Core.Entities;
using Firmness.Core.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

// Implementation of ISaleRepository
public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _db;
    public SaleRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // Add a new sale
    public async Task AddAsync(Sale sale)
    {
        if (sale == null) throw new ArgumentNullException(nameof(sale));
        await _db.Sales.AddAsync(sale);
        await _db.SaveChangesAsync();
    }
    
    // Update an existing sale
    public async Task UpdateAsync(Sale sale)
    {
        if (sale == null) throw new ArgumentNullException(nameof(sale));
        _db.Sales.Update(sale);
        await _db.SaveChangesAsync();
    }
    
    // Delete a sale by id
    public async Task DeleteAsync(Guid id)
    {
        var sale = await _db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return;

        _db.Sales.Remove(sale);
        await _db.SaveChangesAsync();
    }
    
    // Get all sales
    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _db.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .Include(s => s.Customer)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
    
    // Get a sale by id
    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _db.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    // Get all sales by customer id
    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _db.Sales
            .AsNoTracking()
            .Where(s => s.CustomerId == customerId)
            .Include(s => s.Items)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    // Get all sales by date range
    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _db.Sales
            .AsNoTracking()
            .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
            .Include(s => s.Items)
            .Include(s => s.Customer)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
    
}
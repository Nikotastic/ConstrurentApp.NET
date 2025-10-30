using Firmness.Core.Entities;
using Firmness.Core.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

// Implementación del repositorio EF en Infrastructure
namespace Firmness.Infrastructure.Repositories;

// repository for products
public class ProductRepository(ApplicationDbContext db) : IProductRepository
{
    // constructor with dependency injection
    private readonly ApplicationDbContext _db = db;

    // CRUD operations
    public async Task<Product?> GetByIdAsync(Guid id)
        => await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    public Task<IEnumerable<Product>> GetAllAsync()
        => Task.FromResult<IEnumerable<Product>>(_db.Products.AsNoTracking().ToList());

    public Task<IEnumerable<Product>> SearchAsync(string? query)
    {
        // search by name or sku
        var q = _db.Products.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(p => p.Name.Contains(query) || p.SKU.Contains(query));
        return Task.FromResult<IEnumerable<Product>>(q.ToList());
    }

    public async Task AddAsync(Product product)
    {
        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p != null) { _db.Products.Remove(p); await _db.SaveChangesAsync(); }
    }
    
    // Implementation a method of paged
    public async Task<(IEnumerable<Product> Items, long Total)> GetPagedAsync(int page, int pageSize, string? query = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var q = _db.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            q = q.Where(p => p.Name.Contains(query) || p.SKU.Contains(query));
        }

        var total = await q.LongCountAsync();
        var items = await q
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}

using Firmness.Domain.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;


// Implementation of ICategoryRepository

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IPaginatedResult<Category>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalItems = await _context.Categories.LongCountAsync(cancellationToken);
        
        var items = await _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Category>(items, page, pageSize, totalItems);
    }

    public async Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetActiveSortedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories.LongCountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
    }
}

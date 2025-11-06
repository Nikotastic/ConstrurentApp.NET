using Firmness.Core.Common;
using Firmness.Core.Entities;
using Firmness.Core.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

// Implementation of ICustomerRepository
public class CustomerRepository(ApplicationDbContext db) : ICustomerRepository
{
    // Get a customer by id
    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }
    // Get a customer by identity user id
    public async Task<Customer?> GetByIdentityUserIdAsync(string identityUserId)
    {
        return await db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);
    }
    // Add a new customer
    
    public async Task<IPaginatedResult<Customer>> GetAllAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var query = db.Customers.AsNoTracking().OrderBy(c => c.FirstName);
        var total = await query.LongCountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Customer>(items, page, pageSize, total);
    }

    public async Task AddAsync(Customer customer)
    {
        
        if (customer is null) throw new ArgumentNullException(nameof(customer));
        await db.Customers.AddAsync(customer);
        await db.SaveChangesAsync();
    }

    // Delete a customer by id
    public async Task DeleteAsync(Guid id)
    {
        var c = await db.Customers.FindAsync(id);
        if (c == null) return;
        db.Customers.Remove(c);
        await db.SaveChangesAsync();
    }
    
    // Get all customers
    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await db.Customers.AsNoTracking().ToListAsync();
    }

    // Search customers by name or identity user id
    public async Task<IEnumerable<Customer>> SearchAsync(string? query)
    {
        var q = db.Customers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(c => c.IdentityUserId != null && (c.FirstName.Contains(query) || c.IdentityUserId.Contains(query)));
        return await q.ToListAsync();
    }
    // Update a customer
    public async Task UpdateAsync(Customer customer)
    {
        if (customer is null) throw new ArgumentNullException(nameof(customer));
        db.Customers.Update(customer);
        await db.SaveChangesAsync();
    }

    // Check if a customer exists by id
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await db.Customers.AnyAsync(c => c.Id == id);
    }
    
    // Search customers by name or identity user id
    public async Task<IEnumerable<Customer>> SearchByNameOrDocumentAsync(string? term, int page = 1, int pageSize = 50)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var q = db.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.Trim();
            q = q.Where(c =>
                c.Email != null && (EF.Functions.ILike(c.FirstName, $"%{term}%") ||
                                    EF.Functions.ILike(c.LastName, $"%{term}%") ||
                                    EF.Functions.ILike(c.Email, $"%{term}%")));
        }

        return await q
            .OrderBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await db.Customers.LongCountAsync(cancellationToken);
}
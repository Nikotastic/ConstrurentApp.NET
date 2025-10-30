using Firmness.Core.Entities;
using Firmness.Core.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Repositories;

// Implementation of ICustomerRepository
public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _db;
    public CustomerRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // Add a new customer
    public async Task AddAsync(Customer customer)
    {
        await _db.Customers.AddAsync(customer);
        await _db.SaveChangesAsync();
    }

    // Delete a customer by id
    public async Task DeleteAsync(Guid id)
    {
        var c = await _db.Customers.FindAsync(id);
        if (c == null) return;
        _db.Customers.Remove(c);
        await _db.SaveChangesAsync();
    }
    
    // Get all customers
    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _db.Customers.AsNoTracking().ToListAsync();
    }

    // Search customers by name or identity user id
    public async Task<IEnumerable<Customer>> SearchAsync(string? query)
    {
        var q = _db.Customers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(c => c.FirstName.Contains(query) || c.IdentityUserId.Contains(query));
        return await q.ToListAsync();
    }

    // Get a customer by id
    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }
    // Get a customer by identity user id
    public async Task<Customer?> GetByIdentityUserIdAsync(string identityUserId)
    {
        return await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);
    }
    // Update a customer
    public async Task UpdateAsync(Customer customer)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
    }

    // Check if a customer exists by id
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _db.Customers.AnyAsync(c => c.Id == id);
    }
    
    // Search customers by name or identity user id
    public async Task<IEnumerable<Customer>> SearchByNameOrDocumentAsync(string? term, int page = 1, int pageSize = 50)
    {
        var q = _db.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.Trim();
            q = q.Where(c =>
                EF.Functions.ILike(c.FirstName, $"%{term}%") ||
                EF.Functions.ILike(c.LastName, $"%{term}%") ||
                EF.Functions.ILike(c.Email, $"%{term}%"));
        }

        return await q
            .OrderBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    
    
}
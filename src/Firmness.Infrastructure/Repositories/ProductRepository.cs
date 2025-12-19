﻿using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

// Implementación del repositorio EF en Infrastructure
namespace Firmness.Infrastructure.Repositories;

// repository for products
public class ProductRepository(ApplicationDbContext db) : IProductRepository
{
    // CRUD operations
    public async Task<Product?> GetByIdAsync(Guid id)
        => await db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    public Task<IEnumerable<Product>> GetAllAsync()
        => Task.FromResult<IEnumerable<Product>>(db.Products.AsNoTracking().ToList());

    public Task<IEnumerable<Product>> SearchAsync(string? query)
    {
        // search by name or sku
        var q = db.Products.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(p => p.Name.Contains(query) || p.SKU.Contains(query));
        return Task.FromResult<IEnumerable<Product>>(q.ToList());
    }

    public async Task AddAsync(Product product)
    {
        await db.Products.AddAsync(product);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        // Detach any existing tracked entity with the same ID to avoid conflicts
        var existingEntry = db.ChangeTracker.Entries<Product>()
            .FirstOrDefault(e => e.Entity.Id == product.Id);
        
        if (existingEntry != null)
        {
            existingEntry.State = EntityState.Detached;
        }
        
        db.Products.Update(product);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var p = await db.Products.FindAsync(id);
        if (p != null) { db.Products.Remove(p); await db.SaveChangesAsync(); }
    }
    
    // Implementation a method of paged
    public async Task<(IEnumerable<Product> Items, long Total)> GetPagedAsync(int page, int pageSize, string? query = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var q = db.Products.AsNoTracking().AsQueryable();

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

    // Get all products with category
    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
    {
        return await db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .OrderBy(p => p.Category != null ? p.Category.Name : "")
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    // Get all products with category filtered by categoryId
    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync(Guid? categoryId)
    {
        var query = db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(p => p.Category != null ? p.Category.Name : "")
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    // Paged with category filter and includes
    public async Task<(IEnumerable<Product> Items, long Total)> GetPagedWithCategoryAsync(int page, int pageSize, string? query = null, Guid? categoryId = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var q = db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(query))
        {
            var searchTerm = query.Trim().ToLower();
            q = q.Where(p => 
                p.SKU.ToLower().Contains(searchTerm) ||
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm))
            );
        }

        // Apply category filter
        if (categoryId.HasValue)
        {
            q = q.Where(p => p.CategoryId == categoryId.Value);
        }

        var total = await q.LongCountAsync();
        var items = await q
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await db.Products.LongCountAsync(cancellationToken);
}

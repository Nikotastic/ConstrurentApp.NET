using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Data;

// DbContext for the application
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Domain DbSets - renamed Users -> DomainUsers to avoid collision with Identity
    public DbSet<User> DomainUsers { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Important: call Identity configuration first
        base.OnModelCreating(modelBuilder);

        // Treat Person as non-entity (ignore it). User and Customer will be independent entities.
        modelBuilder.Ignore<Person>();

        // Categories
        modelBuilder.Entity<Category>(b =>
        {
            b.ToTable("Category");
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).HasMaxLength(100).IsRequired();
            b.Property(c => c.Description).HasMaxLength(500);
            b.Property(c => c.IsActive).IsRequired();
            b.HasIndex(c => c.Name).IsUnique();
        });

        // Productos
        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("Product");
            b.HasKey(p => p.Id);
            b.Property(p => p.SKU).HasMaxLength(50).IsRequired();
            b.Property(p => p.Name).HasMaxLength(200).IsRequired();
            b.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(p => p.Stock).IsRequired();
            b.Property(p => p.ImageUrl).HasMaxLength(200).IsRequired();
            b.Property(p => p.Description).HasMaxLength(500).IsRequired();
            
            // Nuevas propiedades
            b.Property(p => p.IsActive).IsRequired();
            b.Property(p => p.MinStock).HasColumnType("decimal(18,2)");
            b.Property(p => p.Cost).HasColumnType("decimal(18,2)");
            b.Property(p => p.Barcode).HasMaxLength(50);
            
            // Relación con Category
            b.HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryId)
             .OnDelete(DeleteBehavior.SetNull);
            
            b.HasIndex(p => p.SKU).IsUnique();
            b.HasIndex(p => p.Barcode).IsUnique(false);
        });

        // Users (domain) mapped to table "User"
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("User"); // domain table
            b.HasKey(u => u.Id); // now allowed because Person is ignored
            b.Property(u => u.Email).HasMaxLength(200).IsRequired();
            b.Property(u => u.PasswordHash).HasMaxLength(200).IsRequired();
            b.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
            b.Property(u => u.LastName).HasMaxLength(50).IsRequired();
            b.Property(u => u.Username).HasMaxLength(50).IsRequired();
            b.HasIndex(u => u.Email).IsUnique();
            b.HasIndex(u => u.Username).IsUnique();

            // Identity FK scalar (exists in domain User)
            b.Property(u => u.IdentityUserId).HasMaxLength(450).IsRequired(false);
            b.HasIndex(u => u.IdentityUserId).IsUnique(false);

            // Configure optional 1:1 with AspNetUsers (ApplicationUser) without requiring a navigation on Core
            b.HasOne<ApplicationUser>()
             .WithOne() // or .WithOne(a => a.User) if ApplicationUser exposes navigation
             .HasForeignKey<User>(u => u.IdentityUserId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // Customers (domain) mapped to table "Customer"
        modelBuilder.Entity<Customer>(c =>
        {
            c.ToTable("Customer");
            c.HasKey(cu => cu.Id); // now allowed because Person is ignored
            c.Property(cu => cu.FirstName).HasMaxLength(50).IsRequired();
            c.Property(cu => cu.LastName).HasMaxLength(50).IsRequired();
            c.Property(cu => cu.Email).HasMaxLength(200).IsRequired();
            c.HasIndex(cu => cu.Email).IsUnique();
        });

        // Sales
        modelBuilder.Entity<Sale>(b =>
        {
            b.ToTable("Sale");
            b.HasKey(s => s.Id);
            b.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            
            // Propiedades con ENUMs (se guardan como strings en BD)
            b.Property(s => s.Subtotal).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(s => s.Tax).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(s => s.Discount).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(s => s.Notes).HasMaxLength(1000);
            b.Property(s => s.InvoiceNumber).HasMaxLength(50);
            
            b.HasOne(s => s.Customer).WithMany(c => c.Sales).HasForeignKey(s => s.CustomerId);
            b.HasIndex(s => s.InvoiceNumber).IsUnique(false);
            b.HasIndex(s => s.Status);
        });

        // SaleItems
        modelBuilder.Entity<SaleItem>(b =>
        {
            b.ToTable("SaleItem");
            b.HasKey(si => si.Id);
            b.Property(si => si.Quantity).IsRequired();
            b.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            b.HasOne(si => si.Sale).WithMany(s => s.Items).HasForeignKey(si => si.SaleId);
            b.HasOne(si => si.Product).WithMany(p => p.SaleItems).HasForeignKey(si => si.ProductId);
        });
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
                // entry.Entity.CreatedBy = _currentUserService?.UserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
                //entry.Entity.UpdatedBy = _currentUserService?.UserId;
                entry.Property(e => e.CreatedAt).IsModified = false;
                entry.Property(e => e.CreatedBy).IsModified = false;
            }
        }
    }
}

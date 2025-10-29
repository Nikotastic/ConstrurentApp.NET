// File: Firmness.Infrastructure/Data/AppDbContext.cs
using Firmness.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;

    // OnModelCreating is called when the model is being created
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
        });

        // Users
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("User");
            b.HasKey(u => u.Id);
            b.Property(u => u.Email).HasMaxLength(200).IsRequired();
            b.Property(u => u.PasswordHash).HasMaxLength(200).IsRequired();
            b.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
            b.Property(u => u.LastName).HasMaxLength(50).IsRequired();
            b.Property(u => u.Username).HasMaxLength(50).IsRequired();
            b.HasIndex(u => u.Email).IsUnique();
            b.HasIndex(u => u.Username).IsUnique();
        });

        // Customers
        modelBuilder.Entity<Customer>(b =>
        {
            b.ToTable("Customer");
            b.HasKey(c => c.Id);
            b.Property(c => c.FirstName).HasMaxLength(50).IsRequired();
            b.Property(c => c.LastName).HasMaxLength(50).IsRequired();
            b.Property(c => c.Email).HasMaxLength(200).IsRequired();
            b.HasIndex(c => c.Email).IsUnique();
        });

        // Sales
        modelBuilder.Entity<Sale>(b =>
        {
            b.ToTable("Sale");
            b.HasKey(s => s.Id);
            b.Property(s => s.CreatedAt).IsRequired();
            b.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            b.HasOne(s => s.Customer).WithMany(c => c.Sales).HasForeignKey(s => s.CustomerId);
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
}

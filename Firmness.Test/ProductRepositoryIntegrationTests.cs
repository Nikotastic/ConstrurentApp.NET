using Firmness.Domain.Entities;
using Firmness.Infrastructure.Data;
using Firmness.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Test;

public class ProductRepositoryIntegrationTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAndGet_Product_Works()
    {
        await using var db = CreateDbContext();
        var repo = new ProductRepository(db);

        // Usa el constructor de Product en lugar del object initializer.
        var product = new Product("SKU", "P1",  "", 10m, "", 20);

        await repo.AddAsync(product);

        var fetched = await repo.GetByIdAsync(product.Id);
        Assert.NotNull(fetched);
        Assert.Equal("P1", fetched!.Name);

        var fetchedDirect = await db.Products.FindAsync(product.Id);
        Assert.NotNull(fetchedDirect);
    }
}

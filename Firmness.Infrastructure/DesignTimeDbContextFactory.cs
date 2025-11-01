using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Firmness.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("CONN_STR")
                   ?? "Host=postgres;Port=5432;Database=FirmnessDB;Username=postgres;Password=niko";
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(conn);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
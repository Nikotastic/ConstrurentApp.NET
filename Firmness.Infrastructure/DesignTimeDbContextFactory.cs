using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Firmness.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("CONN_STR");
        
        if (string.IsNullOrWhiteSpace(conn))
        {
            conn = "Host=localhost;Port=5432;Database=FirmnessDB;Username=postgres;Password=niko";
            Console.WriteLine($"[DesignTimeDbContextFactory] Using default connection (localhost): {conn}");
        }
        else
        {
            Console.WriteLine($"[DesignTimeDbContextFactory] Using CONN_STR: {conn}");
        }
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(conn);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Repositories;
using Firmness.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Firmness.Infrastructure.Data;

namespace Firmness.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositories (adapters)
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISaleItemRepository, SaleItemRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IVehicleRentalRepository, VehicleRentalRepository>();

        // UnitOfWork for application services to consume
        services.AddScoped<IUnitOfWork, ApplicationDbContext>();
        
        // Email Service Configuration (adapter)
        // The configuration determines which implementation is used (Gmail or Enterprise)
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        
        // Register the email service based on the configuration
        var emailProvider = configuration.GetValue<string>("EmailSettings:Provider") ?? "Gmail";
        
        if (emailProvider.Equals("Enterprise", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IEmailService, EnterpriseEmailService>();
        }
        else
        {
            services.AddScoped<IEmailService, GmailEmailService>();
        }
        
        return services;
    }
}
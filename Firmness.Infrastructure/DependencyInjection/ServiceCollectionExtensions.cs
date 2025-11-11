using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Firmness.Infrastructure.Data;

namespace Firmness.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registro de repositorios (adapters)
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISaleItemRepository, SaleItemRepository>();  

        // Registrar UnitOfWork para que los application services lo consuman
        services.AddScoped<IUnitOfWork, ApplicationDbContext>();

        // Registro de application services (si tus servicios están en Application project,
        // puedes registrarlos aquí o desde Application; registrar aquí es práctico)
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IBulkImportService, BulkImportService>();

        // añadir otros infra services (email, storage, etc.) aquí

        return services;
    }
}
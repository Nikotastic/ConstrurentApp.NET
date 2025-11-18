﻿using Firmness.Application.Interfaces;
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
        return services;
    }
}
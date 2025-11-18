﻿﻿using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Firmness.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    // Register application services
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services
        services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);
        
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBulkImportService, BulkImportService>();
        services.AddScoped<IExcelTemplateService, ExcelTemplateService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IVehicleRentalService, VehicleRentalService>();
        return services;
    }
}


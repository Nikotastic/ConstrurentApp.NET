using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Application.Services.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Firmness.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);

        // Register all services from the Application layer
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBulkImportService, BulkImportService>();
        services.AddScoped<IExcelTemplateService, ExcelTemplateService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IVehicleRentalService, VehicleRentalService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // Register HttpClient for AI services
        services.AddHttpClient();
        
        // Register AI Chat Service - SMART VERSION with Function Calling
        // This version uses Gemini Function Calling to query database only when needed
        services.AddScoped<IAiChatService, SmartGeminiChatService>();
        
        return services;
    }
}

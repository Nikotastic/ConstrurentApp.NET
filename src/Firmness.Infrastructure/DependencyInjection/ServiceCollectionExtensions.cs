using Amazon.S3;
using Amazon.Runtime;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Repositories;
using Firmness.Infrastructure.Email;
using Firmness.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Firmness.Infrastructure.Data;
using Firmness.Application.Interfaces;

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
        
        // Identity Services
        services.AddScoped<IUserAccountService, Firmness.Infrastructure.Identity.UserAccountService>();

        // Email Service Configuration (adapter)
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        
        var emailProvider = configuration.GetValue<string>("EmailSettings:Provider") ?? "Gmail";
        
        if (emailProvider.Equals("Enterprise", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IEmailService, EnterpriseEmailService>();
        }
        else
        {
            services.AddScoped<IEmailService, GmailEmailService>();
        }
        
        // S3 File Storage Service Configuration
        services.Configure<S3Settings>(configuration.GetSection("S3Settings"));
        
        // Register AWS S3 Client
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Settings = configuration.GetSection("S3Settings").Get<S3Settings>();
            
            if (s3Settings == null || string.IsNullOrWhiteSpace(s3Settings.AccessKey))
            {
                // Return a dummy client if S3 is not configured (for development)
                return null!;
            }
            
            var credentials = new BasicAWSCredentials(s3Settings.AccessKey, s3Settings.SecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(s3Settings.Region)
            };
            
            return new AmazonS3Client(credentials, config);
        });
        
        services.AddScoped<IFileStorageService, S3FileStorageService>();
        
        return services;
    }
}
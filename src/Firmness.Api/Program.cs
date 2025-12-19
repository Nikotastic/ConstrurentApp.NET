using Firmness.Infrastructure.DependencyInjection;
using Firmness.Application.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContext = Firmness.Infrastructure.Data.ApplicationDbContext;
using Firmness.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Add environment variables to configuration 
builder.Configuration.AddEnvironmentVariables();

// Load .env or .env.local if present (same approach as Admin.Web, prefer .env.local)
string[] candidates = { ".env.local", ".env" };
string? envPath = null;
foreach (var name in candidates)
{
    var candidate = Path.Combine(builder.Environment.ContentRootPath, name);
    if (File.Exists(candidate))
    {
        envPath = candidate;
        break;
    }
}
if (envPath == null)
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null)
    {
        foreach (var name in candidates)
        {
            var candidate = Path.Combine(dir.FullName, name);
            if (File.Exists(candidate))
            {
                envPath = candidate;
                break;
            }
        }
        if (envPath != null) break;
        dir = dir.Parent;
    }
}
if (!string.IsNullOrEmpty(envPath))
{
    try
    {
  
        DotNetEnv.Env.Load(envPath);
        Console.WriteLine($"Loaded env file: {envPath}");
    }
    catch
    {
        // ignore
    }
}

// Resolve connection string from environment variables or appsettings
var configuration = builder.Configuration;

// Try to get complete connection string first
var rawConn = Environment.GetEnvironmentVariable("CONN_STR")
              ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
              ?? configuration.GetConnectionString("DefaultConnection");

// Sanitize connection string from potentially invisible characters or quotes coming from Azure
var defaultConn = rawConn?.Trim(' ', '"', '\'', '\r', '\n');

// If no complete connection string, build it from individual environment variables
if (string.IsNullOrWhiteSpace(defaultConn))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("PG_PORT") ?? "5432";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "FirmnessDB";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

    if (string.IsNullOrWhiteSpace(dbPassword))
    {
        Console.WriteLine("Warning: POSTGRES_PASSWORD not found in environment variables. Database connection may fail.");
        Console.WriteLine("Please set POSTGRES_PASSWORD in your .env file or environment variables.");
    }

    defaultConn = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
    Console.WriteLine($"Built connection string from environment variables: Host={dbHost}, Database={dbName}, User={dbUser}");
}

// If we are not running inside a container, replace host 'postgres' with 'localhost'
var inContainer = string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);
if (!inContainer && defaultConn.IndexOf("Host=postgres", StringComparison.OrdinalIgnoreCase) >= 0)
{
    Console.WriteLine("Info: replacing Host=postgres with Host=localhost for local execution.");
    defaultConn = defaultConn.Replace("Host=postgres", "Host=localhost", StringComparison.OrdinalIgnoreCase);
}

// Register DbContext before infrastructure services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        defaultConn,
        b => b.MigrationsAssembly("Firmness.Infrastructure")
    )
);

// ---------- Identity configuration ----------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT configuration
// IMPORTANT: default must be at least 256 bits (32 bytes). Prefer setting JWT__Key in env or appsettings.
var jwtKey = Environment.GetEnvironmentVariable("JWT__Key") ?? configuration["Jwt:Key"] ?? "dev_default_change_me_to_a_strong_secret_please_!2025";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT__Issuer") ?? configuration["Jwt:Issuer"] ?? "Firmness.Api";
var jwtAudience = Environment.GetEnvironmentVariable("JWT__Audience") ?? configuration["Jwt:Audience"] ?? "FirmnessClients";

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
// Validate key size: HS256 requires at least 256 bits (32 bytes)
if (keyBytes.Length < 32)
{
    throw new InvalidOperationException($"JWT key is too short ({keyBytes.Length * 8} bits). The key must be at least 256 bits (32 bytes). Set environment variable JWT__Key or configuration 'Jwt:Key' with a sufficiently long secret.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireClientRole", policy => policy.RequireRole("Client"));
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddControllers();
// Add Application services (includes AutoMapper)
builder.Services.AddApplicationServices();
// Add infrastructure services (includes application services and repositories)
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to accept JWT
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            }, new string[] { }
        }
    });
});

var app = builder.Build();

// Apply migrations and seed Identity data (roles/users) with retry logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    int maxRetries = 10;
    int retryDelay = 2000; // 2 seconds
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            if (db.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Applying pending migrations...");
                db.Database.Migrate();
            }
            else
            {
                logger.LogInformation("No pending migrations found.");
            }
            
            await SeedData.InitializeAsync(services);
            logger.LogInformation("Database migrated and seed data initialized.");
            break; // Success
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Attempt {i + 1}/{maxRetries} failed to migrate database. Retrying in {retryDelay/1000}s...");
            if (i == maxRetries - 1)
            {
                logger.LogError("Max retries reached. Database migration failed.");
            }
            else
            {
                Thread.Sleep(retryDelay);
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true")
{
    app.UseHttpsRedirection();
}

// Enable CORS - MUST be before Authentication/Authorization
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

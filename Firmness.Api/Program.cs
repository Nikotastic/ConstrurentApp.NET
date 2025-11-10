using Firmness.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContext = Firmness.Infrastructure.Data.ApplicationDbContext;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Load .env if present (same approach as Admin.Web)
var envPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
if (!File.Exists(envPath))
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null)
    {
        var candidate = Path.Combine(dir.FullName, ".env");
        if (File.Exists(candidate))
        {
            envPath = candidate;
            break;
        }
        dir = dir.Parent;
    }
}
if (File.Exists(envPath))
{
    try
    {
        // DotNetEnv is optional; if available load env vars
        DotNetEnv.Env.Load(envPath);
    }
    catch
    {
        // ignore if DotNetEnv not available
    }
}

// Resolve connection string from environment variables or appsettings
var configuration = builder.Configuration;
var defaultConn = Environment.GetEnvironmentVariable("CONN_STR")
                  ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                  ?? configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(defaultConn))
{
    // As fallback use a local Postgres connection (developer convenience). You can override with env var.
    defaultConn = "Host=localhost;Port=5432;Database=FirmnessDB;Username=postgres;Password=postgres";
    Console.WriteLine("Warning: No connection string configured. Using fallback local connection string. Set CONN_STR or ConnectionStrings__DefaultConnection to override.");
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

// Add services to the container.
builder.Services.AddControllers();
// Add infrastructure services (includes application services and repositories)
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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

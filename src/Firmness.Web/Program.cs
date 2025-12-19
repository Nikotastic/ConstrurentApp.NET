using Firmness.Application.Interfaces;
using Firmness.Application.DependencyInjection;
using Firmness.Infrastructure.DependencyInjection;
using Firmness.Infrastructure.Identity;
using Firmness.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContext = Firmness.Infrastructure.Data.ApplicationDbContext;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Configure EPPlus for non-commercial use (free)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Better detection: Check if we're actually running inside a Docker container FIRST
// Docker sets the /.dockerenv file or DOTNET_RUNNING_IN_CONTAINER is set by docker-compose
var dockerEnvFile = File.Exists("/.dockerenv");
var inContainer = dockerEnvFile || (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) && 
                  string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase));

if (inContainer)
{
    // In Docker: Only load .env file, NOT .env.local
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
        DotNetEnv.Env.Load(envPath);
        Console.WriteLine("[INFO] Loaded .env for Docker environment");
    }
}
else
{
    // Local development: Load .env.local first (higher priority), then .env
    var envLocalPath = Path.Combine(builder.Environment.ContentRootPath, ".env.local");
    if (!File.Exists(envLocalPath))
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, ".env.local");
            if (File.Exists(candidate))
            {
                envLocalPath = candidate;
                break;
            }
            dir = dir.Parent;
        }
    }
    
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
    
    if (File.Exists(envLocalPath))
    {
        DotNetEnv.Env.Load(envLocalPath);
        Console.WriteLine("[INFO] Loaded .env.local for local development");
    }
    else if (File.Exists(envPath))
    {
        DotNetEnv.Env.Load(envPath);
        Console.WriteLine("[INFO] Loaded .env file");
    }
}

// Resolve connection string from environment variables or appsettings
var configuration = builder.Configuration;

// Get CONN_STR from environment (loaded from .env or .env.local, or set by docker-compose)
var connStrFromEnv = Environment.GetEnvironmentVariable("CONN_STR");

string? defaultConn = null;

if (inContainer)
{
    // Running in Docker: use CONN_STR from environment (docker-compose sets this)
    defaultConn = connStrFromEnv;
    Console.WriteLine("[INFO] Running in Docker container - using CONN_STR from docker-compose");
}
else
{
    // Running locally: use CONN_STR from .env.local or appsettings
    defaultConn = connStrFromEnv 
                  ?? configuration.GetConnectionString("DefaultConnection") 
                  ?? configuration.GetConnectionString("ApplicationDbContextConnection");
    
    Console.WriteLine("[INFO] Running locally - using CONN_STR from .env.local");
}

if (string.IsNullOrWhiteSpace(defaultConn))
    throw new InvalidOperationException("Connection string not configured. Make sure CONN_STR is set in .env.local (for local) or .env (for Docker)");

Console.WriteLine($"[DEBUG] Final connection string: {defaultConn}");

// Register DbContext using the final string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        defaultConn,
        b => b.MigrationsAssembly("Firmness.Infrastructure")
    )
);

// ---------- Identity (with roles) ----------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password / user options — tweak as needed
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure cookie behavior (login / access denied)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// ---------- Authorization policies ----------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireClientRole", policy => policy.RequireRole("Client"));
});

// ---------- MVC / Razor / (Blazor optional) ----------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    // Protect the whole Admin area with RequireAdminRole policy (applies to Areas/Admin pages)
    options.Conventions.AuthorizeAreaFolder("Admin", "/", "RequireAdminRole");
});

// ---------- Application DI registrations (keep your existing registrations) ----------
// Add Application services (includes AutoMapper for DTOs)
builder.Services.AddApplicationServices();
// Add AutoMapper for ViewModels (Web layer)
builder.Services.AddAutoMapper(typeof(Program).Assembly);
// Identity adapter
builder.Services.AddScoped<IIdentityService, IdentityService>();
// Export and Receipt services
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddInfrastructureServices(builder.Configuration);
// CORS 
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy  =>
        {
            policy.WithOrigins("http://localhost:4200", "http://localhost:7192") // La URL de tu Angular
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
var app = builder.Build();

// ---------- Ensure DB + Seed data ----------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        // Apply pending EF migrations (useful in dev; be cautious in prod)
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        // Seed roles and default users
        await SeedData.InitializeAsync(services);
        logger.LogInformation("Database migrated and seed data initialized.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while migrating or seeding the database.");
        // In production you might want to rethrow to avoid running in an inconsistent state:
        // throw;
    }
}

// ---------- Middleware pipeline ----------
if (!env.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // In dev show developer exceptions as needed
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins); 
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ---------- Endpoints ----------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
using Firmness.Application.Interfaces;
using Firmness.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContext = Firmness.Infrastructure.Data.ApplicationDbContext;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Cargar .env (igual que antes)
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
if (File.Exists(envPath)) DotNetEnv.Env.Load(envPath);

// Resolver connection string
var configuration = builder.Configuration;
var defaultConn = Environment.GetEnvironmentVariable("CONN_STR")
                  ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                  ?? configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(defaultConn))
    throw new InvalidOperationException("Connection string not configured. Se buscó: CONN_STR, ConnectionStrings__DefaultConnection, appsettings.");

// Si no estamos corriendo dentro de un contenedor, sustituir host 'postgres' por 'localhost'
var inContainer = string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);
if (!inContainer && defaultConn.IndexOf("Host=postgres", StringComparison.OrdinalIgnoreCase) >= 0)
{
    Console.WriteLine("Info: reemplazando Host=postgres por Host=localhost para ejecución local.");
    defaultConn = defaultConn.Replace("Host=postgres", "Host=localhost", StringComparison.OrdinalIgnoreCase);
}

// Registrar DbContext usando la cadena final
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
        options.SignIn.RequireConfirmedAccount = false;
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
// Identity adapter
builder.Services.AddScoped<IIdentityService, IdentityService>();
// Repositories
builder.Services.AddScoped<Firmness.Core.Interfaces.IProductRepository, Firmness.Infrastructure.Repositories.ProductRepository>();
builder.Services.AddScoped<Firmness.Core.Interfaces.ICustomerRepository, Firmness.Infrastructure.Repositories.CustomerRepository>();
builder.Services.AddScoped<Firmness.Core.Interfaces.ISaleRepository, Firmness.Infrastructure.Repositories.SaleRepository>();
builder.Services.AddScoped<Firmness.Core.Interfaces.ISaleItemRepository, Firmness.Infrastructure.Repositories.SaleItemRepository>();

// Services
builder.Services.AddScoped<Firmness.Application.Interfaces.IProductService, Firmness.Application.Services.ProductService>();
builder.Services.AddScoped<Firmness.Application.Interfaces.ISaleService, Firmness.Application.Services.SaleService>();
builder.Services.AddScoped<Firmness.Application.Interfaces.ICustomerService, Firmness.Application.Services.CustomerService>();

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
    // In dev show migrations endpoint or developer exceptions as needed
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ---------- Endpoints ----------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// If you are using Server-side Blazor, enable these:
// app.MapBlazorHub();
// app.MapFallbackToPage("/_Host");

app.Run();
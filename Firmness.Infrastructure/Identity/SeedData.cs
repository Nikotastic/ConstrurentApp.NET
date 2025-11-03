using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
namespace Firmness.Infrastructure.Identity
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            var roleManager = sp.GetService<RoleManager<IdentityRole>>();
            var userManager = sp.GetService<UserManager<ApplicationUser>>();

            if (roleManager == null) throw new InvalidOperationException("RoleManager<IdentityRole> not registered in DI.");
            if (userManager == null) throw new InvalidOperationException("UserManager<ApplicationUser> not registered in DI.");

            string[] roles = new[] { "Admin", "Client" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!r.Succeeded)
                    {
                        var errors = string.Join(", ", r.Errors.Select(e => e.Description));
                        throw new Exception($"Error creating role '{role}': {errors}");
                    }
                }
            }

            // create admin user
            var adminEmail = "admin@firmness.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var create = await userManager.CreateAsync(adminUser, "Admin123!");
                if (create.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    var errors = string.Join(", ", create.Errors.Select(e => e.Description));
                    throw new Exception($"Error creating admin user: {errors}");
                }
            }
            else
            {
                // Sure admin has Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        throw new Exception($"Error adding Admin role to existing admin user: {errors}");
                    }
                }
            }

            // optional: create client user
            var clientEmail = "client@firmness.local";
            var clientUser = await userManager.FindByEmailAsync(clientEmail);
            if (clientUser == null)
            {
                clientUser = new ApplicationUser
                {
                    UserName = "client",
                    Email = clientEmail,
                    EmailConfirmed = true
                };

                var createClient = await userManager.CreateAsync(clientUser, "Client123!");
                if (createClient.Succeeded)
                {
                    await userManager.AddToRoleAsync(clientUser, "Client");
                }
                else 
                {
                    var errors = string.Join(", ", createClient.Errors.Select(e => e.Description));
                    throw new Exception($"Error creating client user: {errors}");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(clientUser, "Client"))
                {
                    await userManager.AddToRoleAsync(clientUser, "Client");
                }
            }
        }
    }
}
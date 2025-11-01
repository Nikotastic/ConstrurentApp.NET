using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Firmness.Infrastructure.Identity;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = new[] { "Admin", "Client" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!r.Succeeded)
                    {
                        throw new Exception($"Error to create role {role}: {string.Join(", ", r.Errors.Select(e => e.Description))}");
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
                    throw new Exception($"Error to create user admin: {string.Join(", ", create.Errors.Select(e => e.Description))}");
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
            }
        }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Firmness.Infrastructure.Data;
using Firmness.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
            var context = sp.GetService<ApplicationDbContext>();

            if (roleManager == null) throw new InvalidOperationException("RoleManager<IdentityRole> not registered in DI.");
            if (userManager == null) throw new InvalidOperationException("UserManager<ApplicationUser> not registered in DI.");
            if (context == null) throw new InvalidOperationException("ApplicationDbContext not registered in DI.");

            // Seed Roles
            await SeedRolesAsync(roleManager);
            
            // Seed Users
            await SeedUsersAsync(userManager);
            
            // Seed Categories for Construction and Industrial Vehicles
            await SeedCategoriesAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
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
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
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

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            // Check if categories already exist
            if (await context.Categories.AnyAsync())
            {
                return; // Categories already seeded
            }

            var categories = new List<Category>
            {
                // ========== CONSTRUCTION MATERIALS ==========
                new Category("Cement and Concrete (Cemento y Concreto)", "Cement, ready-mix concrete, mortar and related products"),
                new Category("Aggregates (Agregados)", "Sand, gravel, crushed stone and fill materials"),
                new Category("Block and Brick (Block y Ladrillo)", "Concrete blocks, bricks, tiles and masonry materials"),
                new Category("Rebar and Steel (Varilla y Acero)", "Corrugated rebar, wire rod, welded mesh and metal structures"),
                new Category("Wood and Boards (Madera y Tableros)", "Construction lumber, plywood, MDF, OSB and various boards"),
                new Category("Pipe and PVC (Tubería y PVC)", "Hydraulic, sanitary, electrical piping in PVC, copper and other materials"),
                new Category("Hardware and Fittings (Herrajes y Ferretería)", "Nails, screws, hinges, locks and metal accessories"),
                new Category("Paints and Coatings (Pinturas y Recubrimientos)", "Vinyl paints, enamels, waterproofing and sealants"),
                new Category("Floors and Tiles (Pisos y Azulejos)", "Ceramic, porcelain, tile, marble and floor and wall coverings"),
                new Category("Doors and Windows (Puertas y Ventanas)", "Wooden, metal, aluminum doors, windows and frames"),
                new Category("Electrical (Electricidad)", "Electrical wire, outlets, switches, lighting and accessories"),
                new Category("Plumbing and Bathrooms (Plomería y Baños)", "Faucets, showers, toilets, sinks, bathtubs and sanitary accessories"),
                new Category("Waterproofing (Impermeabilizantes)", "Membranes, asphalt products, sealants and waterproofing systems"),
                new Category("Insulation (Aislantes)", "Thermal insulation, acoustic, fiberglass and foams"),
                new Category("Adhesives and Glues (Adhesivos y Pegamentos)", "Construction adhesives, ceramic glue, silicones"),
                new Category("Hand Tools (Herramientas Manuales)", "Hammers, shovels, wheelbarrows, levels and masonry tools"),
                new Category("Power Tools (Herramientas Eléctricas)", "Drills, saws, grinders, rotary hammers and power equipment"),
                new Category("Industrial Safety (Seguridad Industrial)", "Helmets, gloves, harnesses, boots and personal protective equipment"),
                new Category("Geotextiles and Drainage (Geotextiles y Drenaje)", "Geotextile meshes, drainage piping and filtration systems"),
                new Category("Aluminum Windows (Cancelería de Aluminio)", "Aluminum profiles, glass, hardware and window accessories"),

                // ========== INDUSTRIAL VEHICLES AND MACHINERY RENTAL ==========
                new Category("Forklifts (Montacargas)", "Electric forklifts, combustion, reach trucks and stackers"),
                new Category("Excavators (Excavadoras)", "Track excavators, wheeled excavators, mini excavators"),
                new Category("Backhoes (Retroexcavadoras)", "Backhoes, front loaders with backhoe arm"),
                new Category("Front Loaders (Cargadores Frontales)", "Wheel loaders, mini loaders, skid steer loaders"),
                new Category("Compactors (Compactadoras)", "Compactor rollers, vibratory compactors and rammers"),
                new Category("Cranes and Booms (Grúas y Plumas)", "Telescopic cranes, tower cranes, hydraulic booms"),
                new Category("Dump Trucks (Camiones de Volteo)", "Dump trucks 3.5, 7, 14 tons for material transport"),
                new Category("Concrete Mixers (Revolvedoras de Concreto)", "Portable mixers, concrete and mortar mixers"),
                new Category("Electric Generators (Generadores Eléctricos)", "Diesel generators, gasoline, portable and industrial power plants"),
                new Category("Air Compressors (Compresores de Aire)", "Portable compressors, industrial, screw and piston"),
                new Category("Water Pumps (Bombas de Agua)", "Submersible pumps, centrifugal, drainage and sludge pumps"),
                new Category("Scaffolding and Lifts (Andamios y Elevadores)", "Tubular scaffolding, personnel lifts, scissor platforms"),
                new Category("Cutting Equipment (Equipos de Corte)", "Concrete cutters, asphalt, hand and disc saws"),
                new Category("Demolition Hammers (Martillos Demoledores)", "Pneumatic hammers, hydraulic, electric breakers"),
                new Category("Concrete Vibrators (Vibradores de Concreto)", "Immersion vibrators, vibrating screed, vibrating tables"),
                new Category("Light Towers (Torres de Iluminación)", "Construction light towers, portable floodlights"),
                new Category("Welding Equipment (Equipos de Soldadura)", "Welding machines, welding plants, cutting equipment"),
                new Category("Motor Graders (Motoconformadoras)", "Motor graders, leveling and land forming equipment"),
                new Category("Tractors and Bulldozers (Tractores y Bulldozers)", "Track tractors, bulldozers, earthmoving equipment"),
                new Category("Aerial Platforms (Plataformas Elevadoras)", "Scissor platforms, articulated, telescopic for work at height"),
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}
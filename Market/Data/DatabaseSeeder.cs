using Market.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Market.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    SeedRoles(context, logger);
                    // Agrega más métodos de siembra según sea necesario
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }

        private static void SeedRoles(ApplicationDbContext context, ILogger logger)
        {
            if (!context.Roles.Any())
            {
                logger.LogInformation("Seeding roles...");
                context.Roles.AddRange(
                    new Role {Id = 1, Name = "Admin" },
                    new Role {Id = 2, Name = "User" }
                );
                context.SaveChanges();
                logger.LogInformation("Roles seeded successfully.");
            }
        }
    }
}
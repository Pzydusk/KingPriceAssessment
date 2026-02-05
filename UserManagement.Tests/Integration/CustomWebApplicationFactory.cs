using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using UserManagement.API.Context;

namespace UserManagement.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Remove ALL DbContext-related services
                RemoveDbContext<UserManagementContext>(services);

                // Add SQL Server for testing
                // Using LocalDB with a unique database name for isolation
                var databaseName = $"UserManagementTest_{Guid.NewGuid()}";
                services.AddDbContext<UserManagementContext>(options =>
                {
                    options.UseSqlServer(
                        $"Server=.;Database=UserManagementDb;Trusted_Connection=True;TrustServerCertificate=True;",
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(UserManagementContext).Assembly.FullName);
                        });
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                });

                // Build service provider
                var sp = services.BuildServiceProvider();

                // Initialize database
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<UserManagementContext>();

                    // Ensure database is created with migrations
                    db.Database.EnsureDeleted(); // Clean up any existing data
                    db.Database.EnsureCreated();
                    // OR if you have migrations:
                    // db.Database.Migrate();
                }
            });
        }

        private static void RemoveDbContext<TContext>(IServiceCollection services)
            where TContext : DbContext
        {
            // Get descriptors for all DbContext-related services
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<TContext>) ||
                           d.ServiceType == typeof(DbContextOptions) ||
                           d.ServiceType == typeof(TContext) ||
                           (d.ServiceType.IsGenericType &&
                            d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }
        }

        // Optional: Add cleanup when factory is disposed
        protected override void Dispose(bool disposing)
        {
            // Clean up test databases
            if (disposing)
            {
                using (var scope = Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<UserManagementContext>();
                    try
                    {
                        db.Database.EnsureDeleted();
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }

            base.Dispose(disposing);
        }
    }
}
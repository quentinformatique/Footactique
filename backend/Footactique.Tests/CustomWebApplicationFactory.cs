using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Footactique.Services;

namespace Footactique.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove all PostgreSQL-related services
                ServiceDescriptor[] toRemove = services
                    .Where(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                                s.ServiceType == typeof(ApplicationDbContext) ||
                                s.ImplementationType?.Name.Contains("Npgsql") == true ||
                                s.ServiceType.Name.Contains("Npgsql") == true)
                    .ToArray();

                foreach (ServiceDescriptor d in toRemove)
                {
                    services.Remove(d);
                }

                // Add in-memory database
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                });

                // Create a new service provider
                ServiceProvider sp = services.BuildServiceProvider();
                using IServiceScope scope = sp.CreateScope();
                ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Ensure database is created
                db.Database.EnsureCreated();
            });

            builder.UseEnvironment("Testing");
        }
    }
} 
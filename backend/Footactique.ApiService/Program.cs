using Footactique.Services;
using Microsoft.EntityFrameworkCore;
using Footactique.Services.Services;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using Microsoft.OpenApi.Models;

namespace Footactique.Api
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add service defaults & Aspire client integrations.  
            builder.AddServiceDefaults();
            Console.WriteLine(builder.Configuration.GetConnectionString("footactique"));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(builder.Configuration.GetConnectionString("footactique")
                   ?? throw new InvalidOperationException("Connection string 'postgresdb' not found.")));

            // --- OpenTelemetry configuration ---
            // Enable OpenTelemetry only if not running under Aspire/local dev
            var env = builder.Environment.EnvironmentName;
            var isAspire = builder.Configuration["ASPIRE_ENABLED"] == "true";
            if (!isAspire && env != "Development")
            {
                string serviceName = "Footactique.ApiService";
                builder.Services.AddOpenTelemetry()
                    .ConfigureResource(resource => resource.AddService(serviceName))
                    .WithTracing(tracing => tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation()
                    )
                    .WithMetrics(metrics => metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                    );
            }
            // --- End OpenTelemetry configuration ---

            // Register business services
            builder.Services.AddScoped<ITeamCompositionService, TeamCompositionService>();

            // Add MVC controllers
            builder.Services.AddControllers();

            // Add services to the container.  
            builder.Services.AddProblemDetails();

            // --- Swagger/OpenAPI configuration ---
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Footactique API",
                    Version = "v1",
                    Description = "API for managing football team compositions (SaaS)",
                });
                // Optionally, add security, XML comments, etc.
            });
            // --- End Swagger/OpenAPI configuration ---

            var app = builder.Build();

            // --- Apply EF Core migrations automatically in Development ---
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.Migrate();
                }
            }
            // --- End auto-migration ---

            // Configure the HTTP request pipeline.  
            app.UseExceptionHandler();

            // Enable Swagger UI and OpenAPI only in Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Footactique API v1");
                    options.RoutePrefix = string.Empty; // Swagger UI at root
                });
                app.MapOpenApi();
            }

            // Map controller endpoints for API
            app.MapControllers();

            app.MapDefaultEndpoints();

            app.Run();
        }
    }
}
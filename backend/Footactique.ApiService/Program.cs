using Footactique.Services;
using Footactique.Services.Models;
using Footactique.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Footactique.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "*")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Use PostgreSQL for production
            string connectionString = builder.Configuration.GetConnectionString("footactique") 
                ?? throw new InvalidOperationException("Connection string 'footactique' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure JWT Authentication
            string jwtKey = builder.Configuration["Jwt:Key"] ?? "dev_secret_key_1234567890";
            string jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Footactique";
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            // Configure authorization
            builder.Services.AddAuthorization();

            // Register business services
            builder.Services.AddScoped<ITeamCompositionService, TeamCompositionService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Add MVC controllers
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(Program).Assembly);

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Footactique API", 
                    Version = "v1",
                    Description = "API for managing football team compositions. All new users are automatically assigned the 'user' role. Admin users can create additional admin users through the hidden create-admin endpoint."
                });

                // Add JWT Bearer authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Check if running in Aspire context
            bool isAspire = builder.Configuration["ASPIRE_ENABLED"] == "true";
            if (isAspire)
            {
                // Add Aspire service defaults
                builder.AddServiceDefaults();
            }

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Footactique API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            // Redirect root to Swagger UI only in development
            if (app.Environment.IsDevelopment())
            {
                app.MapGet("/", () => Results.Redirect("/swagger"))
                   .ExcludeFromDescription(); // Hide from Swagger documentation
            }

            app.UseHttpsRedirection();
            
            // Use CORS
            app.UseCors("AllowFrontend");
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
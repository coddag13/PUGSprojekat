using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using System.Text;
using TravelPlanner.Common.Enums;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.WebApi
{
    internal sealed class WebApi : StatelessService
    {
        public WebApi(StatelessServiceContext context)
            : base(context)
        {
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);

                        var jwtKey = builder.Configuration["Jwt:Key"]
                            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
                        var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
                        var jwtAudience = builder.Configuration["Jwt:Audience"]
                            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

                        builder.WebHost
                            .UseKestrel()
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url);

                        builder.Services.AddCors(options =>
                        {
                            options.AddPolicy("FrontendPolicy", policy =>
                            {
                                policy
                                    .WithOrigins("http://localhost:5173")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                            });
                        });

                        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = jwtIssuer,
                                    ValidAudience = jwtAudience,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                                };
                            });

                        builder.Services.AddAuthorization();
                        builder.Services.AddDbContext<AuthDbContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection")));
                        builder.Services.AddDbContext<PlanDbContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("PlanConnection")));
                        builder.Services.AddDbContext<SharingDbContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("SharingConnection")));
                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen(options =>
                        {
                            options.SwaggerDoc("v1", new OpenApiInfo
                            {
                                Title = "TravelPlanner.WebApi",
                                Version = "v1"
                            });

                            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                            {
                                Name = "Authorization",
                                Type = SecuritySchemeType.Http,
                                Scheme = "bearer",
                                BearerFormat = "JWT",
                                In = ParameterLocation.Header,
                                Description = "Enter JWT token like: Bearer {your token}"
                            });

                            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

                        var app = builder.Build();
                        EnsureServiceDatabases(app);
                        SeedAdminUser(app);

                        if (app.Environment.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI();
                        }

                        app.UseCors("FrontendPolicy");
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.MapControllers();

                        return app;
                    }))
            };
        }

        private static void SeedAdminUser(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            var adminEmail = configuration["AdminSeed:Email"]?.Trim().ToLowerInvariant();
            var adminPassword = configuration["AdminSeed:Password"]?.Trim();

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                return;

            var existingAdmin = db.Users.FirstOrDefault(u => u.Email == adminEmail);
            if (existingAdmin is null)
            {
                db.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = configuration["AdminSeed:FirstName"]?.Trim() ?? "System",
                    LastName = configuration["AdminSeed:LastName"]?.Trim() ?? "Admin",
                    Email = adminEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    Role = UserRole.Admin
                });

                db.SaveChanges();
                return;
            }

            if (existingAdmin.Role != UserRole.Admin)
            {
                existingAdmin.Role = UserRole.Admin;
                db.SaveChanges();
            }
        }

        private static void EnsureServiceDatabases(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var authDb = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var planDb = scope.ServiceProvider.GetRequiredService<PlanDbContext>();
            var sharingDb = scope.ServiceProvider.GetRequiredService<SharingDbContext>();

            EnsureDatabaseReady(authDb.Database);
            EnsureDatabaseReady(planDb.Database);
            EnsureDatabaseReady(sharingDb.Database);

        }

        private static void EnsureDatabaseReady(DatabaseFacade database)
        {
            try
            {
                database.CanConnect();
            }
            catch (SqlException)
            {
                // If the database is not reachable yet, try creating it below.
            }

            try
            {
                database.Migrate();
            }
            catch (SqlException)
            {
                // In local debug, databases may already exist while the SQL login
                // still lacks CREATE DATABASE permission. In that case we just continue.
            }
        }
    }
}

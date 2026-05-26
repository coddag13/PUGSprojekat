using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.AuthService.Services.Common
{
    internal sealed class AuthDbContextFactory : IAuthDbContextFactory
    {
        private readonly string _connectionString;

        public AuthDbContextFactory()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        public AuthDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new AuthDbContext(options);
        }
    }
}

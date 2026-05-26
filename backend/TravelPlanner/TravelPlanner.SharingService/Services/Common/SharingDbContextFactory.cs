using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.SharingService.Services.Common
{
    internal sealed class SharingDbContextFactory : ISharingDbContextFactory
    {
        private readonly string _connectionString;

        public SharingDbContextFactory()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        public SharingDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<SharingDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new SharingDbContext(options);
        }
    }
}

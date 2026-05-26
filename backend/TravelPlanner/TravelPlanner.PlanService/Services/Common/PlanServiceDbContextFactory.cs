using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Services.Common
{
    internal sealed class PlanServiceDbContextFactory : IPlanServiceDbContextFactory
    {
        private readonly string _connectionString;

        public PlanServiceDbContextFactory()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        public PlanDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<PlanDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new PlanDbContext(options);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TravelPlanner.Infrastructure.Persistence
{
    public class TravelPlannerDbContextFactory : IDesignTimeDbContextFactory<TravelPlannerDbContext>
    {
        public TravelPlannerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TravelPlannerDbContext>();

            optionsBuilder.UseSqlServer(
    "Server=(localdb)\\MSSQLLocalDB;Database=TravelPlannerDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new TravelPlannerDbContext(optionsBuilder.Options);
        }
    }
}
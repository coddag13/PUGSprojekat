using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TravelPlanner.Infrastructure.Persistence
{
    public class PlanDbContextFactory : IDesignTimeDbContextFactory<PlanDbContext>
    {
        public PlanDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PlanDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-FEM7DBU\\SQLEXPRESS;Database=TravelPlannerPlanDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new PlanDbContext(optionsBuilder.Options);
        }
    }
}

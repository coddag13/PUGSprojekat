using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TravelPlanner.Infrastructure.Persistence
{
    public class SharingDbContextFactory : IDesignTimeDbContextFactory<SharingDbContext>
    {
        public SharingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SharingDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-FEM7DBU\\SQLEXPRESS;Database=TravelPlannerSharingDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new SharingDbContext(optionsBuilder.Options);
        }
    }
}

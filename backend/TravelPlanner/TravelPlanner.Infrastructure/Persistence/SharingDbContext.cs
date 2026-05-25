using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.Infrastructure.Persistence
{
    public class SharingDbContext : DbContext
    {
        public SharingDbContext(DbContextOptions<SharingDbContext> options) : base(options)
        {
        }

        public DbSet<ShareToken> ShareTokens => Set<ShareToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShareToken>()
                .Ignore(st => st.TravelPlan);

            modelBuilder.Entity<ShareToken>()
                .HasIndex(st => st.Token)
                .IsUnique();
        }
    }
}

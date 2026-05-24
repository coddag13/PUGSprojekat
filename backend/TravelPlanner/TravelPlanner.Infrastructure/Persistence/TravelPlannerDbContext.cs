using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.Infrastructure.Persistence
{
    public class TravelPlannerDbContext : DbContext
    {
        public TravelPlannerDbContext(DbContextOptions<TravelPlannerDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<TravelPlan> TravelPlans => Set<TravelPlan>();

        public DbSet<Destination> Destinations => Set<Destination>();

        public DbSet<PlanActivity> PlanActivities => Set<PlanActivity>();

        public DbSet<Expense> Expenses => Set<Expense>();

        public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

        public DbSet<ShareToken> ShareTokens => Set<ShareToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TravelPlans)
                .WithOne(tp => tp.Owner)
                .HasForeignKey(tp => tp.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelPlan>()
                .HasMany(tp => tp.Destinations)
                .WithOne(d => d.TravelPlan)
                .HasForeignKey(d => d.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelPlan>()
                .HasMany(tp => tp.Activities)
                .WithOne(a => a.TravelPlan)
                .HasForeignKey(a => a.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelPlan>()
                .HasMany(tp => tp.Expenses)
                .WithOne(e => e.TravelPlan)
                .HasForeignKey(e => e.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelPlan>()
                .HasMany(tp => tp.ChecklistItems)
                .WithOne(c => c.TravelPlan)
                .HasForeignKey(c => c.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelPlan>()
                .HasMany(tp => tp.ShareTokens)
                .WithOne(st => st.TravelPlan)
                .HasForeignKey(st => st.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanActivity>()
                 .HasOne(a => a.Destination)
                 .WithMany()
                 .HasForeignKey(a => a.DestinationId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<TravelPlan>()
                .Property(tp => tp.PlannedBudget)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PlanActivity>()
                .Property(a => a.EstimatedCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);
        }
    }
}
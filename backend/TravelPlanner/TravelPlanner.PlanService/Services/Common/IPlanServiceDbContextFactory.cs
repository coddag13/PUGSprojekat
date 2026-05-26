using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Services.Common
{
    internal interface IPlanServiceDbContextFactory
    {
        PlanDbContext CreateDbContext();
    }
}

using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.SharingService.Services.Common
{
    internal interface ISharingDbContextFactory
    {
        SharingDbContext CreateDbContext();
    }
}

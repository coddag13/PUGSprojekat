using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.AuthService.Services.Common
{
    internal interface IAuthDbContextFactory
    {
        AuthDbContext CreateDbContext();
    }
}

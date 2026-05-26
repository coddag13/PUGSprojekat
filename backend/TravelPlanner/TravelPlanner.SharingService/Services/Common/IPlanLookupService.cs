using TravelPlanner.Common.Models;

namespace TravelPlanner.SharingService.Services.Common
{
    internal interface IPlanLookupService
    {
        Task<TravelPlanData?> GetPlanByIdAsync(Guid travelPlanId);
    }
}

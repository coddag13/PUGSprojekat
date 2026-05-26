using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Models;

namespace TravelPlanner.PlanService.Services.Activities
{
    internal interface IActivityCrudService
    {
        Task<List<ActivityData>> GetActivitiesAsync(Guid travelPlanId);
        Task<ActivityData?> GetActivityByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ActivityData>> CreateActivityAsync(Guid travelPlanId, Guid? destinationId, string name, DateTime date, TimeSpan time, string location, string description, decimal estimatedCost, ActivityStatus status);
        Task<bool> UpdateActivityAsync(Guid travelPlanId, Guid id, Guid? destinationId, string name, DateTime date, TimeSpan time, string location, string description, decimal estimatedCost, ActivityStatus status);
        Task<bool> DeleteActivityAsync(Guid travelPlanId, Guid id);
    }
}

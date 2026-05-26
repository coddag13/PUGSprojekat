using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.PlanService.Mappings
{
    internal static class ActivityMapper
    {
        public static ActivityData Map(PlanActivity activity)
        {
            return new ActivityData
            {
                Id = activity.Id,
                TravelPlanId = activity.TravelPlanId,
                DestinationId = activity.DestinationId,
                Name = activity.Name,
                Date = activity.Date,
                Time = activity.Time,
                Location = activity.Location,
                Description = activity.Description,
                EstimatedCost = activity.EstimatedCost,
                Status = activity.Status
            };
        }
    }
}

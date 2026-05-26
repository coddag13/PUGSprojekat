using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.PlanService.Mappings
{
    internal static class TravelPlanMapper
    {
        public static TravelPlanData Map(TravelPlan plan)
        {
            return new TravelPlanData
            {
                Id = plan.Id,
                OwnerId = plan.OwnerId,
                Title = plan.Title,
                Description = plan.Description,
                StartDate = plan.StartDate,
                EndDate = plan.EndDate,
                PlannedBudget = plan.PlannedBudget,
                Notes = plan.Notes
            };
        }
    }
}

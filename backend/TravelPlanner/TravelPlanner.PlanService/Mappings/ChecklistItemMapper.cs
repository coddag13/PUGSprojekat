using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.PlanService.Mappings
{
    internal static class ChecklistItemMapper
    {
        public static ChecklistItemData Map(ChecklistItem item)
        {
            return new ChecklistItemData
            {
                Id = item.Id,
                TravelPlanId = item.TravelPlanId,
                Text = item.Text,
                IsCompleted = item.IsCompleted
            };
        }
    }
}

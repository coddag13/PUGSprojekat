using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.PlanService.Mappings
{
    internal static class ReminderMapper
    {
        public static ReminderData Map(Reminder reminder)
        {
            return new ReminderData
            {
                Id = reminder.Id,
                TravelPlanId = reminder.TravelPlanId,
                Title = reminder.Title,
                RemindAt = reminder.RemindAt,
                IsCompleted = reminder.IsCompleted
            };
        }
    }
}

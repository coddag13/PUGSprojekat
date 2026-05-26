using TravelPlanner.Common;
using TravelPlanner.Common.Models;

namespace TravelPlanner.PlanService.Services.Reminders
{
    internal interface IReminderCrudService
    {
        Task<List<ReminderData>> GetRemindersAsync(Guid travelPlanId);
        Task<ReminderData?> GetReminderByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ReminderData>> CreateReminderAsync(Guid travelPlanId, string title, DateTime remindAt);
        Task<bool> UpdateReminderAsync(Guid travelPlanId, Guid id, string title, DateTime remindAt, bool isCompleted);
        Task<bool> DeleteReminderAsync(Guid travelPlanId, Guid id);
    }
}

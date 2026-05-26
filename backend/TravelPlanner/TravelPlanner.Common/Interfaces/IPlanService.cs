using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Models;

namespace TravelPlanner.Common.Interfaces
{
    public interface IPlanService : IService
    {
        // Travel Plans
        Task<List<TravelPlanData>> GetAllPlansByOwnerAsync(Guid ownerId);
        Task<TravelPlanData?> GetPlanByIdAsync(Guid id);
        Task<ServiceResponse<TravelPlanData>> CreatePlanAsync(Guid ownerId, string title, string description, DateTime startDate, DateTime endDate, decimal plannedBudget, string notes);
        Task<bool> UpdatePlanAsync(Guid id, string title, string description, DateTime startDate, DateTime endDate, decimal plannedBudget, string notes);
        Task<bool> DeletePlanAsync(Guid id);

        // Destinations
        Task<List<DestinationData>> GetDestinationsAsync(Guid travelPlanId);
        Task<DestinationData?> GetDestinationByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<DestinationData>> CreateDestinationAsync(Guid travelPlanId, string name, string location, DateTime arrivalDate, DateTime departureDate, string description);
        Task<bool> UpdateDestinationAsync(Guid travelPlanId, Guid id, string name, string location, DateTime arrivalDate, DateTime departureDate, string description);
        Task<bool> DeleteDestinationAsync(Guid travelPlanId, Guid id);

        // Activities
        Task<List<ActivityData>> GetActivitiesAsync(Guid travelPlanId);
        Task<ActivityData?> GetActivityByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ActivityData>> CreateActivityAsync(Guid travelPlanId, Guid? destinationId, string name, DateTime date, TimeSpan time, string location, string description, decimal estimatedCost, ActivityStatus status);
        Task<bool> UpdateActivityAsync(Guid travelPlanId, Guid id, Guid? destinationId, string name, DateTime date, TimeSpan time, string location, string description, decimal estimatedCost, ActivityStatus status);
        Task<bool> DeleteActivityAsync(Guid travelPlanId, Guid id);

        // Expenses
        Task<List<ExpenseData>> GetExpensesAsync(Guid travelPlanId);
        Task<ExpenseData?> GetExpenseByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ExpenseData>> CreateExpenseAsync(Guid travelPlanId, string name, ExpenseCategory category, decimal amount, DateTime date, string description);
        Task<bool> UpdateExpenseAsync(Guid travelPlanId, Guid id, string name, ExpenseCategory category, decimal amount, DateTime date, string description);
        Task<bool> DeleteExpenseAsync(Guid travelPlanId, Guid id);

        // Checklist Items
        Task<List<ChecklistItemData>> GetChecklistItemsAsync(Guid travelPlanId);
        Task<ChecklistItemData?> GetChecklistItemByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ChecklistItemData>> CreateChecklistItemAsync(Guid travelPlanId, string text);
        Task<bool> UpdateChecklistItemAsync(Guid travelPlanId, Guid id, string text, bool isCompleted);
        Task<bool> DeleteChecklistItemAsync(Guid travelPlanId, Guid id);

        // Reminders
        Task<List<ReminderData>> GetRemindersAsync(Guid travelPlanId);
        Task<ReminderData?> GetReminderByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ReminderData>> CreateReminderAsync(Guid travelPlanId, string title, DateTime remindAt);
        Task<bool> UpdateReminderAsync(Guid travelPlanId, Guid id, string title, DateTime remindAt, bool isCompleted);
        Task<bool> DeleteReminderAsync(Guid travelPlanId, Guid id);
    }
}

using TravelPlanner.Common;
using TravelPlanner.Common.Models;

namespace TravelPlanner.PlanService.Services.Checklist
{
    internal interface IChecklistCrudService
    {
        Task<List<ChecklistItemData>> GetChecklistItemsAsync(Guid travelPlanId);
        Task<ChecklistItemData?> GetChecklistItemByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ChecklistItemData>> CreateChecklistItemAsync(Guid travelPlanId, string text);
        Task<bool> UpdateChecklistItemAsync(Guid travelPlanId, Guid id, string text, bool isCompleted);
        Task<bool> DeleteChecklistItemAsync(Guid travelPlanId, Guid id);
    }
}

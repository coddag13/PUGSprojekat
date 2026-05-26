using TravelPlanner.Common;
using TravelPlanner.Common.Models;

namespace TravelPlanner.PlanService.Services.Plans
{
    internal interface IPlanCrudService
    {
        Task<List<TravelPlanData>> GetAllPlansByOwnerAsync(Guid ownerId);
        Task<TravelPlanData?> GetPlanByIdAsync(Guid id);
        Task<ServiceResponse<TravelPlanData>> CreatePlanAsync(Guid ownerId, string title, string description, DateTime startDate, DateTime endDate, decimal plannedBudget, string notes);
        Task<bool> UpdatePlanAsync(Guid id, string title, string description, DateTime startDate, DateTime endDate, decimal plannedBudget, string notes);
        Task<bool> DeletePlanAsync(Guid id);
    }
}

using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Validation
{
    internal interface IChecklistValidationService
    {
        string? ValidateText(string text);
        Task<string?> ValidateForPlanAsync(PlanDbContext db, Guid travelPlanId, string text, Guid? excludeChecklistItemId = null);
    }
}

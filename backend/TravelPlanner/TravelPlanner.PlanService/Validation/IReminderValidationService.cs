using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Validation
{
    internal interface IReminderValidationService
    {
        string? ValidateTitle(string title);
        Task<string?> ValidateForPlanAsync(PlanDbContext db, Guid travelPlanId, DateTime remindAt);
    }
}

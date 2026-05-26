using TravelPlanner.Common.Enums;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.PlanService.Services.Common;

namespace TravelPlanner.PlanService.Validation
{
    internal interface IActivityValidationService
    {
        string? ValidateFields(string name, string location, decimal estimatedCost, ActivityStatus status);
        Task<string?> ValidateForPlanAsync(
            PlanDbContext db,
            Guid travelPlanId,
            Guid? destinationId,
            DateTime date,
            TimeSpan time,
            Guid? excludeActivityId = null);
        Task<string?> ValidateBudgetAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal estimatedCost,
            IBudgetCalculationService budgetCalculationService,
            Guid? excludeActivityId = null);
    }
}

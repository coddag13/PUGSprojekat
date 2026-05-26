using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.PlanService.Services.Common;

namespace TravelPlanner.PlanService.Validation
{
    internal interface IPlanValidationService
    {
        string? ValidateCreate(string title, DateTime startDate, DateTime endDate, decimal plannedBudget);
        bool ValidateUpdateFields(string title, DateTime startDate, DateTime endDate, decimal plannedBudget);
        Task<bool> ValidateUpdateStateAsync(
            PlanDbContext db,
            Guid planId,
            DateTime startDate,
            DateTime endDate,
            decimal plannedBudget,
            IBudgetCalculationService budgetCalculationService);
    }
}

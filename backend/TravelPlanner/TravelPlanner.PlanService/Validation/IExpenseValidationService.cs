using TravelPlanner.Common.Enums;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.PlanService.Services.Common;

namespace TravelPlanner.PlanService.Validation
{
    internal interface IExpenseValidationService
    {
        string? ValidateFields(string name, ExpenseCategory category, decimal amount);
        Task<string?> ValidateForPlanAsync(PlanDbContext db, Guid travelPlanId, DateTime date);
        Task<string?> ValidateBudgetAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal amount,
            IBudgetCalculationService budgetCalculationService,
            Guid? excludeExpenseId = null);
    }
}

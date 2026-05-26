using TravelPlanner.Common.Enums;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.PlanService.Services.Common;

namespace TravelPlanner.PlanService.Validation
{
    internal sealed class ExpenseValidationService : IExpenseValidationService
    {
        public string? ValidateFields(string name, ExpenseCategory category, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Expense name is required.";

            if (amount < 0)
                return "Expense amount cannot be negative.";

            if (!Enum.IsDefined(typeof(ExpenseCategory), category))
                return "Invalid expense category.";

            return null;
        }

        public async Task<string?> ValidateForPlanAsync(PlanDbContext db, Guid travelPlanId, DateTime date)
        {
            var plan = await db.TravelPlans.FindAsync(travelPlanId);
            if (plan is null)
                return "Travel plan not found.";

            return date.Date < plan.StartDate.Date || date.Date > plan.EndDate.Date
                ? "Expense date must be within the travel plan period."
                : null;
        }

        public async Task<string?> ValidateBudgetAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal amount,
            IBudgetCalculationService budgetCalculationService,
            Guid? excludeExpenseId = null)
        {
            var wouldExceedBudget = await budgetCalculationService.WouldExceedBudgetWithExpenseAsync(
                db,
                travelPlanId,
                amount,
                excludeExpenseId);

            return wouldExceedBudget ? "This expense would exceed the planned budget." : null;
        }
    }
}

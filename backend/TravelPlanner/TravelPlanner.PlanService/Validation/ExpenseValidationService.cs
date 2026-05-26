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
                return "Naziv troška je obavezan.";

            if (amount < 0)
                return "Iznos troška ne može biti negativan.";

            if (!Enum.IsDefined(typeof(ExpenseCategory), category))
                return "Kategorija troška nije ispravna.";

            return null;
        }

        public async Task<string?> ValidateForPlanAsync(PlanDbContext db, Guid travelPlanId, DateTime date)
        {
            var plan = await db.TravelPlans.FindAsync(travelPlanId);
            if (plan is null)
                return "Plan putovanja nije pronađen.";

            return date.Date < plan.StartDate.Date || date.Date > plan.EndDate.Date
                ? "Datum troška mora biti unutar perioda plana putovanja."
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

            return wouldExceedBudget ? "Ovaj trošak bi prešao planirani budžet." : null;
        }
    }
}

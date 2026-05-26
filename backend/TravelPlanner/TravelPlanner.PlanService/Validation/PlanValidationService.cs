using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.PlanService.Services.Common;

namespace TravelPlanner.PlanService.Validation
{
    internal sealed class PlanValidationService : IPlanValidationService
    {
        public string? ValidateCreate(string title, DateTime startDate, DateTime endDate, decimal plannedBudget)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "Title is required.";

            if (endDate < startDate)
                return "End date cannot be before start date.";

            if (plannedBudget < 0)
                return "Planned budget cannot be negative.";

            return null;
        }

        public bool ValidateUpdateFields(string title, DateTime startDate, DateTime endDate, decimal plannedBudget)
        {
            return !string.IsNullOrWhiteSpace(title)
                && endDate >= startDate
                && plannedBudget >= 0;
        }

        public async Task<bool> ValidateUpdateStateAsync(
            PlanDbContext db,
            Guid planId,
            DateTime startDate,
            DateTime endDate,
            decimal plannedBudget,
            IBudgetCalculationService budgetCalculationService)
        {
            var activitiesOutsideRange = await db.PlanActivities.AnyAsync(a =>
                a.TravelPlanId == planId &&
                (a.Date.Date < startDate.Date || a.Date.Date > endDate.Date));

            if (activitiesOutsideRange)
                return false;

            var destinationsOutsideRange = await db.Destinations.AnyAsync(d =>
                d.TravelPlanId == planId &&
                (d.ArrivalDate.Date < startDate.Date || d.DepartureDate.Date > endDate.Date));

            if (destinationsOutsideRange)
                return false;

            var expensesOutsideRange = await db.Expenses.AnyAsync(e =>
                e.TravelPlanId == planId &&
                (e.Date.Date < startDate.Date || e.Date.Date > endDate.Date));

            if (expensesOutsideRange)
                return false;

            var remindersOutsideRange = await db.Reminders.AnyAsync(r =>
                r.TravelPlanId == planId &&
                (r.RemindAt < startDate || r.RemindAt > endDate.AddDays(1).AddTicks(-1)));

            if (remindersOutsideRange)
                return false;

            var committedAmount = await budgetCalculationService.GetCommittedAmountAsync(db, planId);
            return committedAmount <= plannedBudget;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Services.Common
{
    internal sealed class BudgetCalculationService : IBudgetCalculationService
    {
        public async Task<decimal> GetCommittedAmountAsync(
            PlanDbContext db,
            Guid travelPlanId,
            Guid? excludeActivityId = null,
            Guid? excludeExpenseId = null)
        {
            var activitiesQuery = db.PlanActivities.Where(a => a.TravelPlanId == travelPlanId);
            if (excludeActivityId.HasValue)
            {
                activitiesQuery = activitiesQuery.Where(a => a.Id != excludeActivityId.Value);
            }

            var expensesQuery = db.Expenses.Where(e => e.TravelPlanId == travelPlanId);
            if (excludeExpenseId.HasValue)
            {
                expensesQuery = expensesQuery.Where(e => e.Id != excludeExpenseId.Value);
            }

            var activitiesTotal = await activitiesQuery
                .Select(a => (decimal?)a.EstimatedCost)
                .SumAsync() ?? 0m;

            var expensesTotal = await expensesQuery
                .Select(e => (decimal?)e.Amount)
                .SumAsync() ?? 0m;

            return activitiesTotal + expensesTotal;
        }

        public async Task<bool> WouldExceedBudgetWithActivityAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal newActivityCost,
            Guid? excludeActivityId = null)
        {
            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return true;

            var committedAmount = await GetCommittedAmountAsync(db, travelPlanId, excludeActivityId: excludeActivityId);
            return committedAmount + newActivityCost > plan.PlannedBudget;
        }

        public async Task<bool> WouldExceedBudgetWithExpenseAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal newExpenseAmount,
            Guid? excludeExpenseId = null)
        {
            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return true;

            var committedAmount = await GetCommittedAmountAsync(db, travelPlanId, excludeExpenseId: excludeExpenseId);
            return committedAmount + newExpenseAmount > plan.PlannedBudget;
        }
    }
}

using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Services.Common
{
    internal interface IBudgetCalculationService
    {
        Task<decimal> GetCommittedAmountAsync(
            PlanDbContext db,
            Guid travelPlanId,
            Guid? excludeActivityId = null,
            Guid? excludeExpenseId = null);

        Task<bool> WouldExceedBudgetWithActivityAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal newActivityCost,
            Guid? excludeActivityId = null);

        Task<bool> WouldExceedBudgetWithExpenseAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal newExpenseAmount,
            Guid? excludeExpenseId = null);
    }
}

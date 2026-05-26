using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.PlanService.Mappings
{
    internal static class ExpenseMapper
    {
        public static ExpenseData Map(Expense expense)
        {
            return new ExpenseData
            {
                Id = expense.Id,
                TravelPlanId = expense.TravelPlanId,
                Name = expense.Name,
                Category = expense.Category,
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description
            };
        }
    }
}

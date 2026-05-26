using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Models;

namespace TravelPlanner.PlanService.Services.Expenses
{
    internal interface IExpenseCrudService
    {
        Task<List<ExpenseData>> GetExpensesAsync(Guid travelPlanId);
        Task<ExpenseData?> GetExpenseByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ExpenseData>> CreateExpenseAsync(Guid travelPlanId, string name, ExpenseCategory category, decimal amount, DateTime date, string description);
        Task<bool> UpdateExpenseAsync(Guid travelPlanId, Guid id, string name, ExpenseCategory category, decimal amount, DateTime date, string description);
        Task<bool> DeleteExpenseAsync(Guid travelPlanId, Guid id);
    }
}

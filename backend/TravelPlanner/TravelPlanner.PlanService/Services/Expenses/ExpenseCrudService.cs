using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.PlanService.Mappings;
using TravelPlanner.PlanService.Services.Common;
using TravelPlanner.PlanService.Validation;

namespace TravelPlanner.PlanService.Services.Expenses
{
    internal sealed class ExpenseCrudService : IExpenseCrudService
    {
        private readonly IPlanServiceDbContextFactory _dbFactory;
        private readonly IExpenseValidationService _validationService;
        private readonly IBudgetCalculationService _budgetCalculationService;

        public ExpenseCrudService(
            IPlanServiceDbContextFactory dbFactory,
            IExpenseValidationService validationService,
            IBudgetCalculationService budgetCalculationService)
        {
            _dbFactory = dbFactory;
            _validationService = validationService;
            _budgetCalculationService = budgetCalculationService;
        }

        public async Task<List<ExpenseData>> GetExpensesAsync(Guid travelPlanId)
        {
            await using var db = _dbFactory.CreateDbContext();

            var expenses = await db.Expenses
                .AsNoTracking()
                .Where(e => e.TravelPlanId == travelPlanId)
                .ToListAsync();

            return expenses.Select(ExpenseMapper.Map).ToList();
        }

        public async Task<ExpenseData?> GetExpenseByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var expense = await db.Expenses
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            return expense is null ? null : ExpenseMapper.Map(expense);
        }

        public async Task<ServiceResponse<ExpenseData>> CreateExpenseAsync(
            Guid travelPlanId,
            string name,
            ExpenseCategory category,
            decimal amount,
            DateTime date,
            string description)
        {
            await using var db = _dbFactory.CreateDbContext();

            name = name?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            var fieldError = _validationService.ValidateFields(name, category, amount);
            if (fieldError is not null)
                return ServiceResponse<ExpenseData>.Fail(fieldError);

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, date);
            if (planError is not null)
                return ServiceResponse<ExpenseData>.Fail(planError);

            var budgetError = await _validationService.ValidateBudgetAsync(
                db,
                travelPlanId,
                amount,
                _budgetCalculationService);

            if (budgetError is not null)
                return ServiceResponse<ExpenseData>.Fail(budgetError);

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Name = name,
                Category = category,
                Amount = amount,
                Date = date,
                Description = description
            };

            db.Expenses.Add(expense);
            await db.SaveChangesAsync();

            return ServiceResponse<ExpenseData>.Ok(ExpenseMapper.Map(expense));
        }

        public async Task<bool> UpdateExpenseAsync(
            Guid travelPlanId,
            Guid id,
            string name,
            ExpenseCategory category,
            decimal amount,
            DateTime date,
            string description)
        {
            await using var db = _dbFactory.CreateDbContext();

            var expense = await db.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            if (expense is null)
                return false;

            name = name?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (_validationService.ValidateFields(name, category, amount) is not null)
                return false;

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, date);
            if (planError is not null)
                return false;

            var budgetError = await _validationService.ValidateBudgetAsync(
                db,
                travelPlanId,
                amount,
                _budgetCalculationService,
                id);

            if (budgetError is not null)
                return false;

            expense.Name = name;
            expense.Category = category;
            expense.Amount = amount;
            expense.Date = date;
            expense.Description = description;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExpenseAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var expense = await db.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            if (expense is null)
                return false;

            db.Expenses.Remove(expense);
            await db.SaveChangesAsync();
            return true;
        }
    }
}

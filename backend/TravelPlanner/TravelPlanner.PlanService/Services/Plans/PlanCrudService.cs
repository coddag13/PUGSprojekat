using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.PlanService.Mappings;
using TravelPlanner.PlanService.Services.Common;
using TravelPlanner.PlanService.Validation;

namespace TravelPlanner.PlanService.Services.Plans
{
    internal sealed class PlanCrudService : IPlanCrudService
    {
        private readonly IPlanServiceDbContextFactory _dbFactory;
        private readonly IBudgetCalculationService _budgetCalculationService;
        private readonly IPlanValidationService _validationService;

        public PlanCrudService(
            IPlanServiceDbContextFactory dbFactory,
            IBudgetCalculationService budgetCalculationService,
            IPlanValidationService validationService)
        {
            _dbFactory = dbFactory;
            _budgetCalculationService = budgetCalculationService;
            _validationService = validationService;
        }

        public async Task<List<TravelPlanData>> GetAllPlansByOwnerAsync(Guid ownerId)
        {
            await using var db = _dbFactory.CreateDbContext();

            var plans = await db.TravelPlans
                .AsNoTracking()
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();

            return plans.Select(TravelPlanMapper.Map).ToList();
        }

        public async Task<TravelPlanData?> GetPlanByIdAsync(Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var plan = await db.TravelPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return plan is null ? null : TravelPlanMapper.Map(plan);
        }

        public async Task<ServiceResponse<TravelPlanData>> CreatePlanAsync(
            Guid ownerId,
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            decimal plannedBudget,
            string notes)
        {
            await using var db = _dbFactory.CreateDbContext();

            title = title?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            notes = notes?.Trim() ?? string.Empty;

            var validationError = _validationService.ValidateCreate(title, startDate, endDate, plannedBudget);
            if (validationError is not null)
                return ServiceResponse<TravelPlanData>.Fail(validationError);

            var plan = new TravelPlan
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = title,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                PlannedBudget = plannedBudget,
                Notes = notes
            };

            db.TravelPlans.Add(plan);
            await db.SaveChangesAsync();

            return ServiceResponse<TravelPlanData>.Ok(TravelPlanMapper.Map(plan));
        }

        public async Task<bool> UpdatePlanAsync(
            Guid id,
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            decimal plannedBudget,
            string notes)
        {
            await using var db = _dbFactory.CreateDbContext();

            var plan = await db.TravelPlans.FindAsync(id);
            if (plan is null)
                return false;

            title = title?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            notes = notes?.Trim() ?? string.Empty;

            if (!_validationService.ValidateUpdateFields(title, startDate, endDate, plannedBudget))
                return false;

            var validState = await _validationService.ValidateUpdateStateAsync(
                db,
                id,
                startDate,
                endDate,
                plannedBudget,
                _budgetCalculationService);

            if (!validState)
                return false;

            plan.Title = title;
            plan.Description = description;
            plan.StartDate = startDate;
            plan.EndDate = endDate;
            plan.PlannedBudget = plannedBudget;
            plan.Notes = notes;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePlanAsync(Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var plan = await db.TravelPlans.FindAsync(id);
            if (plan is null)
                return false;

            db.TravelPlans.Remove(plan);
            await db.SaveChangesAsync();
            return true;
        }
    }
}

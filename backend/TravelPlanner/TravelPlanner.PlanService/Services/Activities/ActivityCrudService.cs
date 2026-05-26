using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.PlanService.Mappings;
using TravelPlanner.PlanService.Services.Common;
using TravelPlanner.PlanService.Validation;

namespace TravelPlanner.PlanService.Services.Activities
{
    internal sealed class ActivityCrudService : IActivityCrudService
    {
        private readonly IPlanServiceDbContextFactory _dbFactory;
        private readonly IActivityValidationService _validationService;
        private readonly IBudgetCalculationService _budgetCalculationService;

        public ActivityCrudService(
            IPlanServiceDbContextFactory dbFactory,
            IActivityValidationService validationService,
            IBudgetCalculationService budgetCalculationService)
        {
            _dbFactory = dbFactory;
            _validationService = validationService;
            _budgetCalculationService = budgetCalculationService;
        }

        public async Task<List<ActivityData>> GetActivitiesAsync(Guid travelPlanId)
        {
            await using var db = _dbFactory.CreateDbContext();

            var activities = await db.PlanActivities
                .AsNoTracking()
                .Where(a => a.TravelPlanId == travelPlanId)
                .ToListAsync();

            return activities.Select(ActivityMapper.Map).ToList();
        }

        public async Task<ActivityData?> GetActivityByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var activity = await db.PlanActivities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            return activity is null ? null : ActivityMapper.Map(activity);
        }

        public async Task<ServiceResponse<ActivityData>> CreateActivityAsync(
            Guid travelPlanId,
            Guid? destinationId,
            string name,
            DateTime date,
            TimeSpan time,
            string location,
            string description,
            decimal estimatedCost,
            ActivityStatus status)
        {
            await using var db = _dbFactory.CreateDbContext();

            name = name?.Trim() ?? string.Empty;
            location = location?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            var fieldError = _validationService.ValidateFields(name, location, estimatedCost, status);
            if (fieldError is not null)
                return ServiceResponse<ActivityData>.Fail(fieldError);

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, destinationId, date, time);
            if (planError is not null)
                return ServiceResponse<ActivityData>.Fail(planError);

            var budgetError = await _validationService.ValidateBudgetAsync(
                db,
                travelPlanId,
                estimatedCost,
                _budgetCalculationService);

            if (budgetError is not null)
                return ServiceResponse<ActivityData>.Fail(budgetError);

            var activity = new PlanActivity
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                DestinationId = destinationId,
                Name = name,
                Date = date,
                Time = time,
                Location = location,
                Description = description,
                EstimatedCost = estimatedCost,
                Status = status
            };

            db.PlanActivities.Add(activity);
            await db.SaveChangesAsync();

            return ServiceResponse<ActivityData>.Ok(ActivityMapper.Map(activity));
        }

        public async Task<bool> UpdateActivityAsync(
            Guid travelPlanId,
            Guid id,
            Guid? destinationId,
            string name,
            DateTime date,
            TimeSpan time,
            string location,
            string description,
            decimal estimatedCost,
            ActivityStatus status)
        {
            await using var db = _dbFactory.CreateDbContext();

            var activity = await db.PlanActivities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            if (activity is null)
                return false;

            name = name?.Trim() ?? string.Empty;
            location = location?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (_validationService.ValidateFields(name, location, estimatedCost, status) is not null)
                return false;

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, destinationId, date, time, id);
            if (planError is not null)
                return false;

            var budgetError = await _validationService.ValidateBudgetAsync(
                db,
                travelPlanId,
                estimatedCost,
                _budgetCalculationService,
                id);

            if (budgetError is not null)
                return false;

            activity.DestinationId = destinationId;
            activity.Name = name;
            activity.Date = date;
            activity.Time = time;
            activity.Location = location;
            activity.Description = description;
            activity.EstimatedCost = estimatedCost;
            activity.Status = status;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteActivityAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var activity = await db.PlanActivities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            if (activity is null)
                return false;

            db.PlanActivities.Remove(activity);
            await db.SaveChangesAsync();
            return true;
        }
    }
}

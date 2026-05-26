using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common.Enums;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.PlanService.Services.Common;

namespace TravelPlanner.PlanService.Validation
{
    internal sealed class ActivityValidationService : IActivityValidationService
    {
        public string? ValidateFields(string name, string location, decimal estimatedCost, ActivityStatus status)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Activity name is required.";

            if (string.IsNullOrWhiteSpace(location))
                return "Activity location is required.";

            if (estimatedCost < 0)
                return "Estimated cost cannot be negative.";

            if (!Enum.IsDefined(typeof(ActivityStatus), status))
                return "Invalid activity status.";

            return null;
        }

        public async Task<string?> ValidateForPlanAsync(
            PlanDbContext db,
            Guid travelPlanId,
            Guid? destinationId,
            DateTime date,
            TimeSpan time,
            Guid? excludeActivityId = null)
        {
            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return "Travel plan not found.";

            if (date.Date < plan.StartDate.Date || date.Date > plan.EndDate.Date)
                return "Activity date must be within the travel plan period.";

            if (destinationId.HasValue)
            {
                var destination = await db.Destinations
                    .FirstOrDefaultAsync(d => d.Id == destinationId.Value && d.TravelPlanId == travelPlanId);

                if (destination is null)
                    return "Destination not found in this travel plan.";

                if (date.Date < destination.ArrivalDate.Date || date.Date > destination.DepartureDate.Date)
                    return "Activity date must be within the selected destination period.";
            }

            var hasConflict = await db.PlanActivities.AnyAsync(a =>
                a.TravelPlanId == travelPlanId &&
                (!excludeActivityId.HasValue || a.Id != excludeActivityId.Value) &&
                a.Date.Date == date.Date &&
                a.Time == time);

            return hasConflict ? "Another activity already exists at the same date and time." : null;
        }

        public async Task<string?> ValidateBudgetAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal estimatedCost,
            IBudgetCalculationService budgetCalculationService,
            Guid? excludeActivityId = null)
        {
            var wouldExceedBudget = await budgetCalculationService.WouldExceedBudgetWithActivityAsync(
                db,
                travelPlanId,
                estimatedCost,
                excludeActivityId);

            return wouldExceedBudget ? "This activity would exceed the planned budget." : null;
        }
    }
}

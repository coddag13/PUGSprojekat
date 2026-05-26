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
                return "Naziv aktivnosti je obavezan.";

            if (string.IsNullOrWhiteSpace(location))
                return "Lokacija aktivnosti je obavezna.";

            if (estimatedCost < 0)
                return "Procijenjeni trošak ne može biti negativan.";

            if (!Enum.IsDefined(typeof(ActivityStatus), status))
                return "Status aktivnosti nije ispravan.";

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
                return "Plan putovanja nije pronađen.";

            if (date.Date < plan.StartDate.Date || date.Date > plan.EndDate.Date)
                return "Datum aktivnosti mora biti unutar perioda plana putovanja.";

            if (destinationId.HasValue)
            {
                var destination = await db.Destinations
                    .FirstOrDefaultAsync(d => d.Id == destinationId.Value && d.TravelPlanId == travelPlanId);

                if (destination is null)
                    return "Destinacija nije pronađena u ovom planu putovanja.";

                if (date.Date < destination.ArrivalDate.Date || date.Date > destination.DepartureDate.Date)
                    return "Datum aktivnosti mora biti unutar perioda odabrane destinacije.";
            }

            var hasConflict = await db.PlanActivities.AnyAsync(a =>
                a.TravelPlanId == travelPlanId &&
                (!excludeActivityId.HasValue || a.Id != excludeActivityId.Value) &&
                a.Date.Date == date.Date &&
                a.Time == time);

            return hasConflict ? "Već postoji aktivnost u istom datumu i terminu." : null;
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

            return wouldExceedBudget ? "Ova aktivnost bi prešla planirani budžet." : null;
        }
    }
}

using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Validation
{
    internal sealed class ReminderValidationService : IReminderValidationService
    {
        public string? ValidateTitle(string title)
        {
            return string.IsNullOrWhiteSpace(title) ? "Naslov podsjetnika je obavezan." : null;
        }

        public async Task<string?> ValidateForPlanAsync(PlanDbContext db, Guid travelPlanId, DateTime remindAt)
        {
            var plan = await db.TravelPlans.FindAsync(travelPlanId);
            if (plan is null)
                return "Plan putovanja nije pronađen.";

            return remindAt < plan.StartDate || remindAt > plan.EndDate.AddDays(1).AddTicks(-1)
                ? "Podsjetnik mora biti unutar perioda plana putovanja."
                : null;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Validation
{
    internal sealed class DestinationValidationService : IDestinationValidationService
    {
        public string? ValidateFields(string name, string location, DateTime arrivalDate, DateTime departureDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Naziv destinacije je obavezan.";

            if (string.IsNullOrWhiteSpace(location))
                return "Lokacija destinacije je obavezna.";

            if (departureDate < arrivalDate)
                return "Datum odlaska ne može biti prije datuma dolaska.";

            return null;
        }

        public async Task<string?> ValidateForPlanAsync(
            PlanDbContext db,
            Guid travelPlanId,
            DateTime arrivalDate,
            DateTime departureDate,
            Guid? excludeDestinationId = null)
        {
            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return "Plan putovanja nije pronađen.";

            if (arrivalDate.Date < plan.StartDate.Date)
                return "Datum dolaska ne može biti prije početka plana putovanja.";

            if (departureDate.Date > plan.EndDate.Date)
                return "Datum odlaska ne može biti poslije kraja plana putovanja.";

            var overlaps = await db.Destinations.AnyAsync(d =>
                d.TravelPlanId == travelPlanId &&
                (!excludeDestinationId.HasValue || d.Id != excludeDestinationId.Value) &&
                arrivalDate.Date <= d.DepartureDate.Date &&
                departureDate.Date >= d.ArrivalDate.Date);

            return overlaps ? "Datumi destinacije se preklapaju sa postojećom destinacijom." : null;
        }
    }
}

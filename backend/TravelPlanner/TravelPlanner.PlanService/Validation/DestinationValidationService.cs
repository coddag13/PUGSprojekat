using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Validation
{
    internal sealed class DestinationValidationService : IDestinationValidationService
    {
        public string? ValidateFields(string name, string location, DateTime arrivalDate, DateTime departureDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Destination name is required.";

            if (string.IsNullOrWhiteSpace(location))
                return "Destination location is required.";

            if (departureDate < arrivalDate)
                return "Departure date cannot be before arrival date.";

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
                return "Travel plan not found.";

            if (arrivalDate.Date < plan.StartDate.Date)
                return "Arrival date cannot be before the start of the travel plan.";

            if (departureDate.Date > plan.EndDate.Date)
                return "Departure date cannot be after the end of the travel plan.";

            var overlaps = await db.Destinations.AnyAsync(d =>
                d.TravelPlanId == travelPlanId &&
                (!excludeDestinationId.HasValue || d.Id != excludeDestinationId.Value) &&
                arrivalDate.Date <= d.DepartureDate.Date &&
                departureDate.Date >= d.ArrivalDate.Date);

            return overlaps ? "Destination dates overlap with an existing destination." : null;
        }
    }
}

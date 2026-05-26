using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Validation
{
    internal interface IDestinationValidationService
    {
        string? ValidateFields(string name, string location, DateTime arrivalDate, DateTime departureDate);
        Task<string?> ValidateForPlanAsync(
            PlanDbContext db,
            Guid travelPlanId,
            DateTime arrivalDate,
            DateTime departureDate,
            Guid? excludeDestinationId = null);
    }
}

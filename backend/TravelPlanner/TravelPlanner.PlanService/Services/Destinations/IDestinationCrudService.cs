using TravelPlanner.Common;
using TravelPlanner.Common.Models;

namespace TravelPlanner.PlanService.Services.Destinations
{
    internal interface IDestinationCrudService
    {
        Task<List<DestinationData>> GetDestinationsAsync(Guid travelPlanId);
        Task<DestinationData?> GetDestinationByIdAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<DestinationData>> CreateDestinationAsync(Guid travelPlanId, string name, string location, DateTime arrivalDate, DateTime departureDate, string description);
        Task<bool> UpdateDestinationAsync(Guid travelPlanId, Guid id, string name, string location, DateTime arrivalDate, DateTime departureDate, string description);
        Task<bool> DeleteDestinationAsync(Guid travelPlanId, Guid id);
    }
}

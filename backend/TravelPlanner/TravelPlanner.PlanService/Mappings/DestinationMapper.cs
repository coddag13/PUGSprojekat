using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.PlanService.Mappings
{
    internal static class DestinationMapper
    {
        public static DestinationData Map(Destination destination)
        {
            return new DestinationData
            {
                Id = destination.Id,
                TravelPlanId = destination.TravelPlanId,
                Name = destination.Name,
                Location = destination.Location,
                ArrivalDate = destination.ArrivalDate,
                DepartureDate = destination.DepartureDate,
                Description = destination.Description
            };
        }
    }
}

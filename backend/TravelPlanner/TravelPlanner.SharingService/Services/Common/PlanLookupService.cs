using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;

namespace TravelPlanner.SharingService.Services.Common
{
    internal sealed class PlanLookupService : IPlanLookupService
    {
        private static IPlanService PlanService =>
            ServiceProxy.Create<IPlanService>(
                new Uri("fabric:/TravelPlanner/TravelPlanner.PlanService"),
                new ServicePartitionKey(0));

        public Task<TravelPlanData?> GetPlanByIdAsync(Guid travelPlanId)
        {
            return PlanService.GetPlanByIdAsync(travelPlanId);
        }
    }
}

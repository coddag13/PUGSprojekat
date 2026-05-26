using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.SharingService.Mappings
{
    internal static class ShareTokenMapper
    {
        public static ShareTokenData Map(ShareToken token) => new()
        {
            Id = token.Id,
            TravelPlanId = token.TravelPlanId,
            Token = token.Token,
            AccessType = token.AccessType,
            ExpiresAt = token.ExpiresAt
        };
    }
}

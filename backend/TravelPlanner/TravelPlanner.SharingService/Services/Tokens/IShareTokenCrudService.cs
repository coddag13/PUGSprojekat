using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Models;

namespace TravelPlanner.SharingService.Services.Tokens
{
    internal interface IShareTokenCrudService
    {
        Task<List<ShareTokenData>> GetTokensByPlanAsync(Guid travelPlanId);
        Task<ServiceResponse<ShareTokenData>> CreateTokenAsync(Guid travelPlanId, ShareAccessType accessType, DateTime expiresAt);
        Task<bool> DeleteTokenAsync(Guid travelPlanId, Guid id);
        Task<ServiceResponse<ShareTokenData>> ValidateTokenAsync(string token);
        Task<ServiceResponse<ShareTokenData>> ValidateTokenForPlanAsync(Guid travelPlanId, string token);
        Task DeleteTokensByPlanAsync(Guid travelPlanId);
    }
}

using TravelPlanner.Common.Models;

namespace TravelPlanner.SharingService.Validation
{
    internal interface IShareTokenValidationService
    {
        string? ValidateTokenValue(string token);
        string? ValidateExpiry(DateTime expiresAt);
        string? ValidateTokenBelongsToPlan(ShareTokenData token, Guid travelPlanId);
    }
}

using TravelPlanner.Common.Models;

namespace TravelPlanner.SharingService.Validation
{
    internal sealed class ShareTokenValidationService : IShareTokenValidationService
    {
        public string? ValidateTokenValue(string token)
        {
            return string.IsNullOrWhiteSpace(token) ? "Token is required." : null;
        }

        public string? ValidateExpiry(DateTime expiresAt)
        {
            return expiresAt <= DateTime.UtcNow ? "Expiry date must be in the future." : null;
        }

        public string? ValidateTokenBelongsToPlan(ShareTokenData token, Guid travelPlanId)
        {
            return token.TravelPlanId != travelPlanId
                ? "Token does not belong to this travel plan."
                : null;
        }
    }
}

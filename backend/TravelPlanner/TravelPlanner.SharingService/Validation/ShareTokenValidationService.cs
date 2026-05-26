using TravelPlanner.Common.Models;

namespace TravelPlanner.SharingService.Validation
{
    internal sealed class ShareTokenValidationService : IShareTokenValidationService
    {
        public string? ValidateTokenValue(string token)
        {
            return string.IsNullOrWhiteSpace(token) ? "Token je obavezan." : null;
        }

        public string? ValidateExpiry(DateTime expiresAt)
        {
            return expiresAt <= DateTime.UtcNow ? "Datum isteka mora biti u budućnosti." : null;
        }

        public string? ValidateTokenBelongsToPlan(ShareTokenData token, Guid travelPlanId)
        {
            return token.TravelPlanId != travelPlanId
                ? "Token ne pripada ovom planu putovanja."
                : null;
        }
    }
}

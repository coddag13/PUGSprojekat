using TravelPlanner.Common.Enums;

namespace TravelPlanner.Infrastructure.Entities
{
    public class ShareToken
    {
        public Guid Id { get; set; }

        public Guid TravelPlanId { get; set; }

        public string Token { get; set; } = string.Empty;

        public ShareAccessType AccessType { get; set; }

        public DateTime ExpiresAt { get; set; }

        public TravelPlan TravelPlan { get; set; } = null!;
    }
}

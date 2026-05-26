using TravelPlanner.Common.Enums;

namespace TravelPlanner.Common.Models
{
    public class ShareTokenData
    {
        public Guid Id { get; set; }
        public Guid TravelPlanId { get; set; }
        public string Token { get; set; } = string.Empty;
        public ShareAccessType AccessType { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

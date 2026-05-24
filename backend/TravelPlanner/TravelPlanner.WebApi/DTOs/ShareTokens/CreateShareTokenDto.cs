using TravelPlanner.Common.Enums;

namespace TravelPlanner.WebApi.DTOs.ShareTokens
{
    public class CreateShareTokenDto
    {
        public ShareAccessType AccessType { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
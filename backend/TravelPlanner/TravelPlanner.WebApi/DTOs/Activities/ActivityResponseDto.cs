using TravelPlanner.Common.Enums;

namespace TravelPlanner.WebApi.DTOs.Activities
{
    public class ActivityResponseDto
    {
        public Guid Id { get; set; }
        public Guid TravelPlanId { get; set; }
        public Guid? DestinationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public ActivityStatus Status { get; set; }
    }
}
namespace TravelPlanner.WebApi.DTOs.Admin
{
    public class AdminTravelPlanResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PlannedBudget { get; set; }
        public string OwnerEmail { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
    }
}

namespace TravelPlanner.Infrastructure.Entities
{
    public class Reminder
    {
        public Guid Id { get; set; }

        public Guid TravelPlanId { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime RemindAt { get; set; }

        public bool IsCompleted { get; set; }

        public TravelPlan TravelPlan { get; set; } = null!;
    }
}

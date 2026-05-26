namespace TravelPlanner.WebApi.DTOs.Reminders
{
    public class ReminderResponseDto
    {
        public Guid Id { get; set; }
        public Guid TravelPlanId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime RemindAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}

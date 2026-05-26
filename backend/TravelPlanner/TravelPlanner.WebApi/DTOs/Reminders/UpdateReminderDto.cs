namespace TravelPlanner.WebApi.DTOs.Reminders
{
    public class UpdateReminderDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime RemindAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}

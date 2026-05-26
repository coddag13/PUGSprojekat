namespace TravelPlanner.WebApi.DTOs.Reminders
{
    public class CreateReminderDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime RemindAt { get; set; }
    }
}

namespace TravelPlanner.WebApi.DTOs.ChecklistItems
{
    public class UpdateChecklistItemDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
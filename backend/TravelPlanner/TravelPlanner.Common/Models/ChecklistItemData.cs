namespace TravelPlanner.Common.Models
{
    public class ChecklistItemData
    {
        public Guid Id { get; set; }
        public Guid TravelPlanId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}

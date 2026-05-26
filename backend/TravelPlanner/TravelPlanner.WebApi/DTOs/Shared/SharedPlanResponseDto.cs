using TravelPlanner.Common.Enums;
using TravelPlanner.WebApi.DTOs.Activities;
using TravelPlanner.WebApi.DTOs.ChecklistItems;
using TravelPlanner.WebApi.DTOs.Destinations;
using TravelPlanner.WebApi.DTOs.Expenses;
using TravelPlanner.WebApi.DTOs.Reminders;
using TravelPlanner.WebApi.DTOs.TravelPlans;

namespace TravelPlanner.WebApi.DTOs.Shared
{
    public class SharedPlanResponseDto
    {
        public ShareAccessType AccessType { get; set; }
        public TravelPlanResponseDto Plan { get; set; } = new();
        public List<DestinationResponseDto> Destinations { get; set; } = [];
        public List<ActivityResponseDto> Activities { get; set; } = [];
        public List<ExpenseResponseDto> Expenses { get; set; } = [];
        public List<ChecklistItemResponseDto> ChecklistItems { get; set; } = [];
        public List<ReminderResponseDto> Reminders { get; set; } = [];
    }
}

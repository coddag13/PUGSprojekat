using TravelPlanner.Common.Models;
using TravelPlanner.WebApi.DTOs.Activities;
using TravelPlanner.WebApi.DTOs.ChecklistItems;
using TravelPlanner.WebApi.DTOs.Destinations;
using TravelPlanner.WebApi.DTOs.Expenses;
using TravelPlanner.WebApi.DTOs.Reminders;
using TravelPlanner.WebApi.DTOs.ShareTokens;
using TravelPlanner.WebApi.DTOs.Shared;
using TravelPlanner.WebApi.DTOs.TravelPlans;

namespace TravelPlanner.WebApi.Mappings
{
    internal static class ResponseMappers
    {
        public static TravelPlanResponseDto ToTravelPlanResponse(this TravelPlanData plan) => new()
        {
            Id = plan.Id,
            OwnerId = plan.OwnerId,
            Title = plan.Title,
            Description = plan.Description,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            PlannedBudget = plan.PlannedBudget,
            Notes = plan.Notes
        };

        public static DestinationResponseDto ToDestinationResponse(this DestinationData destination) => new()
        {
            Id = destination.Id,
            TravelPlanId = destination.TravelPlanId,
            Name = destination.Name,
            Location = destination.Location,
            ArrivalDate = destination.ArrivalDate,
            DepartureDate = destination.DepartureDate,
            Description = destination.Description
        };

        public static ActivityResponseDto ToActivityResponse(this ActivityData activity) => new()
        {
            Id = activity.Id,
            TravelPlanId = activity.TravelPlanId,
            DestinationId = activity.DestinationId,
            Name = activity.Name,
            Date = activity.Date,
            Time = activity.Time,
            Location = activity.Location,
            Description = activity.Description,
            EstimatedCost = activity.EstimatedCost,
            Status = activity.Status
        };

        public static ExpenseResponseDto ToExpenseResponse(this ExpenseData expense) => new()
        {
            Id = expense.Id,
            TravelPlanId = expense.TravelPlanId,
            Name = expense.Name,
            Category = expense.Category,
            Amount = expense.Amount,
            Date = expense.Date,
            Description = expense.Description
        };

        public static ChecklistItemResponseDto ToChecklistResponse(this ChecklistItemData item) => new()
        {
            Id = item.Id,
            TravelPlanId = item.TravelPlanId,
            Text = item.Text,
            IsCompleted = item.IsCompleted
        };

        public static ReminderResponseDto ToReminderResponse(this ReminderData reminder) => new()
        {
            Id = reminder.Id,
            TravelPlanId = reminder.TravelPlanId,
            Title = reminder.Title,
            RemindAt = reminder.RemindAt,
            IsCompleted = reminder.IsCompleted
        };

        public static ShareTokenResponseDto ToShareTokenResponse(this ShareTokenData token) => new()
        {
            Id = token.Id,
            TravelPlanId = token.TravelPlanId,
            Token = token.Token,
            AccessType = token.AccessType,
            ExpiresAt = token.ExpiresAt
        };

        public static SharedPlanResponseDto ToSharedPlanResponse(
            this ShareTokenData shareToken,
            TravelPlanData plan,
            IEnumerable<DestinationData> destinations,
            IEnumerable<ActivityData> activities,
            IEnumerable<ExpenseData> expenses,
            IEnumerable<ChecklistItemData> checklistItems,
            IEnumerable<ReminderData> reminders) => new()
        {
            AccessType = shareToken.AccessType,
            Plan = plan.ToTravelPlanResponse(),
            Destinations = destinations.Select(ToDestinationResponse).ToList(),
            Activities = activities.Select(ToActivityResponse).ToList(),
            Expenses = expenses.Select(ToExpenseResponse).ToList(),
            ChecklistItems = checklistItems.Select(ToChecklistResponse).ToList(),
            Reminders = reminders.Select(ToReminderResponse).ToList()
        };
    }
}

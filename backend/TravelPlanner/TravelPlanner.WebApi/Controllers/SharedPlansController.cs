using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.WebApi.DTOs.Activities;
using TravelPlanner.WebApi.DTOs.ChecklistItems;
using TravelPlanner.WebApi.DTOs.Destinations;
using TravelPlanner.WebApi.DTOs.Expenses;
using TravelPlanner.WebApi.DTOs.Reminders;
using TravelPlanner.WebApi.DTOs.Shared;
using TravelPlanner.WebApi.DTOs.TravelPlans;

namespace TravelPlanner.WebApi.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/shared-plans/{token}")]
    public class SharedPlansController : ControllerBase
    {
        private static ISharingService SharingService =>
            ServiceProxy.Create<ISharingService>(new Uri("fabric:/TravelPlanner/TravelPlanner.SharingService"));

        private static IPlanService PlanService =>
            ServiceProxy.Create<IPlanService>(
                new Uri("fabric:/TravelPlanner/TravelPlanner.PlanService"),
                new ServicePartitionKey(0));

        [HttpGet]
        public async Task<IActionResult> GetSharedPlan(string token)
        {
            var tokenResult = await SharingService.ValidateTokenAsync(token);
            if (!tokenResult.Success)
                return Unauthorized(tokenResult.Error);

            var shared = await BuildSharedPlanResponseAsync(tokenResult.Data!);
            return Ok(shared);
        }

        [HttpPut("activities/{id:guid}")]
        public async Task<IActionResult> UpdateActivityStatus(string token, Guid id, UpdateSharedActivityStatusDto dto)
        {
            var tokenResult = await ValidateSharedEditAccessAsync(token);
            if (tokenResult.Result is not null)
                return tokenResult.Result;

            var shareToken = tokenResult.Token!;
            var activity = await PlanService.GetActivityByIdAsync(shareToken.TravelPlanId, id);
            if (activity is null)
                return NotFound();

            var updated = await PlanService.UpdateActivityAsync(
                shareToken.TravelPlanId,
                id,
                activity.DestinationId,
                activity.Name,
                activity.Date,
                activity.Time,
                activity.Location,
                activity.Description,
                activity.EstimatedCost,
                dto.Status);

            if (!updated)
                return BadRequest("Activity status could not be updated.");

            return NoContent();
        }

        [HttpPost("checklist-items")]
        public async Task<IActionResult> CreateChecklistItem(string token, CreateChecklistItemDto dto)
        {
            var tokenResult = await ValidateSharedEditAccessAsync(token);
            if (tokenResult.Result is not null)
                return tokenResult.Result;

            var shareToken = tokenResult.Token!;
            var result = await PlanService.CreateChecklistItemAsync(shareToken.TravelPlanId, dto.Text);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(MapToChecklistResponse(result.Data!));
        }

        [HttpPut("checklist-items/{id:guid}")]
        public async Task<IActionResult> UpdateChecklistItem(string token, Guid id, UpdateChecklistItemDto dto)
        {
            var tokenResult = await ValidateSharedEditAccessAsync(token);
            if (tokenResult.Result is not null)
                return tokenResult.Result;

            var shareToken = tokenResult.Token!;
            var updated = await PlanService.UpdateChecklistItemAsync(
                shareToken.TravelPlanId,
                id,
                dto.Text,
                dto.IsCompleted);

            if (!updated)
                return BadRequest("Checklist item could not be updated.");

            return NoContent();
        }

        private async Task<(ShareTokenData? Token, IActionResult? Result)> ValidateSharedEditAccessAsync(string token)
        {
            var tokenResult = await SharingService.ValidateTokenAsync(token);
            if (!tokenResult.Success)
                return (null, Unauthorized(tokenResult.Error));

            if (tokenResult.Data!.AccessType != ShareAccessType.Edit)
                return (null, StatusCode(StatusCodes.Status403Forbidden, "This token has view-only access."));

            return (tokenResult.Data, null);
        }

        private async Task<SharedPlanResponseDto> BuildSharedPlanResponseAsync(ShareTokenData shareToken)
        {
            var plan = await PlanService.GetPlanByIdAsync(shareToken.TravelPlanId)
                ?? throw new InvalidOperationException("Travel plan not found for this token.");

            var destinations = await PlanService.GetDestinationsAsync(shareToken.TravelPlanId);
            var activities = await PlanService.GetActivitiesAsync(shareToken.TravelPlanId);
            var expenses = await PlanService.GetExpensesAsync(shareToken.TravelPlanId);
            var checklistItems = await PlanService.GetChecklistItemsAsync(shareToken.TravelPlanId);
            var reminders = await PlanService.GetRemindersAsync(shareToken.TravelPlanId);

            return new SharedPlanResponseDto
            {
                AccessType = shareToken.AccessType,
                Plan = MapToTravelPlanResponse(plan),
                Destinations = destinations.Select(MapToDestinationResponse).ToList(),
                Activities = activities.Select(MapToActivityResponse).ToList(),
                Expenses = expenses.Select(MapToExpenseResponse).ToList(),
                ChecklistItems = checklistItems.Select(MapToChecklistResponse).ToList(),
                Reminders = reminders.Select(MapToReminderResponse).ToList()
            };
        }

        private static TravelPlanResponseDto MapToTravelPlanResponse(TravelPlanData plan)
        {
            return new TravelPlanResponseDto
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
        }

        private static DestinationResponseDto MapToDestinationResponse(DestinationData destination)
        {
            return new DestinationResponseDto
            {
                Id = destination.Id,
                TravelPlanId = destination.TravelPlanId,
                Name = destination.Name,
                Location = destination.Location,
                ArrivalDate = destination.ArrivalDate,
                DepartureDate = destination.DepartureDate,
                Description = destination.Description
            };
        }

        private static ActivityResponseDto MapToActivityResponse(ActivityData activity)
        {
            return new ActivityResponseDto
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
        }

        private static ExpenseResponseDto MapToExpenseResponse(ExpenseData expense)
        {
            return new ExpenseResponseDto
            {
                Id = expense.Id,
                TravelPlanId = expense.TravelPlanId,
                Name = expense.Name,
                Category = expense.Category,
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description
            };
        }

        private static ChecklistItemResponseDto MapToChecklistResponse(ChecklistItemData item)
        {
            return new ChecklistItemResponseDto
            {
                Id = item.Id,
                TravelPlanId = item.TravelPlanId,
                Text = item.Text,
                IsCompleted = item.IsCompleted
            };
        }

        private static ReminderResponseDto MapToReminderResponse(ReminderData reminder)
        {
            return new ReminderResponseDto
            {
                Id = reminder.Id,
                TravelPlanId = reminder.TravelPlanId,
                Title = reminder.Title,
                RemindAt = reminder.RemindAt,
                IsCompleted = reminder.IsCompleted
            };
        }
    }
}

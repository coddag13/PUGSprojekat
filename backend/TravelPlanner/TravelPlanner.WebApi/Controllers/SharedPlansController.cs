using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.WebApi.DTOs.Activities;
using TravelPlanner.WebApi.DTOs.ChecklistItems;
using TravelPlanner.WebApi.DTOs.Shared;
using TravelPlanner.WebApi.Extensions;
using TravelPlanner.WebApi.Mappings;

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
                return BadRequest("Status aktivnosti nije bilo moguće izmijeniti.");

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

            return Ok(result.Data!.ToChecklistResponse());
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
                return BadRequest("Stavku liste nije bilo moguće izmijeniti.");

            return NoContent();
        }

        private async Task<(ShareTokenData? Token, IActionResult? Result)> ValidateSharedEditAccessAsync(string token)
        {
            var tokenResult = await SharingService.ValidateTokenAsync(token);
            if (!tokenResult.Success)
                return (null, Unauthorized(tokenResult.Error));

            if (tokenResult.Data!.AccessType != ShareAccessType.Edit)
                return (null, StatusCode(StatusCodes.Status403Forbidden, "Ovaj token ima samo pristup za pregled."));

            return (tokenResult.Data, null);
        }

        private async Task<SharedPlanResponseDto> BuildSharedPlanResponseAsync(ShareTokenData shareToken)
        {
            var plan = await PlanService.GetPlanByIdAsync(shareToken.TravelPlanId)
                ?? throw new InvalidOperationException("Plan putovanja nije pronađen za ovaj token.");

            var destinations = await PlanService.GetDestinationsAsync(shareToken.TravelPlanId);
            var activities = await PlanService.GetActivitiesAsync(shareToken.TravelPlanId);
            var expenses = await PlanService.GetExpensesAsync(shareToken.TravelPlanId);
            var checklistItems = await PlanService.GetChecklistItemsAsync(shareToken.TravelPlanId);
            var reminders = await PlanService.GetRemindersAsync(shareToken.TravelPlanId);

            return shareToken.ToSharedPlanResponse(
                plan,
                destinations,
                activities,
                expenses,
                checklistItems,
                reminders);
        }
    }
}

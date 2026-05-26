using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.WebApi.DTOs.Reminders;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/reminders")]
    public class RemindersController : ControllerBase
    {
        private static IPlanService PlanService =>
            ServiceProxy.Create<IPlanService>(
                new Uri("fabric:/TravelPlanner/TravelPlanner.PlanService"),
                new ServicePartitionKey(0));

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid travelPlanId)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var reminders = await PlanService.GetRemindersAsync(travelPlanId);
            return Ok(reminders.Select(MapToResponse));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid travelPlanId, Guid id)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var reminder = await PlanService.GetReminderByIdAsync(travelPlanId, id);
            if (reminder is null)
                return NotFound();

            return Ok(MapToResponse(reminder));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid travelPlanId, CreateReminderDto dto)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var result = await PlanService.CreateReminderAsync(travelPlanId, dto.Title, dto.RemindAt);
            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { travelPlanId, id = result.Data!.Id }, MapToResponse(result.Data));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateReminderDto dto)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var updated = await PlanService.UpdateReminderAsync(travelPlanId, id, dto.Title, dto.RemindAt, dto.IsCompleted);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid travelPlanId, Guid id)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var deleted = await PlanService.DeleteReminderAsync(travelPlanId, id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private async Task<IActionResult?> EnsurePlanOwnership(Guid travelPlanId)
        {
            var ownerId = GetOwnerId();
            var plan = await PlanService.GetPlanByIdAsync(travelPlanId);

            if (plan is null)
                return NotFound("Plan putovanja nije pronađen.");

            if (plan.OwnerId != ownerId && !IsAdmin())
                return Forbid();

            return null;
        }

        private Guid GetOwnerId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            return Guid.Parse(claim!.Value);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        private static ReminderResponseDto MapToResponse(ReminderData reminder)
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

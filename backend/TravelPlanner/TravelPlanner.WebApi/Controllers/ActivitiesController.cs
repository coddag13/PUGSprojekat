using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.WebApi.DTOs.Activities;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/activities")]
    public class ActivitiesController : ControllerBase
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

            var items = await PlanService.GetActivitiesAsync(travelPlanId);
            return Ok(items.Select(MapToResponse));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid travelPlanId, Guid id)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var activity = await PlanService.GetActivityByIdAsync(travelPlanId, id);
            if (activity is null)
                return NotFound();

            return Ok(MapToResponse(activity));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid travelPlanId, CreateActivityDto dto)
        {
            if (dto.EstimatedCost < 0)
                return BadRequest("Estimated cost cannot be negative.");

            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var result = await PlanService.CreateActivityAsync(
                travelPlanId,
                dto.DestinationId,
                dto.Name,
                dto.Date,
                dto.Time,
                dto.Location,
                dto.Description,
                dto.EstimatedCost,
                dto.Status);

            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { travelPlanId, id = result.Data!.Id }, MapToResponse(result.Data));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateActivityDto dto)
        {
            if (dto.EstimatedCost < 0)
                return BadRequest("Estimated cost cannot be negative.");

            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var updated = await PlanService.UpdateActivityAsync(
                travelPlanId,
                id,
                dto.DestinationId,
                dto.Name,
                dto.Date,
                dto.Time,
                dto.Location,
                dto.Description,
                dto.EstimatedCost,
                dto.Status);

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

            var deleted = await PlanService.DeleteActivityAsync(travelPlanId, id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private async Task<IActionResult?> EnsurePlanOwnership(Guid travelPlanId)
        {
            var ownerId = GetOwnerId();
            var plan = await PlanService.GetPlanByIdAsync(travelPlanId);

            if (plan is null)
                return NotFound("Travel plan not found.");

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

        private static ActivityResponseDto MapToResponse(ActivityData activity)
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
    }
}

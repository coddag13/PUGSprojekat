using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.WebApi.DTOs.Activities;
using TravelPlanner.WebApi.Extensions;
using TravelPlanner.WebApi.Mappings;

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
            return Ok(items.Select(activity => activity.ToActivityResponse()));
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

            return Ok(activity.ToActivityResponse());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid travelPlanId, CreateActivityDto dto)
        {
            if (dto.EstimatedCost < 0)
                return BadRequest("Procijenjeni trošak ne može biti negativan.");

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

            return CreatedAtAction(
                nameof(GetById),
                new { travelPlanId, id = result.Data!.Id },
                result.Data.ToActivityResponse());
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateActivityDto dto)
        {
            if (dto.EstimatedCost < 0)
                return BadRequest("Procijenjeni trošak ne može biti negativan.");

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
            var ownerId = User.GetUserId();
            var plan = await PlanService.GetPlanByIdAsync(travelPlanId);

            if (plan is null)
                return NotFound("Plan putovanja nije pronađen.");

            if (plan.OwnerId != ownerId && !User.IsAdminUser())
                return Forbid();

            return null;
        }
    }
}

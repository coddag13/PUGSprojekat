using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.WebApi.DTOs.Destinations;
using TravelPlanner.WebApi.Extensions;
using TravelPlanner.WebApi.Mappings;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/destinations")]
    public class DestinationsController : ControllerBase
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

            var items = await PlanService.GetDestinationsAsync(travelPlanId);
            return Ok(items.Select(destination => destination.ToDestinationResponse()));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid travelPlanId, Guid id)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var destination = await PlanService.GetDestinationByIdAsync(travelPlanId, id);
            if (destination is null)
                return NotFound();

            return Ok(destination.ToDestinationResponse());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid travelPlanId, CreateDestinationDto dto)
        {
            if (dto.DepartureDate < dto.ArrivalDate)
                return BadRequest("Datum odlaska ne može biti prije datuma dolaska.");

            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var result = await PlanService.CreateDestinationAsync(
                travelPlanId,
                dto.Name,
                dto.Location,
                dto.ArrivalDate,
                dto.DepartureDate,
                dto.Description);

            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(GetById),
                new { travelPlanId, id = result.Data!.Id },
                result.Data.ToDestinationResponse());
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateDestinationDto dto)
        {
            if (dto.DepartureDate < dto.ArrivalDate)
                return BadRequest("Datum odlaska ne može biti prije datuma dolaska.");

            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var updated = await PlanService.UpdateDestinationAsync(
                travelPlanId,
                id,
                dto.Name,
                dto.Location,
                dto.ArrivalDate,
                dto.DepartureDate,
                dto.Description);

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

            var deleted = await PlanService.DeleteDestinationAsync(travelPlanId, id);
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

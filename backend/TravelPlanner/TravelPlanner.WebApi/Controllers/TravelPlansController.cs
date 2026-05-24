using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.WebApi.DTOs.TravelPlans;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans")]
    public class TravelPlansController : ControllerBase
    {
        private static IPlanService PlanService =>
            ServiceProxy.Create<IPlanService>(
                new Uri("fabric:/TravelPlanner/TravelPlanner.PlanService"),
                new ServicePartitionKey(0));

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ownerId = GetOwnerId();
            var plans = await PlanService.GetAllPlansByOwnerAsync(ownerId);

            return Ok(plans.Select(MapToResponse));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ownerId = GetOwnerId();
            var plan = await PlanService.GetPlanByIdAsync(id);

            if (plan is null)
                return NotFound();

            if (plan.OwnerId != ownerId)
                return Forbid();

            return Ok(MapToResponse(plan));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTravelPlanDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest("End date cannot be before start date.");

            if (dto.PlannedBudget < 0)
                return BadRequest("Budget cannot be negative.");

            var ownerId = GetOwnerId();
            var result = await PlanService.CreatePlanAsync(
                ownerId,
                dto.Title,
                dto.Description,
                dto.StartDate,
                dto.EndDate,
                dto.PlannedBudget,
                dto.Notes);

            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, MapToResponse(result.Data));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTravelPlanDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest("End date cannot be before start date.");

            if (dto.PlannedBudget < 0)
                return BadRequest("Budget cannot be negative.");

            var ownerId = GetOwnerId();
            var existingPlan = await PlanService.GetPlanByIdAsync(id);

            if (existingPlan is null)
                return NotFound();

            if (existingPlan.OwnerId != ownerId)
                return Forbid();

            var updated = await PlanService.UpdatePlanAsync(
                id,
                dto.Title,
                dto.Description,
                dto.StartDate,
                dto.EndDate,
                dto.PlannedBudget,
                dto.Notes);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ownerId = GetOwnerId();
            var existingPlan = await PlanService.GetPlanByIdAsync(id);

            if (existingPlan is null)
                return NotFound();

            if (existingPlan.OwnerId != ownerId)
                return Forbid();

            var deleted = await PlanService.DeletePlanAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private static TravelPlanResponseDto MapToResponse(TravelPlanner.Common.Models.TravelPlanData plan)
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

        private Guid GetOwnerId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            return Guid.Parse(claim!.Value);
        }
    }
}
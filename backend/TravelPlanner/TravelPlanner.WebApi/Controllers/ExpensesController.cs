using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.WebApi.DTOs.Expenses;
using TravelPlanner.WebApi.Extensions;
using TravelPlanner.WebApi.Mappings;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/expenses")]
    public class ExpensesController : ControllerBase
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

            var items = await PlanService.GetExpensesAsync(travelPlanId);
            return Ok(items.Select(expense => expense.ToExpenseResponse()));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid travelPlanId, Guid id)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var expense = await PlanService.GetExpenseByIdAsync(travelPlanId, id);
            if (expense is null)
                return NotFound();

            return Ok(expense.ToExpenseResponse());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid travelPlanId, CreateExpenseDto dto)
        {
            if (dto.Amount < 0)
                return BadRequest("Iznos ne može biti negativan.");

            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var result = await PlanService.CreateExpenseAsync(
                travelPlanId,
                dto.Name,
                dto.Category,
                dto.Amount,
                dto.Date,
                dto.Description);

            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(GetById),
                new { travelPlanId, id = result.Data!.Id },
                result.Data.ToExpenseResponse());
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateExpenseDto dto)
        {
            if (dto.Amount < 0)
                return BadRequest("Iznos ne može biti negativan.");

            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var updated = await PlanService.UpdateExpenseAsync(
                travelPlanId,
                id,
                dto.Name,
                dto.Category,
                dto.Amount,
                dto.Date,
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

            var deleted = await PlanService.DeleteExpenseAsync(travelPlanId, id);
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

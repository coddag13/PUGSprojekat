using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.WebApi.DTOs.ShareTokens;
using TravelPlanner.WebApi.Extensions;
using TravelPlanner.WebApi.Mappings;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/share-tokens")]
    public class ShareTokensController : ControllerBase
    {
        private static ISharingService SharingService =>
            ServiceProxy.Create<ISharingService>(new Uri("fabric:/TravelPlanner/TravelPlanner.SharingService"));

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

            var tokens = await SharingService.GetTokensByPlanAsync(travelPlanId);
            return Ok(tokens.Select(token => token.ToShareTokenResponse()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid travelPlanId, CreateShareTokenDto dto)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var result = await SharingService.CreateTokenAsync(travelPlanId, dto.AccessType, dto.ExpiresAt);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(result.Data!.ToShareTokenResponse());
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid travelPlanId, Guid id)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var deleted = await SharingService.DeleteTokenAsync(travelPlanId, id);
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

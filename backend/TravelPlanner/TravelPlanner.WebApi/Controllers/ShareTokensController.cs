using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.WebApi.DTOs.ShareTokens;

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
            ServiceProxy.Create<IPlanService>(new Uri("fabric:/TravelPlanner/TravelPlanner.PlanService"));

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid travelPlanId)
        {
            var ownershipResult = await EnsurePlanOwnership(travelPlanId);
            if (ownershipResult is not null)
                return ownershipResult;

            var tokens = await SharingService.GetTokensByPlanAsync(travelPlanId);
            return Ok(tokens.Select(MapToResponse));
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

            return Ok(MapToResponse(result.Data!));
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

        [AllowAnonymous]
        [HttpGet("access/{token}")]
        public async Task<IActionResult> GetByToken(Guid travelPlanId, string token)
        {
            var result = await SharingService.ValidateTokenAsync(travelPlanId, token);
            if (!result.Success)
                return Unauthorized(result.Error);

            return Ok(MapToResponse(result.Data!));
        }

        private async Task<IActionResult?> EnsurePlanOwnership(Guid travelPlanId)
        {
            var ownerId = GetOwnerId();
            var plan = await PlanService.GetPlanByIdAsync(travelPlanId);

            if (plan is null)
                return NotFound("Travel plan not found.");

            if (plan.OwnerId != ownerId)
                return Forbid();

            return null;
        }

        private Guid GetOwnerId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            return Guid.Parse(claim!.Value);
        }

        private static ShareTokenResponseDto MapToResponse(ShareTokenData token)
        {
            return new ShareTokenResponseDto
            {
                Id = token.Id,
                TravelPlanId = token.TravelPlanId,
                Token = token.Token,
                AccessType = token.AccessType,
                ExpiresAt = token.ExpiresAt
            };
        }
    }
}
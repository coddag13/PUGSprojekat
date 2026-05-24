using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.ShareTokens;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/share-tokens")]
    public class ShareTokensController : ControllerBase
    {
        private readonly TravelPlannerDbContext _context;

        public ShareTokensController(TravelPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShareTokenResponseDto>>> GetAll(Guid travelPlanId)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            var tokens = await _context.ShareTokens
                .Where(t => t.TravelPlanId == travelPlanId)
                .ToListAsync();

            return Ok(tokens.Select(MapToResponse));
        }

        [HttpPost]
        public async Task<ActionResult<ShareTokenResponseDto>> Create(Guid travelPlanId, CreateShareTokenDto dto)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            if (dto.ExpiresAt <= DateTime.UtcNow)
                return BadRequest("Expiry date must be in the future.");

            var shareToken = new ShareToken
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Token = Guid.NewGuid().ToString("N"),
                AccessType = dto.AccessType,
                ExpiresAt = dto.ExpiresAt
            };

            _context.ShareTokens.Add(shareToken);
            await _context.SaveChangesAsync();

            return Ok(MapToResponse(shareToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid travelPlanId, Guid id)
        {
            var token = await _context.ShareTokens
                .FirstOrDefaultAsync(t => t.Id == id && t.TravelPlanId == travelPlanId);

            if (token is null) return NotFound();

            _context.ShareTokens.Remove(token);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Javni endpoint - bez [Authorize], pristup putem tokena
        [AllowAnonymous]
        [HttpGet("access/{token}")]
        public async Task<ActionResult<ShareTokenResponseDto>> GetByToken(Guid travelPlanId, string token)
        {
            var shareToken = await _context.ShareTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.TravelPlanId == travelPlanId);

            if (shareToken is null) return NotFound();

            if (shareToken.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("This share link has expired.");

            return Ok(MapToResponse(shareToken));
        }

        private static ShareTokenResponseDto MapToResponse(ShareToken t) => new()
        {
            Id = t.Id,
            TravelPlanId = t.TravelPlanId,
            Token = t.Token,
            AccessType = t.AccessType,
            ExpiresAt = t.ExpiresAt
        };
    }
}
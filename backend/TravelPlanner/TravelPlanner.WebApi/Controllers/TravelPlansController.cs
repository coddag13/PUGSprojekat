using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.TravelPlans;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans")]
    public class TravelPlansController : ControllerBase
    {
        private readonly TravelPlannerDbContext _context;

        public TravelPlansController(TravelPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TravelPlanResponseDto>>> GetAll()
        {
            var plans = await _context.TravelPlans.ToListAsync();
            var result = plans.Select(MapToResponse);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TravelPlanResponseDto>> GetById(Guid id)
        {
            var plan = await _context.TravelPlans.FindAsync(id);
            if (plan is null) return NotFound();
            return Ok(MapToResponse(plan));
        }

        [HttpPost]
        public async Task<ActionResult<TravelPlanResponseDto>> Create(CreateTravelPlanDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest("End date cannot be before start date.");
            if (dto.PlannedBudget < 0)
                return BadRequest("Budget cannot be negative.");

            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                               ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            var ownerId = Guid.Parse(ownerIdClaim!.Value);

            var plan = new TravelPlan
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                PlannedBudget = dto.PlannedBudget,
                Notes = dto.Notes,
                OwnerId = ownerId
            };

            _context.TravelPlans.Add(plan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = plan.Id }, MapToResponse(plan));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTravelPlanDto dto)
        {
            var plan = await _context.TravelPlans.FindAsync(id);
            if (plan is null) return NotFound();

            if (dto.EndDate < dto.StartDate)
                return BadRequest("End date cannot be before start date.");
            if (dto.PlannedBudget < 0)
                return BadRequest("Budget cannot be negative.");

            plan.Title = dto.Title;
            plan.Description = dto.Description;
            plan.StartDate = dto.StartDate;
            plan.EndDate = dto.EndDate;
            plan.PlannedBudget = dto.PlannedBudget;
            plan.Notes = dto.Notes;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var plan = await _context.TravelPlans.FindAsync(id);
            if (plan is null) return NotFound();

            _context.TravelPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static TravelPlanResponseDto MapToResponse(TravelPlan plan) => new()
        {
            Id = plan.Id,
            Title = plan.Title,
            Description = plan.Description,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            PlannedBudget = plan.PlannedBudget,
            Notes = plan.Notes,
            OwnerId = plan.OwnerId
        };
    }
}
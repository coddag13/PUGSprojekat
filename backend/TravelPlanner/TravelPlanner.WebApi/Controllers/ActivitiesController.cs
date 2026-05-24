using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.Activities;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/activities")]
    public class ActivitiesController : ControllerBase
    {
        private readonly TravelPlannerDbContext _context;

        public ActivitiesController(TravelPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityResponseDto>>> GetAll(Guid travelPlanId)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            var activities = await _context.PlanActivities
                .Where(a => a.TravelPlanId == travelPlanId)
                .ToListAsync();

            return Ok(activities.Select(MapToResponse));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ActivityResponseDto>> GetById(Guid travelPlanId, Guid id)
        {
            var activity = await _context.PlanActivities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            if (activity is null) return NotFound();
            return Ok(MapToResponse(activity));
        }

        [HttpPost]
        public async Task<ActionResult<ActivityResponseDto>> Create(Guid travelPlanId, CreateActivityDto dto)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            if (dto.EstimatedCost < 0)
                return BadRequest("Estimated cost cannot be negative.");

            if (dto.DestinationId.HasValue)
            {
                var destExists = await _context.Destinations
                    .AnyAsync(d => d.Id == dto.DestinationId.Value && d.TravelPlanId == travelPlanId);
                if (!destExists) return BadRequest("Destination not found in this travel plan.");
            }

            var activity = new PlanActivity
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                DestinationId = dto.DestinationId,
                Name = dto.Name,
                Date = dto.Date,
                Time = dto.Time,
                Location = dto.Location,
                Description = dto.Description,
                EstimatedCost = dto.EstimatedCost,
                Status = dto.Status
            };

            _context.PlanActivities.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { travelPlanId, id = activity.Id }, MapToResponse(activity));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateActivityDto dto)
        {
            var activity = await _context.PlanActivities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            if (activity is null) return NotFound();

            if (dto.EstimatedCost < 0)
                return BadRequest("Estimated cost cannot be negative.");

            if (dto.DestinationId.HasValue)
            {
                var destExists = await _context.Destinations
                    .AnyAsync(d => d.Id == dto.DestinationId.Value && d.TravelPlanId == travelPlanId);
                if (!destExists) return BadRequest("Destination not found in this travel plan.");
            }

            activity.DestinationId = dto.DestinationId;
            activity.Name = dto.Name;
            activity.Date = dto.Date;
            activity.Time = dto.Time;
            activity.Location = dto.Location;
            activity.Description = dto.Description;
            activity.EstimatedCost = dto.EstimatedCost;
            activity.Status = dto.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid travelPlanId, Guid id)
        {
            var activity = await _context.PlanActivities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            if (activity is null) return NotFound();

            _context.PlanActivities.Remove(activity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static ActivityResponseDto MapToResponse(PlanActivity a) => new()
        {
            Id = a.Id,
            TravelPlanId = a.TravelPlanId,
            DestinationId = a.DestinationId,
            Name = a.Name,
            Date = a.Date,
            Time = a.Time,
            Location = a.Location,
            Description = a.Description,
            EstimatedCost = a.EstimatedCost,
            Status = a.Status
        };
    }
}
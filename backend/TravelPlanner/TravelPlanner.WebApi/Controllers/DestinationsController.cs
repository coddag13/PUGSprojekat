using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.Destinations;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/destinations")]
    public class DestinationsController : ControllerBase
    {
        private readonly TravelPlannerDbContext _context;

        public DestinationsController(TravelPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DestinationResponseDto>>> GetAll(Guid travelPlanId)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            var destinations = await _context.Destinations
                .Where(d => d.TravelPlanId == travelPlanId)
                .ToListAsync();

            return Ok(destinations.Select(MapToResponse));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DestinationResponseDto>> GetById(Guid travelPlanId, Guid id)
        {
            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            if (destination is null) return NotFound();
            return Ok(MapToResponse(destination));
        }

        [HttpPost]
        public async Task<ActionResult<DestinationResponseDto>> Create(Guid travelPlanId, CreateDestinationDto dto)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            if (dto.DepartureDate < dto.ArrivalDate)
                return BadRequest("Departure date cannot be before arrival date.");

            var destination = new Destination
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Name = dto.Name,
                Location = dto.Location,
                ArrivalDate = dto.ArrivalDate,
                DepartureDate = dto.DepartureDate,
                Description = dto.Description
            };

            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { travelPlanId, id = destination.Id }, MapToResponse(destination));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateDestinationDto dto)
        {
            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            if (destination is null) return NotFound();

            if (dto.DepartureDate < dto.ArrivalDate)
                return BadRequest("Departure date cannot be before arrival date.");

            destination.Name = dto.Name;
            destination.Location = dto.Location;
            destination.ArrivalDate = dto.ArrivalDate;
            destination.DepartureDate = dto.DepartureDate;
            destination.Description = dto.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid travelPlanId, Guid id)
        {
            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            if (destination is null) return NotFound();

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static DestinationResponseDto MapToResponse(Destination d) => new()
        {
            Id = d.Id,
            TravelPlanId = d.TravelPlanId,
            Name = d.Name,
            Location = d.Location,
            ArrivalDate = d.ArrivalDate,
            DepartureDate = d.DepartureDate,
            Description = d.Description
        };
    }
}
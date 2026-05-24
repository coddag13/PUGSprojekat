using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.TravelPlans;

namespace TravelPlanner.WebApi.Controllers
{
    [ApiController]
    [Route("api/travel-plans")]
    public class TravelPlansController : ControllerBase
    {
        private readonly TravelPlannerDbContext _context;

        public TravelPlansController(TravelPlannerDbContext context)
        {
            _context = context;
        }

        // GET api/travel-plans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TravelPlanResponseDto>>> GetAll()
        {
            var plans = await _context.TravelPlans.ToListAsync();
            var result = plans.Select(MapToResponse);
            return Ok(result);
        }

        // GET api/travel-plans/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TravelPlanResponseDto>> GetById(Guid id)
        {
            var plan = await _context.TravelPlans.FindAsync(id);
            if (plan is null) return NotFound();
            return Ok(MapToResponse(plan));
        }

        // POST api/travel-plans
        [HttpPost]
        public async Task<ActionResult<TravelPlanResponseDto>> Create(CreateTravelPlanDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest("End date cannot be before start date.");
            if (dto.PlannedBudget < 0)
                return BadRequest("Budget cannot be negative.");

            var plan = new TravelPlan
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                PlannedBudget = dto.PlannedBudget,
                Notes = dto.Notes,
                OwnerId = Guid.Empty // privremeno, zamijeniće se kad dodamo auth
            };

            _context.TravelPlans.Add(plan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = plan.Id }, MapToResponse(plan));
        }

        // PUT api/travel-plans/{id}
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

        // DELETE api/travel-plans/{id}
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
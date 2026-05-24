using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.ChecklistItems;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/checklist-items")]
    public class ChecklistItemsController : ControllerBase
    {
        private readonly TravelPlannerDbContext _context;

        public ChecklistItemsController(TravelPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChecklistItemResponseDto>>> GetAll(Guid travelPlanId)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            var items = await _context.ChecklistItems
                .Where(c => c.TravelPlanId == travelPlanId)
                .ToListAsync();

            return Ok(items.Select(MapToResponse));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ChecklistItemResponseDto>> GetById(Guid travelPlanId, Guid id)
        {
            var item = await _context.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            if (item is null) return NotFound();
            return Ok(MapToResponse(item));
        }

        [HttpPost]
        public async Task<ActionResult<ChecklistItemResponseDto>> Create(Guid travelPlanId, CreateChecklistItemDto dto)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Text = dto.Text,
                IsCompleted = false
            };

            _context.ChecklistItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { travelPlanId, id = item.Id }, MapToResponse(item));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateChecklistItemDto dto)
        {
            var item = await _context.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            if (item is null) return NotFound();

            item.Text = dto.Text;
            item.IsCompleted = dto.IsCompleted;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid travelPlanId, Guid id)
        {
            var item = await _context.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            if (item is null) return NotFound();

            _context.ChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static ChecklistItemResponseDto MapToResponse(ChecklistItem c) => new()
        {
            Id = c.Id,
            TravelPlanId = c.TravelPlanId,
            Text = c.Text,
            IsCompleted = c.IsCompleted
        };
    }
}
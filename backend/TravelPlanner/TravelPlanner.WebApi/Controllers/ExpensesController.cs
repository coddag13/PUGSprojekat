using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.Expenses;
using Microsoft.AspNetCore.Authorization;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans/{travelPlanId:guid}/expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly TravelPlannerDbContext _context;

        public ExpensesController(TravelPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseResponseDto>>> GetAll(Guid travelPlanId)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            var expenses = await _context.Expenses
                .Where(e => e.TravelPlanId == travelPlanId)
                .ToListAsync();

            return Ok(expenses.Select(MapToResponse));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ExpenseResponseDto>> GetById(Guid travelPlanId, Guid id)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            if (expense is null) return NotFound();
            return Ok(MapToResponse(expense));
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseResponseDto>> Create(Guid travelPlanId, CreateExpenseDto dto)
        {
            var planExists = await _context.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists) return NotFound("Travel plan not found.");

            if (dto.Amount < 0)
                return BadRequest("Amount cannot be negative.");

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Name = dto.Name,
                Category = dto.Category,
                Amount = dto.Amount,
                Date = dto.Date,
                Description = dto.Description
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { travelPlanId, id = expense.Id }, MapToResponse(expense));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid travelPlanId, Guid id, UpdateExpenseDto dto)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            if (expense is null) return NotFound();

            if (dto.Amount < 0)
                return BadRequest("Amount cannot be negative.");

            expense.Name = dto.Name;
            expense.Category = dto.Category;
            expense.Amount = dto.Amount;
            expense.Date = dto.Date;
            expense.Description = dto.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid travelPlanId, Guid id)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            if (expense is null) return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static ExpenseResponseDto MapToResponse(Expense e) => new()
        {
            Id = e.Id,
            TravelPlanId = e.TravelPlanId,
            Name = e.Name,
            Category = e.Category,
            Amount = e.Amount,
            Date = e.Date,
            Description = e.Description
        };
    }
}
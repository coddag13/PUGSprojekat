using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelPlanner.Common.Enums;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.Admin;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly TravelPlannerDbContext _db;

        public AdminController(TravelPlannerDbContext db)
        {
            _db = db;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AdminUserResponseDto>>> GetUsers()
        {
            var users = await _db.Users
                .AsNoTracking()
                .Include(u => u.TravelPlans)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new AdminUserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    TravelPlansCount = u.TravelPlans.Count
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("users/{id:guid}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid id, UpdateUserRoleDto dto)
        {
            if (!Enum.TryParse<UserRole>(dto.Role, true, out var parsedRole))
                return BadRequest("Invalid role.");

            var currentUserId = GetCurrentUserId();
            if (currentUserId == id && parsedRole != UserRole.Admin)
                return BadRequest("Admin cannot remove their own admin role.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
                return NotFound();

            if (user.Role == UserRole.Admin && parsedRole != UserRole.Admin)
            {
                var adminCount = await _db.Users.CountAsync(u => u.Role == UserRole.Admin);
                if (adminCount <= 1)
                    return BadRequest("At least one admin account must remain in the system.");
            }

            user.Role = parsedRole;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("users/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == id)
                return BadRequest("Admin cannot delete their own account.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
                return NotFound();

            if (user.Role == UserRole.Admin)
            {
                var adminCount = await _db.Users.CountAsync(u => u.Role == UserRole.Admin);
                if (adminCount <= 1)
                    return BadRequest("At least one admin account must remain in the system.");
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("travel-plans")]
        public async Task<ActionResult<IEnumerable<AdminTravelPlanResponseDto>>> GetTravelPlans()
        {
            var plans = await _db.TravelPlans
                .AsNoTracking()
                .Include(tp => tp.Owner)
                .OrderBy(tp => tp.StartDate)
                .ThenBy(tp => tp.Title)
                .Select(tp => new AdminTravelPlanResponseDto
                {
                    Id = tp.Id,
                    Title = tp.Title,
                    Description = tp.Description,
                    StartDate = tp.StartDate,
                    EndDate = tp.EndDate,
                    PlannedBudget = tp.PlannedBudget,
                    OwnerEmail = tp.Owner.Email,
                    OwnerName = $"{tp.Owner.FirstName} {tp.Owner.LastName}"
                })
                .ToListAsync();

            return Ok(plans);
        }

        [HttpDelete("travel-plans/{id:guid}")]
        public async Task<IActionResult> DeleteTravelPlan(Guid id)
        {
            var plan = await _db.TravelPlans.FirstOrDefaultAsync(tp => tp.Id == id);
            if (plan is null)
                return NotFound();

            _db.TravelPlans.Remove(plan);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            return Guid.Parse(claim!.Value);
        }
    }
}

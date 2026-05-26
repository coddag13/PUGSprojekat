using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common.Enums;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.WebApi.DTOs.Admin;
using TravelPlanner.WebApi.Extensions;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AuthDbContext _authDb;
        private readonly PlanDbContext _planDb;
        private readonly SharingDbContext _sharingDb;

        public AdminController(AuthDbContext authDb, PlanDbContext planDb, SharingDbContext sharingDb)
        {
            _authDb = authDb;
            _planDb = planDb;
            _sharingDb = sharingDb;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AdminUserResponseDto>>> GetUsers()
        {
            var planCounts = await _planDb.TravelPlans
                .AsNoTracking()
                .GroupBy(tp => tp.OwnerId)
                .Select(group => new { OwnerId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(item => item.OwnerId, item => item.Count);

            var users = await _authDb.Users
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new AdminUserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    TravelPlansCount = 0
                })
                .ToListAsync();

            foreach (var user in users)
            {
                user.TravelPlansCount = planCounts.TryGetValue(user.Id, out var count) ? count : 0;
            }

            return Ok(users);
        }

        [HttpPut("users/{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserRoleDto dto)
        {
            if (!Enum.TryParse<UserRole>(dto.Role, true, out var parsedRole))
                return BadRequest("Uloga nije ispravna.");

            var currentUserId = User.GetUserId();
            if (currentUserId == id && parsedRole != UserRole.Admin)
                return BadRequest("Administrator ne može ukloniti svoju administratorsku ulogu.");

            var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
                return NotFound();

            if (user.Role == UserRole.Admin && parsedRole != UserRole.Admin)
            {
                var adminCount = await _authDb.Users.CountAsync(u => u.Role == UserRole.Admin);
                if (adminCount <= 1)
                    return BadRequest("U sistemu mora ostati najmanje jedan administratorski nalog.");
            }

            user.Role = parsedRole;
            await _authDb.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("users/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId == id)
                return BadRequest("Administrator ne može obrisati svoj nalog.");

            var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
                return NotFound();

            if (user.Role == UserRole.Admin)
            {
                var adminCount = await _authDb.Users.CountAsync(u => u.Role == UserRole.Admin);
                if (adminCount <= 1)
                    return BadRequest("U sistemu mora ostati najmanje jedan administratorski nalog.");
            }

            var planIds = await _planDb.TravelPlans
                .Where(tp => tp.OwnerId == id)
                .Select(tp => tp.Id)
                .ToListAsync();

            if (planIds.Count > 0)
            {
                var shareTokens = await _sharingDb.ShareTokens
                    .Where(st => planIds.Contains(st.TravelPlanId))
                    .ToListAsync();

                _sharingDb.ShareTokens.RemoveRange(shareTokens);
                await _sharingDb.SaveChangesAsync();

                var plans = await _planDb.TravelPlans.Where(tp => tp.OwnerId == id).ToListAsync();
                _planDb.TravelPlans.RemoveRange(plans);
                await _planDb.SaveChangesAsync();
            }

            _authDb.Users.Remove(user);
            await _authDb.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("travel-plans")]
        public async Task<ActionResult<IEnumerable<AdminTravelPlanResponseDto>>> GetTravelPlans()
        {
            var users = await _authDb.Users
                .AsNoTracking()
                .ToDictionaryAsync(u => u.Id);

            var plans = await _planDb.TravelPlans
                .AsNoTracking()
                .OrderBy(tp => tp.StartDate)
                .ThenBy(tp => tp.Title)
                .ToListAsync();

            return Ok(plans.Select(tp =>
            {
                users.TryGetValue(tp.OwnerId, out var owner);

                return new AdminTravelPlanResponseDto
                {
                    Id = tp.Id,
                    Title = tp.Title,
                    Description = tp.Description,
                    StartDate = tp.StartDate,
                    EndDate = tp.EndDate,
                    PlannedBudget = tp.PlannedBudget,
                    OwnerEmail = owner?.Email ?? string.Empty,
                    OwnerName = owner is null ? "Nepoznat korisnik" : $"{owner.FirstName} {owner.LastName}"
                };
            }));
        }

        [HttpDelete("travel-plans/{id:guid}")]
        public async Task<IActionResult> DeleteTravelPlan(Guid id)
        {
            var plan = await _planDb.TravelPlans.FirstOrDefaultAsync(tp => tp.Id == id);
            if (plan is null)
                return NotFound();

            var shareTokens = await _sharingDb.ShareTokens.Where(st => st.TravelPlanId == id).ToListAsync();
            _sharingDb.ShareTokens.RemoveRange(shareTokens);
            await _sharingDb.SaveChangesAsync();

            _planDb.TravelPlans.Remove(plan);
            await _planDb.SaveChangesAsync();

            return NoContent();
        }
    }
}

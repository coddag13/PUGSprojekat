using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.PlanService.Mappings;
using TravelPlanner.PlanService.Services.Common;
using TravelPlanner.PlanService.Validation;

namespace TravelPlanner.PlanService.Services.Reminders
{
    internal sealed class ReminderCrudService : IReminderCrudService
    {
        private readonly IPlanServiceDbContextFactory _dbFactory;
        private readonly IReminderValidationService _validationService;

        public ReminderCrudService(
            IPlanServiceDbContextFactory dbFactory,
            IReminderValidationService validationService)
        {
            _dbFactory = dbFactory;
            _validationService = validationService;
        }

        public async Task<List<ReminderData>> GetRemindersAsync(Guid travelPlanId)
        {
            await using var db = _dbFactory.CreateDbContext();

            var reminders = await db.Reminders
                .AsNoTracking()
                .Where(r => r.TravelPlanId == travelPlanId)
                .OrderBy(r => r.RemindAt)
                .ToListAsync();

            return reminders.Select(ReminderMapper.Map).ToList();
        }

        public async Task<ReminderData?> GetReminderByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var reminder = await db.Reminders
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.TravelPlanId == travelPlanId);

            return reminder is null ? null : ReminderMapper.Map(reminder);
        }

        public async Task<ServiceResponse<ReminderData>> CreateReminderAsync(Guid travelPlanId, string title, DateTime remindAt)
        {
            await using var db = _dbFactory.CreateDbContext();

            title = title?.Trim() ?? string.Empty;

            var titleError = _validationService.ValidateTitle(title);
            if (titleError is not null)
                return ServiceResponse<ReminderData>.Fail(titleError);

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, remindAt);
            if (planError is not null)
                return ServiceResponse<ReminderData>.Fail(planError);

            var reminder = new Reminder
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Title = title,
                RemindAt = remindAt,
                IsCompleted = false
            };

            db.Reminders.Add(reminder);
            await db.SaveChangesAsync();

            return ServiceResponse<ReminderData>.Ok(ReminderMapper.Map(reminder));
        }

        public async Task<bool> UpdateReminderAsync(Guid travelPlanId, Guid id, string title, DateTime remindAt, bool isCompleted)
        {
            await using var db = _dbFactory.CreateDbContext();

            var reminder = await db.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.TravelPlanId == travelPlanId);

            if (reminder is null)
                return false;

            title = title?.Trim() ?? string.Empty;

            if (_validationService.ValidateTitle(title) is not null)
                return false;

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, remindAt);
            if (planError is not null)
                return false;

            reminder.Title = title;
            reminder.RemindAt = remindAt;
            reminder.IsCompleted = isCompleted;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReminderAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var reminder = await db.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.TravelPlanId == travelPlanId);

            if (reminder is null)
                return false;

            db.Reminders.Remove(reminder);
            await db.SaveChangesAsync();
            return true;
        }
    }
}

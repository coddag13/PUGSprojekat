using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.PlanService.Mappings;
using TravelPlanner.PlanService.Services.Common;
using TravelPlanner.PlanService.Validation;

namespace TravelPlanner.PlanService.Services.Checklist
{
    internal sealed class ChecklistCrudService : IChecklistCrudService
    {
        private readonly IPlanServiceDbContextFactory _dbFactory;
        private readonly IChecklistValidationService _validationService;

        public ChecklistCrudService(
            IPlanServiceDbContextFactory dbFactory,
            IChecklistValidationService validationService)
        {
            _dbFactory = dbFactory;
            _validationService = validationService;
        }

        public async Task<List<ChecklistItemData>> GetChecklistItemsAsync(Guid travelPlanId)
        {
            await using var db = _dbFactory.CreateDbContext();

            var items = await db.ChecklistItems
                .AsNoTracking()
                .Where(c => c.TravelPlanId == travelPlanId)
                .ToListAsync();

            return items.Select(ChecklistItemMapper.Map).ToList();
        }

        public async Task<ChecklistItemData?> GetChecklistItemByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var item = await db.ChecklistItems
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            return item is null ? null : ChecklistItemMapper.Map(item);
        }

        public async Task<ServiceResponse<ChecklistItemData>> CreateChecklistItemAsync(Guid travelPlanId, string text)
        {
            await using var db = _dbFactory.CreateDbContext();

            text = text?.Trim() ?? string.Empty;

            var fieldError = _validationService.ValidateText(text);
            if (fieldError is not null)
                return ServiceResponse<ChecklistItemData>.Fail(fieldError);

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, text);
            if (planError is not null)
                return ServiceResponse<ChecklistItemData>.Fail(planError);

            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Text = text,
                IsCompleted = false
            };

            db.ChecklistItems.Add(item);
            await db.SaveChangesAsync();

            return ServiceResponse<ChecklistItemData>.Ok(ChecklistItemMapper.Map(item));
        }

        public async Task<bool> UpdateChecklistItemAsync(Guid travelPlanId, Guid id, string text, bool isCompleted)
        {
            await using var db = _dbFactory.CreateDbContext();

            var item = await db.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            if (item is null)
                return false;

            text = text?.Trim() ?? string.Empty;

            if (_validationService.ValidateText(text) is not null)
                return false;

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, text, id);
            if (planError is not null)
                return false;

            item.Text = text;
            item.IsCompleted = isCompleted;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteChecklistItemAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var item = await db.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            if (item is null)
                return false;

            db.ChecklistItems.Remove(item);
            await db.SaveChangesAsync();
            return true;
        }
    }
}

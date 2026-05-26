using Microsoft.EntityFrameworkCore;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService.Validation
{
    internal sealed class ChecklistValidationService : IChecklistValidationService
    {
        public string? ValidateText(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? "Checklist item text is required." : null;
        }

        public async Task<string?> ValidateForPlanAsync(PlanDbContext db, Guid travelPlanId, string text, Guid? excludeChecklistItemId = null)
        {
            var planExists = await db.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists)
                return "Travel plan not found.";

            var exists = await db.ChecklistItems.AnyAsync(c =>
                c.TravelPlanId == travelPlanId &&
                (!excludeChecklistItemId.HasValue || c.Id != excludeChecklistItemId.Value) &&
                c.Text.ToLower() == text.ToLower());

            return exists ? "Checklist item already exists." : null;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.PlanService
{
    internal sealed class PlanService : StatefulService, IPlanService
    {
        private readonly string _connectionString;

        public PlanService(StatefulServiceContext context) : base(context)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<List<TravelPlanData>> GetAllPlansByOwnerAsync(Guid ownerId)
        {
            await using var db = CreateDbContext();

            var plans = await db.TravelPlans
                .AsNoTracking()
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();

            return plans.Select(MapToPlanData).ToList();
        }

        public async Task<TravelPlanData?> GetPlanByIdAsync(Guid id)
        {
            await using var db = CreateDbContext();

            var plan = await db.TravelPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return plan is null ? null : MapToPlanData(plan);
        }

        public async Task<ServiceResponse<TravelPlanData>> CreatePlanAsync(
            Guid ownerId,
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            decimal plannedBudget,
            string notes)
        {
            await using var db = CreateDbContext();

            title = title?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            notes = notes?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(title))
                return ServiceResponse<TravelPlanData>.Fail("Title is required.");

            if (endDate < startDate)
                return ServiceResponse<TravelPlanData>.Fail("End date cannot be before start date.");

            if (plannedBudget < 0)
                return ServiceResponse<TravelPlanData>.Fail("Planned budget cannot be negative.");

            var ownerExists = await db.Users.AnyAsync(u => u.Id == ownerId);
            if (!ownerExists)
                return ServiceResponse<TravelPlanData>.Fail("Owner not found.");

            var plan = new TravelPlan
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = title,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                PlannedBudget = plannedBudget,
                Notes = notes
            };

            db.TravelPlans.Add(plan);
            await db.SaveChangesAsync();

            return ServiceResponse<TravelPlanData>.Ok(MapToPlanData(plan));
        }

        public async Task<bool> UpdatePlanAsync(
            Guid id,
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            decimal plannedBudget,
            string notes)
        {
            await using var db = CreateDbContext();

            var plan = await db.TravelPlans.FindAsync(id);
            if (plan is null)
                return false;

            title = title?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            notes = notes?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(title))
                return false;

            if (endDate < startDate || plannedBudget < 0)
                return false;

            plan.Title = title;
            plan.Description = description;
            plan.StartDate = startDate;
            plan.EndDate = endDate;
            plan.PlannedBudget = plannedBudget;
            plan.Notes = notes;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePlanAsync(Guid id)
        {
            await using var db = CreateDbContext();

            var plan = await db.TravelPlans.FindAsync(id);
            if (plan is null)
                return false;

            db.TravelPlans.Remove(plan);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<DestinationData>> GetDestinationsAsync(Guid travelPlanId)
        {
            await using var db = CreateDbContext();

            var destinations = await db.Destinations
                .AsNoTracking()
                .Where(d => d.TravelPlanId == travelPlanId)
                .ToListAsync();

            return destinations.Select(MapToDestinationData).ToList();
        }

        public async Task<DestinationData?> GetDestinationByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var destination = await db.Destinations
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            return destination is null ? null : MapToDestinationData(destination);
        }

        public async Task<ServiceResponse<DestinationData>> CreateDestinationAsync(
            Guid travelPlanId,
            string name,
            string location,
            DateTime arrivalDate,
            DateTime departureDate,
            string description)
        {
            await using var db = CreateDbContext();

            name = name?.Trim() ?? string.Empty;
            location = location?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse<DestinationData>.Fail("Destination name is required.");

            if (string.IsNullOrWhiteSpace(location))
                return ServiceResponse<DestinationData>.Fail("Destination location is required.");

            if (departureDate < arrivalDate)
                return ServiceResponse<DestinationData>.Fail("Departure date cannot be before arrival date.");

            var planExists = await db.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists)
                return ServiceResponse<DestinationData>.Fail("Travel plan not found.");

            var destination = new Destination
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Name = name,
                Location = location,
                ArrivalDate = arrivalDate,
                DepartureDate = departureDate,
                Description = description
            };

            db.Destinations.Add(destination);
            await db.SaveChangesAsync();

            return ServiceResponse<DestinationData>.Ok(MapToDestinationData(destination));
        }

        public async Task<bool> UpdateDestinationAsync(
            Guid travelPlanId,
            Guid id,
            string name,
            string location,
            DateTime arrivalDate,
            DateTime departureDate,
            string description)
        {
            await using var db = CreateDbContext();

            var destination = await db.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            if (destination is null)
                return false;

            name = name?.Trim() ?? string.Empty;
            location = location?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(location))
                return false;

            if (departureDate < arrivalDate)
                return false;

            destination.Name = name;
            destination.Location = location;
            destination.ArrivalDate = arrivalDate;
            destination.DepartureDate = departureDate;
            destination.Description = description;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDestinationAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var destination = await db.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            if (destination is null)
                return false;

            db.Destinations.Remove(destination);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ActivityData>> GetActivitiesAsync(Guid travelPlanId)
        {
            await using var db = CreateDbContext();

            var activities = await db.PlanActivities
                .AsNoTracking()
                .Where(a => a.TravelPlanId == travelPlanId)
                .ToListAsync();

            return activities.Select(MapToActivityData).ToList();
        }

        public async Task<ActivityData?> GetActivityByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var activity = await db.PlanActivities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            return activity is null ? null : MapToActivityData(activity);
        }

        public async Task<ServiceResponse<ActivityData>> CreateActivityAsync(
            Guid travelPlanId,
            Guid? destinationId,
            string name,
            DateTime date,
            TimeSpan time,
            string location,
            string description,
            decimal estimatedCost,
            ActivityStatus status)
        {
            await using var db = CreateDbContext();

            name = name?.Trim() ?? string.Empty;
            location = location?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse<ActivityData>.Fail("Activity name is required.");

            if (estimatedCost < 0)
                return ServiceResponse<ActivityData>.Fail("Estimated cost cannot be negative.");

            var planExists = await db.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists)
                return ServiceResponse<ActivityData>.Fail("Travel plan not found.");

            if (destinationId.HasValue)
            {
                var destinationExists = await db.Destinations
                    .AnyAsync(d => d.Id == destinationId.Value && d.TravelPlanId == travelPlanId);

                if (!destinationExists)
                    return ServiceResponse<ActivityData>.Fail("Destination not found in this travel plan.");
            }

            var activity = new PlanActivity
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                DestinationId = destinationId,
                Name = name,
                Date = date,
                Time = time,
                Location = location,
                Description = description,
                EstimatedCost = estimatedCost,
                Status = status
            };

            db.PlanActivities.Add(activity);
            await db.SaveChangesAsync();

            return ServiceResponse<ActivityData>.Ok(MapToActivityData(activity));
        }

        public async Task<bool> UpdateActivityAsync(
            Guid travelPlanId,
            Guid id,
            Guid? destinationId,
            string name,
            DateTime date,
            TimeSpan time,
            string location,
            string description,
            decimal estimatedCost,
            ActivityStatus status)
        {
            await using var db = CreateDbContext();

            var activity = await db.PlanActivities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            if (activity is null)
                return false;

            name = name?.Trim() ?? string.Empty;
            location = location?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || estimatedCost < 0)
                return false;

            if (destinationId.HasValue)
            {
                var destinationExists = await db.Destinations
                    .AnyAsync(d => d.Id == destinationId.Value && d.TravelPlanId == travelPlanId);

                if (!destinationExists)
                    return false;
            }

            activity.DestinationId = destinationId;
            activity.Name = name;
            activity.Date = date;
            activity.Time = time;
            activity.Location = location;
            activity.Description = description;
            activity.EstimatedCost = estimatedCost;
            activity.Status = status;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteActivityAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var activity = await db.PlanActivities
                .FirstOrDefaultAsync(a => a.Id == id && a.TravelPlanId == travelPlanId);

            if (activity is null)
                return false;

            db.PlanActivities.Remove(activity);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ExpenseData>> GetExpensesAsync(Guid travelPlanId)
        {
            await using var db = CreateDbContext();

            var expenses = await db.Expenses
                .AsNoTracking()
                .Where(e => e.TravelPlanId == travelPlanId)
                .ToListAsync();

            return expenses.Select(MapToExpenseData).ToList();
        }

        public async Task<ExpenseData?> GetExpenseByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var expense = await db.Expenses
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            return expense is null ? null : MapToExpenseData(expense);
        }

        public async Task<ServiceResponse<ExpenseData>> CreateExpenseAsync(
            Guid travelPlanId,
            string name,
            ExpenseCategory category,
            decimal amount,
            DateTime date,
            string description)
        {
            await using var db = CreateDbContext();

            name = name?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse<ExpenseData>.Fail("Expense name is required.");

            if (amount < 0)
                return ServiceResponse<ExpenseData>.Fail("Expense amount cannot be negative.");

            var planExists = await db.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists)
                return ServiceResponse<ExpenseData>.Fail("Travel plan not found.");

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Name = name,
                Category = category,
                Amount = amount,
                Date = date,
                Description = description
            };

            db.Expenses.Add(expense);
            await db.SaveChangesAsync();

            return ServiceResponse<ExpenseData>.Ok(MapToExpenseData(expense));
        }

        public async Task<bool> UpdateExpenseAsync(
            Guid travelPlanId,
            Guid id,
            string name,
            ExpenseCategory category,
            decimal amount,
            DateTime date,
            string description)
        {
            await using var db = CreateDbContext();

            var expense = await db.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            if (expense is null)
                return false;

            name = name?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || amount < 0)
                return false;

            expense.Name = name;
            expense.Category = category;
            expense.Amount = amount;
            expense.Date = date;
            expense.Description = description;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExpenseAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var expense = await db.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.TravelPlanId == travelPlanId);

            if (expense is null)
                return false;

            db.Expenses.Remove(expense);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChecklistItemData>> GetChecklistItemsAsync(Guid travelPlanId)
        {
            await using var db = CreateDbContext();

            var items = await db.ChecklistItems
                .AsNoTracking()
                .Where(c => c.TravelPlanId == travelPlanId)
                .ToListAsync();

            return items.Select(MapToChecklistItemData).ToList();
        }

        public async Task<ChecklistItemData?> GetChecklistItemByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var item = await db.ChecklistItems
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            return item is null ? null : MapToChecklistItemData(item);
        }

        public async Task<ServiceResponse<ChecklistItemData>> CreateChecklistItemAsync(Guid travelPlanId, string text)
        {
            await using var db = CreateDbContext();

            text = text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(text))
                return ServiceResponse<ChecklistItemData>.Fail("Checklist item text is required.");

            var planExists = await db.TravelPlans.AnyAsync(p => p.Id == travelPlanId);
            if (!planExists)
                return ServiceResponse<ChecklistItemData>.Fail("Travel plan not found.");

            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Text = text,
                IsCompleted = false
            };

            db.ChecklistItems.Add(item);
            await db.SaveChangesAsync();

            return ServiceResponse<ChecklistItemData>.Ok(MapToChecklistItemData(item));
        }

        public async Task<bool> UpdateChecklistItemAsync(Guid travelPlanId, Guid id, string text, bool isCompleted)
        {
            await using var db = CreateDbContext();

            var item = await db.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            if (item is null)
                return false;

            text = text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            item.Text = text;
            item.IsCompleted = isCompleted;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteChecklistItemAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var item = await db.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.TravelPlanId == travelPlanId);

            if (item is null)
                return false;

            db.ChecklistItems.Remove(item);
            await db.SaveChangesAsync();
            return true;
        }

        private TravelPlannerDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<TravelPlannerDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new TravelPlannerDbContext(options);
        }

        private static TravelPlanData MapToPlanData(TravelPlan plan)
        {
            return new TravelPlanData
            {
                Id = plan.Id,
                OwnerId = plan.OwnerId,
                Title = plan.Title,
                Description = plan.Description,
                StartDate = plan.StartDate,
                EndDate = plan.EndDate,
                PlannedBudget = plan.PlannedBudget,
                Notes = plan.Notes
            };
        }

        private static DestinationData MapToDestinationData(Destination destination)
        {
            return new DestinationData
            {
                Id = destination.Id,
                TravelPlanId = destination.TravelPlanId,
                Name = destination.Name,
                Location = destination.Location,
                ArrivalDate = destination.ArrivalDate,
                DepartureDate = destination.DepartureDate,
                Description = destination.Description
            };
        }

        private static ActivityData MapToActivityData(PlanActivity activity)
        {
            return new ActivityData
            {
                Id = activity.Id,
                TravelPlanId = activity.TravelPlanId,
                DestinationId = activity.DestinationId,
                Name = activity.Name,
                Date = activity.Date,
                Time = activity.Time,
                Location = activity.Location,
                Description = activity.Description,
                EstimatedCost = activity.EstimatedCost,
                Status = activity.Status
            };
        }

        private static ExpenseData MapToExpenseData(Expense expense)
        {
            return new ExpenseData
            {
                Id = expense.Id,
                TravelPlanId = expense.TravelPlanId,
                Name = expense.Name,
                Category = expense.Category,
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description
            };
        }

        private static ChecklistItemData MapToChecklistItemData(ChecklistItem item)
        {
            return new ChecklistItemData
            {
                Id = item.Id,
                TravelPlanId = item.TravelPlanId,
                Text = item.Text,
                IsCompleted = item.IsCompleted
            };
        }
    }
}
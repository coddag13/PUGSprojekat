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

            var activitiesOutsideRange = await db.PlanActivities.AnyAsync(a =>
                a.TravelPlanId == id &&
                (a.Date.Date < startDate.Date || a.Date.Date > endDate.Date));

            if (activitiesOutsideRange)
                return false;

            var destinationsOutsideRange = await db.Destinations.AnyAsync(d =>
                d.TravelPlanId == id &&
                (d.ArrivalDate.Date < startDate.Date || d.DepartureDate.Date > endDate.Date));

            if (destinationsOutsideRange)
                return false;

            var expensesOutsideRange = await db.Expenses.AnyAsync(e =>
                e.TravelPlanId == id &&
                (e.Date.Date < startDate.Date || e.Date.Date > endDate.Date));

            if (expensesOutsideRange)
                return false;

            var committedAmount = await GetCommittedAmountAsync(db, id);
            if (committedAmount > plannedBudget)
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

            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return ServiceResponse<DestinationData>.Fail("Travel plan not found.");

            if (arrivalDate.Date < plan.StartDate.Date)
                return ServiceResponse<DestinationData>.Fail("Arrival date cannot be before the start of the travel plan.");

            if (departureDate.Date > plan.EndDate.Date)
                return ServiceResponse<DestinationData>.Fail("Departure date cannot be after the end of the travel plan.");

            var overlaps = await HasDestinationOverlapAsync(db, travelPlanId, arrivalDate, departureDate);
            if (overlaps)
                return ServiceResponse<DestinationData>.Fail("Destination dates overlap with an existing destination.");

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

            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return false;

            if (arrivalDate.Date < plan.StartDate.Date)
                return false;

            if (departureDate.Date > plan.EndDate.Date)
                return false;

            var overlaps = await HasDestinationOverlapAsync(db, travelPlanId, arrivalDate, departureDate, id);
            if (overlaps)
                return false;

            destination.Name = name;
            destination.Location = location;
            destination.ArrivalDate = arrivalDate;
            destination.DepartureDate = departureDate;
            destination.Description = description;

            await db.SaveChangesAsync();
            return true;
        }


        private static async Task<bool> HasDestinationOverlapAsync(
    PlanDbContext db,
    Guid travelPlanId,
    DateTime arrivalDate,
    DateTime departureDate,
    Guid? excludeDestinationId = null)
        {
            return await db.Destinations.AnyAsync(d =>
                d.TravelPlanId == travelPlanId &&
                (!excludeDestinationId.HasValue || d.Id != excludeDestinationId.Value) &&
                arrivalDate.Date <= d.DepartureDate.Date &&
                departureDate.Date >= d.ArrivalDate.Date);
        }

        public async Task<bool> DeleteDestinationAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var destination = await db.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            if (destination is null)
                return false;

            var relatedActivities = await db.PlanActivities
                .Where(a => a.TravelPlanId == travelPlanId && a.DestinationId == id)
                .ToListAsync();

            if (relatedActivities.Count > 0)
            {
                db.PlanActivities.RemoveRange(relatedActivities);
            }

            db.Destinations.Remove(destination);
            await db.SaveChangesAsync();
            return true;
        }

        private static async Task<bool> HasActivityScheduleConflictAsync(
    PlanDbContext db,
    Guid travelPlanId,
    DateTime date,
    TimeSpan time,
    Guid? excludeActivityId = null)
        {
            return await db.PlanActivities.AnyAsync(a =>
                a.TravelPlanId == travelPlanId &&
                (!excludeActivityId.HasValue || a.Id != excludeActivityId.Value) &&
                a.Date.Date == date.Date &&
                a.Time == time);
        }

        private static async Task<decimal> GetCommittedAmountAsync(
            PlanDbContext db,
            Guid travelPlanId,
            Guid? excludeActivityId = null,
            Guid? excludeExpenseId = null)
        {
            var activitiesQuery = db.PlanActivities.Where(a => a.TravelPlanId == travelPlanId);
            if (excludeActivityId.HasValue)
            {
                activitiesQuery = activitiesQuery.Where(a => a.Id != excludeActivityId.Value);
            }

            var expensesQuery = db.Expenses.Where(e => e.TravelPlanId == travelPlanId);
            if (excludeExpenseId.HasValue)
            {
                expensesQuery = expensesQuery.Where(e => e.Id != excludeExpenseId.Value);
            }

            var activitiesTotal = await activitiesQuery
                .Select(a => (decimal?)a.EstimatedCost)
                .SumAsync() ?? 0m;

            var expensesTotal = await expensesQuery
                .Select(e => (decimal?)e.Amount)
                .SumAsync() ?? 0m;

            return activitiesTotal + expensesTotal;
        }

        private static async Task<bool> WouldExceedBudgetWithActivityAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal newActivityCost,
            Guid? excludeActivityId = null)
        {
            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return true;

            var committedAmount = await GetCommittedAmountAsync(db, travelPlanId, excludeActivityId: excludeActivityId);
            return committedAmount + newActivityCost > plan.PlannedBudget;
        }

        private static async Task<bool> WouldExceedBudgetWithExpenseAsync(
            PlanDbContext db,
            Guid travelPlanId,
            decimal newExpenseAmount,
            Guid? excludeExpenseId = null)
        {
            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return true;

            var committedAmount = await GetCommittedAmountAsync(db, travelPlanId, excludeExpenseId: excludeExpenseId);
            return committedAmount + newExpenseAmount > plan.PlannedBudget;
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

            if (string.IsNullOrWhiteSpace(location))
                return ServiceResponse<ActivityData>.Fail("Activity location is required.");

            if (estimatedCost < 0)
                return ServiceResponse<ActivityData>.Fail("Estimated cost cannot be negative.");

            if (!Enum.IsDefined(typeof(ActivityStatus), status))
                return ServiceResponse<ActivityData>.Fail("Invalid activity status.");

            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return ServiceResponse<ActivityData>.Fail("Travel plan not found.");

            if (date.Date < plan.StartDate.Date || date.Date > plan.EndDate.Date)
                return ServiceResponse<ActivityData>.Fail("Activity date must be within the travel plan period.");

            if (destinationId.HasValue)
            {
                var destination = await db.Destinations
                    .FirstOrDefaultAsync(d => d.Id == destinationId.Value && d.TravelPlanId == travelPlanId);

                if (destination is null)
                    return ServiceResponse<ActivityData>.Fail("Destination not found in this travel plan.");

                if (date.Date < destination.ArrivalDate.Date || date.Date > destination.DepartureDate.Date)
                    return ServiceResponse<ActivityData>.Fail("Activity date must be within the selected destination period.");
            }

            var hasConflict = await HasActivityScheduleConflictAsync(db, travelPlanId, date, time);
            if (hasConflict)
                return ServiceResponse<ActivityData>.Fail("Another activity already exists at the same date and time.");

            var wouldExceedBudget = await WouldExceedBudgetWithActivityAsync(db, travelPlanId, estimatedCost);
            if (wouldExceedBudget)
                return ServiceResponse<ActivityData>.Fail("This activity would exceed the planned budget.");

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

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(location))
                return false;

            if (estimatedCost < 0)
                return false;

            if (!Enum.IsDefined(typeof(ActivityStatus), status))
                return false;

            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return false;

            if (date.Date < plan.StartDate.Date || date.Date > plan.EndDate.Date)
                return false;

            if (destinationId.HasValue)
            {
                var destination = await db.Destinations
                    .FirstOrDefaultAsync(d => d.Id == destinationId.Value && d.TravelPlanId == travelPlanId);

                if (destination is null)
                    return false;

                if (date.Date < destination.ArrivalDate.Date || date.Date > destination.DepartureDate.Date)
                    return false;
            }

            var hasConflict = await HasActivityScheduleConflictAsync(db, travelPlanId, date, time, id);
            if (hasConflict)
                return false;

            var wouldExceedBudget = await WouldExceedBudgetWithActivityAsync(db, travelPlanId, estimatedCost, id);
            if (wouldExceedBudget)
                return false;

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

            if (!Enum.IsDefined(typeof(ExpenseCategory), category))
                return ServiceResponse<ExpenseData>.Fail("Invalid expense category.");

            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return ServiceResponse<ExpenseData>.Fail("Travel plan not found.");

            if (date.Date < plan.StartDate.Date || date.Date > plan.EndDate.Date)
                return ServiceResponse<ExpenseData>.Fail("Expense date must be within the travel plan period.");

            var wouldExceedBudget = await WouldExceedBudgetWithExpenseAsync(db, travelPlanId, amount);
            if (wouldExceedBudget)
                return ServiceResponse<ExpenseData>.Fail("This expense would exceed the planned budget.");

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

            if (!Enum.IsDefined(typeof(ExpenseCategory), category))
                return false;

            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return false;

            if (date.Date < plan.StartDate.Date || date.Date > plan.EndDate.Date)
                return false;

            var wouldExceedBudget = await WouldExceedBudgetWithExpenseAsync(db, travelPlanId, amount, id);
            if (wouldExceedBudget)
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

            var exists = await db.ChecklistItems.AnyAsync(c =>
                c.TravelPlanId == travelPlanId &&
                c.Text.ToLower() == text.ToLower());

            if (exists)
                return ServiceResponse<ChecklistItemData>.Fail("Checklist item already exists.");

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

            var duplicateExists = await db.ChecklistItems.AnyAsync(c =>
                c.TravelPlanId == travelPlanId &&
                c.Id != id &&
                c.Text.ToLower() == text.ToLower());

            if (duplicateExists)
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

        public async Task<List<ReminderData>> GetRemindersAsync(Guid travelPlanId)
        {
            await using var db = CreateDbContext();

            var reminders = await db.Reminders
                .AsNoTracking()
                .Where(r => r.TravelPlanId == travelPlanId)
                .OrderBy(r => r.RemindAt)
                .ToListAsync();

            return reminders.Select(MapToReminderData).ToList();
        }

        public async Task<ReminderData?> GetReminderByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var reminder = await db.Reminders
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.TravelPlanId == travelPlanId);

            return reminder is null ? null : MapToReminderData(reminder);
        }

        public async Task<ServiceResponse<ReminderData>> CreateReminderAsync(Guid travelPlanId, string title, DateTime remindAt)
        {
            await using var db = CreateDbContext();

            title = title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(title))
                return ServiceResponse<ReminderData>.Fail("Reminder title is required.");

            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return ServiceResponse<ReminderData>.Fail("Travel plan not found.");

            if (remindAt < plan.StartDate || remindAt > plan.EndDate.AddDays(1).AddTicks(-1))
                return ServiceResponse<ReminderData>.Fail("Reminder must be within the travel plan period.");

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

            return ServiceResponse<ReminderData>.Ok(MapToReminderData(reminder));
        }

        public async Task<bool> UpdateReminderAsync(Guid travelPlanId, Guid id, string title, DateTime remindAt, bool isCompleted)
        {
            await using var db = CreateDbContext();

            var reminder = await db.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.TravelPlanId == travelPlanId);

            if (reminder is null)
                return false;

            title = title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(title))
                return false;

            var plan = await db.TravelPlans.FirstOrDefaultAsync(p => p.Id == travelPlanId);
            if (plan is null)
                return false;

            if (remindAt < plan.StartDate || remindAt > plan.EndDate.AddDays(1).AddTicks(-1))
                return false;

            reminder.Title = title;
            reminder.RemindAt = remindAt;
            reminder.IsCompleted = isCompleted;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReminderAsync(Guid travelPlanId, Guid id)
        {
            await using var db = CreateDbContext();

            var reminder = await db.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.TravelPlanId == travelPlanId);

            if (reminder is null)
                return false;

            db.Reminders.Remove(reminder);
            await db.SaveChangesAsync();
            return true;
        }

        private PlanDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<PlanDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new PlanDbContext(options);
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

        private static ReminderData MapToReminderData(Reminder reminder)
        {
            return new ReminderData
            {
                Id = reminder.Id,
                TravelPlanId = reminder.TravelPlanId,
                Title = reminder.Title,
                RemindAt = reminder.RemindAt,
                IsCompleted = reminder.IsCompleted
            };
        }
    }
}

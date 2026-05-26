using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.PlanService.Services.Activities;
using TravelPlanner.PlanService.Services.Checklist;
using TravelPlanner.PlanService.Services.Common;
using TravelPlanner.PlanService.Services.Destinations;
using TravelPlanner.PlanService.Services.Expenses;
using TravelPlanner.PlanService.Services.Plans;
using TravelPlanner.PlanService.Services.Reminders;
using TravelPlanner.PlanService.Validation;

namespace TravelPlanner.PlanService
{
    internal sealed class PlanService : StatefulService, IPlanService
    {
        private readonly IPlanCrudService _planCrudService;
        private readonly IDestinationCrudService _destinationCrudService;
        private readonly IActivityCrudService _activityCrudService;
        private readonly IExpenseCrudService _expenseCrudService;
        private readonly IChecklistCrudService _checklistCrudService;
        private readonly IReminderCrudService _reminderCrudService;

        public PlanService(StatefulServiceContext context) : base(context)
        {
            IPlanServiceDbContextFactory dbFactory = new PlanServiceDbContextFactory();
            IBudgetCalculationService budgetCalculationService = new BudgetCalculationService();

            IPlanValidationService planValidationService = new PlanValidationService();
            IDestinationValidationService destinationValidationService = new DestinationValidationService();
            IActivityValidationService activityValidationService = new ActivityValidationService();
            IExpenseValidationService expenseValidationService = new ExpenseValidationService();
            IChecklistValidationService checklistValidationService = new ChecklistValidationService();
            IReminderValidationService reminderValidationService = new ReminderValidationService();

            _planCrudService = new PlanCrudService(dbFactory, budgetCalculationService, planValidationService);
            _destinationCrudService = new DestinationCrudService(dbFactory, destinationValidationService);
            _activityCrudService = new ActivityCrudService(dbFactory, activityValidationService, budgetCalculationService);
            _expenseCrudService = new ExpenseCrudService(dbFactory, expenseValidationService, budgetCalculationService);
            _checklistCrudService = new ChecklistCrudService(dbFactory, checklistValidationService);
            _reminderCrudService = new ReminderCrudService(dbFactory, reminderValidationService);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<List<TravelPlanData>> GetAllPlansByOwnerAsync(Guid ownerId)
            => _planCrudService.GetAllPlansByOwnerAsync(ownerId);

        public Task<TravelPlanData?> GetPlanByIdAsync(Guid id)
            => _planCrudService.GetPlanByIdAsync(id);

        public Task<ServiceResponse<TravelPlanData>> CreatePlanAsync(
            Guid ownerId,
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            decimal plannedBudget,
            string notes)
            => _planCrudService.CreatePlanAsync(ownerId, title, description, startDate, endDate, plannedBudget, notes);

        public Task<bool> UpdatePlanAsync(
            Guid id,
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            decimal plannedBudget,
            string notes)
            => _planCrudService.UpdatePlanAsync(id, title, description, startDate, endDate, plannedBudget, notes);

        public Task<bool> DeletePlanAsync(Guid id)
            => _planCrudService.DeletePlanAsync(id);

        public Task<List<DestinationData>> GetDestinationsAsync(Guid travelPlanId)
            => _destinationCrudService.GetDestinationsAsync(travelPlanId);

        public Task<DestinationData?> GetDestinationByIdAsync(Guid travelPlanId, Guid id)
            => _destinationCrudService.GetDestinationByIdAsync(travelPlanId, id);

        public Task<ServiceResponse<DestinationData>> CreateDestinationAsync(
            Guid travelPlanId,
            string name,
            string location,
            DateTime arrivalDate,
            DateTime departureDate,
            string description)
            => _destinationCrudService.CreateDestinationAsync(travelPlanId, name, location, arrivalDate, departureDate, description);

        public Task<bool> UpdateDestinationAsync(
            Guid travelPlanId,
            Guid id,
            string name,
            string location,
            DateTime arrivalDate,
            DateTime departureDate,
            string description)
            => _destinationCrudService.UpdateDestinationAsync(travelPlanId, id, name, location, arrivalDate, departureDate, description);

        public Task<bool> DeleteDestinationAsync(Guid travelPlanId, Guid id)
            => _destinationCrudService.DeleteDestinationAsync(travelPlanId, id);

        public Task<List<ActivityData>> GetActivitiesAsync(Guid travelPlanId)
            => _activityCrudService.GetActivitiesAsync(travelPlanId);

        public Task<ActivityData?> GetActivityByIdAsync(Guid travelPlanId, Guid id)
            => _activityCrudService.GetActivityByIdAsync(travelPlanId, id);

        public Task<ServiceResponse<ActivityData>> CreateActivityAsync(
            Guid travelPlanId,
            Guid? destinationId,
            string name,
            DateTime date,
            TimeSpan time,
            string location,
            string description,
            decimal estimatedCost,
            ActivityStatus status)
            => _activityCrudService.CreateActivityAsync(travelPlanId, destinationId, name, date, time, location, description, estimatedCost, status);

        public Task<bool> UpdateActivityAsync(
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
            => _activityCrudService.UpdateActivityAsync(travelPlanId, id, destinationId, name, date, time, location, description, estimatedCost, status);

        public Task<bool> DeleteActivityAsync(Guid travelPlanId, Guid id)
            => _activityCrudService.DeleteActivityAsync(travelPlanId, id);

        public Task<List<ExpenseData>> GetExpensesAsync(Guid travelPlanId)
            => _expenseCrudService.GetExpensesAsync(travelPlanId);

        public Task<ExpenseData?> GetExpenseByIdAsync(Guid travelPlanId, Guid id)
            => _expenseCrudService.GetExpenseByIdAsync(travelPlanId, id);

        public Task<ServiceResponse<ExpenseData>> CreateExpenseAsync(
            Guid travelPlanId,
            string name,
            ExpenseCategory category,
            decimal amount,
            DateTime date,
            string description)
            => _expenseCrudService.CreateExpenseAsync(travelPlanId, name, category, amount, date, description);

        public Task<bool> UpdateExpenseAsync(
            Guid travelPlanId,
            Guid id,
            string name,
            ExpenseCategory category,
            decimal amount,
            DateTime date,
            string description)
            => _expenseCrudService.UpdateExpenseAsync(travelPlanId, id, name, category, amount, date, description);

        public Task<bool> DeleteExpenseAsync(Guid travelPlanId, Guid id)
            => _expenseCrudService.DeleteExpenseAsync(travelPlanId, id);

        public Task<List<ChecklistItemData>> GetChecklistItemsAsync(Guid travelPlanId)
            => _checklistCrudService.GetChecklistItemsAsync(travelPlanId);

        public Task<ChecklistItemData?> GetChecklistItemByIdAsync(Guid travelPlanId, Guid id)
            => _checklistCrudService.GetChecklistItemByIdAsync(travelPlanId, id);

        public Task<ServiceResponse<ChecklistItemData>> CreateChecklistItemAsync(Guid travelPlanId, string text)
            => _checklistCrudService.CreateChecklistItemAsync(travelPlanId, text);

        public Task<bool> UpdateChecklistItemAsync(Guid travelPlanId, Guid id, string text, bool isCompleted)
            => _checklistCrudService.UpdateChecklistItemAsync(travelPlanId, id, text, isCompleted);

        public Task<bool> DeleteChecklistItemAsync(Guid travelPlanId, Guid id)
            => _checklistCrudService.DeleteChecklistItemAsync(travelPlanId, id);

        public Task<List<ReminderData>> GetRemindersAsync(Guid travelPlanId)
            => _reminderCrudService.GetRemindersAsync(travelPlanId);

        public Task<ReminderData?> GetReminderByIdAsync(Guid travelPlanId, Guid id)
            => _reminderCrudService.GetReminderByIdAsync(travelPlanId, id);

        public Task<ServiceResponse<ReminderData>> CreateReminderAsync(Guid travelPlanId, string title, DateTime remindAt)
            => _reminderCrudService.CreateReminderAsync(travelPlanId, title, remindAt);

        public Task<bool> UpdateReminderAsync(Guid travelPlanId, Guid id, string title, DateTime remindAt, bool isCompleted)
            => _reminderCrudService.UpdateReminderAsync(travelPlanId, id, title, remindAt, isCompleted);

        public Task<bool> DeleteReminderAsync(Guid travelPlanId, Guid id)
            => _reminderCrudService.DeleteReminderAsync(travelPlanId, id);
    }
}

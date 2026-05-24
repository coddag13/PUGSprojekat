using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelPlanner.Infrastructure.Entities
{
    public class TravelPlan
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal PlannedBudget { get; set; }

        public string Notes { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public User Owner { get; set; } = null!;

        public ICollection<Destination> Destinations { get; set; } = new List<Destination>();

        public ICollection<PlanActivity> Activities { get; set; } = new List<PlanActivity>();

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

        public ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();

        public ICollection<ShareToken> ShareTokens { get; set; } = new List<ShareToken>();
    }
}

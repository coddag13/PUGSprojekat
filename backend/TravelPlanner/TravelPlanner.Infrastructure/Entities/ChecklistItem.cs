using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelPlanner.Infrastructure.Entities
{
    public class ChecklistItem
    {
        public Guid Id { get; set; }

        public Guid TravelPlanId { get; set; }

        public string Text { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public TravelPlan TravelPlan { get; set; } = null!;
    }
}

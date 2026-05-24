using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TravelPlanner.Common.Enums;

namespace TravelPlanner.Common.Models
{
    public class ActivityData
    {
        public Guid Id { get; set; }
        public Guid TravelPlanId { get; set; }
        public Guid? DestinationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public ActivityStatus Status { get; set; }
    }
}

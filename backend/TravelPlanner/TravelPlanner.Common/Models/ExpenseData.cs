using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TravelPlanner.Common.Enums;

namespace TravelPlanner.Common.Models
{
    public class ExpenseData
    {
        public Guid Id { get; set; }
        public Guid TravelPlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ExpenseCategory Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
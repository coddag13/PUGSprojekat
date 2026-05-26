using TravelPlanner.Common.Enums;

namespace TravelPlanner.Infrastructure.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public ICollection<TravelPlan> TravelPlans { get; set; } = new List<TravelPlan>();
    }
}

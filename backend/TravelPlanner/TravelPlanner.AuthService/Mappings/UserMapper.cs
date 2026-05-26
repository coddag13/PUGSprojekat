using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;

namespace TravelPlanner.AuthService.Mappings
{
    internal static class UserMapper
    {
        public static UserData Map(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return new UserData
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}

using TravelPlanner.Common;
using TravelPlanner.Common.Models;

namespace TravelPlanner.AuthService.Services.Auth
{
    internal interface IAuthApplicationService
    {
        Task<ServiceResponse<UserData>> RegisterAsync(string firstName, string lastName, string email, string password);
        Task<ServiceResponse<UserData>> LoginAsync(string email, string password);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Common.Models;

namespace TravelPlanner.Common.Interfaces
{
    public interface IAuthService : IService
    {
        Task<ServiceResponse<UserData>> RegisterAsync(string firstName, string lastName, string email, string password);
        Task<ServiceResponse<UserData>> LoginAsync(string email, string password);
    }
}
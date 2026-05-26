using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using TravelPlanner.AuthService.Services.Auth;
using TravelPlanner.AuthService.Services.Common;
using TravelPlanner.AuthService.Validation;
using TravelPlanner.Common;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;

namespace TravelPlanner.AuthService
{
    internal sealed class AuthService : StatelessService, IAuthService
    {
        private readonly IAuthApplicationService _authApplicationService;

        public AuthService(StatelessServiceContext context) : base(context)
        {
            IAuthDbContextFactory dbContextFactory = new AuthDbContextFactory();
            IAuthValidationService validationService = new AuthValidationService();

            _authApplicationService = new AuthApplicationService(
                dbContextFactory,
                validationService);
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<ServiceResponse<UserData>> RegisterAsync(
            string firstName,
            string lastName,
            string email,
            string password)
            => _authApplicationService.RegisterAsync(firstName, lastName, email, password);

        public Task<ServiceResponse<UserData>> LoginAsync(string email, string password)
            => _authApplicationService.LoginAsync(email, password);
    }
}

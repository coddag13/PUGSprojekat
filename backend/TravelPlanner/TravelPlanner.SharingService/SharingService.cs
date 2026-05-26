using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.SharingService.Services.Common;
using TravelPlanner.SharingService.Services.Tokens;
using TravelPlanner.SharingService.Validation;

namespace TravelPlanner.SharingService
{
    internal sealed class SharingService : StatelessService, ISharingService
    {
        private readonly IShareTokenCrudService _shareTokenCrudService;

        public SharingService(StatelessServiceContext context) : base(context)
        {
            ISharingDbContextFactory dbContextFactory = new SharingDbContextFactory();
            IPlanLookupService planLookupService = new PlanLookupService();
            IShareTokenValidationService validationService = new ShareTokenValidationService();

            _shareTokenCrudService = new ShareTokenCrudService(
                dbContextFactory,
                planLookupService,
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

        public Task<List<ShareTokenData>> GetTokensByPlanAsync(Guid travelPlanId)
            => _shareTokenCrudService.GetTokensByPlanAsync(travelPlanId);

        public Task<ServiceResponse<ShareTokenData>> CreateTokenAsync(
            Guid travelPlanId,
            ShareAccessType accessType,
            DateTime expiresAt)
            => _shareTokenCrudService.CreateTokenAsync(travelPlanId, accessType, expiresAt);

        public Task<bool> DeleteTokenAsync(Guid travelPlanId, Guid id)
            => _shareTokenCrudService.DeleteTokenAsync(travelPlanId, id);

        public Task<ServiceResponse<ShareTokenData>> ValidateTokenAsync(string token)
            => _shareTokenCrudService.ValidateTokenAsync(token);

        public Task<ServiceResponse<ShareTokenData>> ValidateTokenForPlanAsync(Guid travelPlanId, string token)
            => _shareTokenCrudService.ValidateTokenForPlanAsync(travelPlanId, token);

        public Task DeleteTokensByPlanAsync(Guid travelPlanId)
            => _shareTokenCrudService.DeleteTokensByPlanAsync(travelPlanId);
    }
}

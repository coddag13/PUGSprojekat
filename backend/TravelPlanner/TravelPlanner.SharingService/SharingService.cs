using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.SharingService
{
    internal sealed class SharingService : StatelessService, ISharingService
    {
        private readonly string _connectionString;

        public SharingService(StatelessServiceContext context) : base(context)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<List<ShareTokenData>> GetTokensByPlanAsync(Guid travelPlanId)
        {
            using var db = CreateDbContext();
            var tokens = await db.ShareTokens
                .Where(t => t.TravelPlanId == travelPlanId)
                .ToListAsync();
            return tokens.Select(MapToData).ToList();
        }

        public async Task<ServiceResponse<ShareTokenData>> CreateTokenAsync(Guid travelPlanId, ShareAccessType accessType, DateTime expiresAt)
        {
            using var db = CreateDbContext();
            var plan = await PlanService.GetPlanByIdAsync(travelPlanId);
            if (plan is null) return ServiceResponse<ShareTokenData>.Fail("Travel plan not found.");

            if (expiresAt <= DateTime.UtcNow)
                return ServiceResponse<ShareTokenData>.Fail("Expiry date must be in the future.");

            var token = new ShareToken
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Token = Guid.NewGuid().ToString("N"),
                AccessType = accessType,
                ExpiresAt = expiresAt
            };
            db.ShareTokens.Add(token);
            await db.SaveChangesAsync();
            return ServiceResponse<ShareTokenData>.Ok(MapToData(token));
        }

        public async Task<bool> DeleteTokenAsync(Guid travelPlanId, Guid id)
        {
            using var db = CreateDbContext();
            var token = await db.ShareTokens
                .FirstOrDefaultAsync(t => t.Id == id && t.TravelPlanId == travelPlanId);
            if (token is null) return false;

            db.ShareTokens.Remove(token);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceResponse<ShareTokenData>> ValidateTokenAsync(string token)
        {
            using var db = CreateDbContext();
            token = token?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                return ServiceResponse<ShareTokenData>.Fail("Token is required.");

            var shareToken = await db.ShareTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Token == token);

            if (shareToken is null) return ServiceResponse<ShareTokenData>.Fail("Token not found.");
            if (shareToken.ExpiresAt < DateTime.UtcNow) return ServiceResponse<ShareTokenData>.Fail("Token has expired.");

            return ServiceResponse<ShareTokenData>.Ok(MapToData(shareToken));
        }

        public async Task<ServiceResponse<ShareTokenData>> ValidateTokenForPlanAsync(Guid travelPlanId, string token)
        {
            var result = await ValidateTokenAsync(token);
            if (!result.Success)
                return result;

            if (result.Data!.TravelPlanId != travelPlanId)
                return ServiceResponse<ShareTokenData>.Fail("Token does not belong to this travel plan.");

            return result;
        }

        public async Task DeleteTokensByPlanAsync(Guid travelPlanId)
        {
            using var db = CreateDbContext();
            var tokens = await db.ShareTokens.Where(t => t.TravelPlanId == travelPlanId).ToListAsync();

            if (tokens.Count == 0)
                return;

            db.ShareTokens.RemoveRange(tokens);
            await db.SaveChangesAsync();
        }

        private static IPlanService PlanService =>
            ServiceProxy.Create<IPlanService>(
                new Uri("fabric:/TravelPlanner/TravelPlanner.PlanService"),
                new ServicePartitionKey(0));

        private SharingDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<SharingDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
            return new SharingDbContext(options);
        }

        private static ShareTokenData MapToData(ShareToken t) => new()
        {
            Id = t.Id,
            TravelPlanId = t.TravelPlanId,
            Token = t.Token,
            AccessType = t.AccessType,
            ExpiresAt = t.ExpiresAt
        };
    }
}

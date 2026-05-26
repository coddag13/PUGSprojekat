using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common;
using TravelPlanner.Common.Enums;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.SharingService.Mappings;
using TravelPlanner.SharingService.Services.Common;
using TravelPlanner.SharingService.Validation;

namespace TravelPlanner.SharingService.Services.Tokens
{
    internal sealed class ShareTokenCrudService : IShareTokenCrudService
    {
        private readonly ISharingDbContextFactory _dbContextFactory;
        private readonly IPlanLookupService _planLookupService;
        private readonly IShareTokenValidationService _validationService;

        public ShareTokenCrudService(
            ISharingDbContextFactory dbContextFactory,
            IPlanLookupService planLookupService,
            IShareTokenValidationService validationService)
        {
            _dbContextFactory = dbContextFactory;
            _planLookupService = planLookupService;
            _validationService = validationService;
        }

        public async Task<List<ShareTokenData>> GetTokensByPlanAsync(Guid travelPlanId)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var tokens = await db.ShareTokens
                .AsNoTracking()
                .Where(t => t.TravelPlanId == travelPlanId)
                .ToListAsync();

            return tokens.Select(ShareTokenMapper.Map).ToList();
        }

        public async Task<ServiceResponse<ShareTokenData>> CreateTokenAsync(
            Guid travelPlanId,
            ShareAccessType accessType,
            DateTime expiresAt)
        {
            var expiryValidationError = _validationService.ValidateExpiry(expiresAt);
            if (expiryValidationError is not null)
                return ServiceResponse<ShareTokenData>.Fail(expiryValidationError);

            var plan = await _planLookupService.GetPlanByIdAsync(travelPlanId);
            if (plan is null)
                return ServiceResponse<ShareTokenData>.Fail("Travel plan not found.");

            await using var db = _dbContextFactory.CreateDbContext();

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

            return ServiceResponse<ShareTokenData>.Ok(ShareTokenMapper.Map(token));
        }

        public async Task<bool> DeleteTokenAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var token = await db.ShareTokens
                .FirstOrDefaultAsync(t => t.Id == id && t.TravelPlanId == travelPlanId);

            if (token is null)
                return false;

            db.ShareTokens.Remove(token);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceResponse<ShareTokenData>> ValidateTokenAsync(string token)
        {
            token = token?.Trim() ?? string.Empty;

            var tokenValidationError = _validationService.ValidateTokenValue(token);
            if (tokenValidationError is not null)
                return ServiceResponse<ShareTokenData>.Fail(tokenValidationError);

            await using var db = _dbContextFactory.CreateDbContext();

            var shareToken = await db.ShareTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Token == token);

            if (shareToken is null)
                return ServiceResponse<ShareTokenData>.Fail("Token not found.");

            if (shareToken.ExpiresAt < DateTime.UtcNow)
                return ServiceResponse<ShareTokenData>.Fail("Token has expired.");

            return ServiceResponse<ShareTokenData>.Ok(ShareTokenMapper.Map(shareToken));
        }

        public async Task<ServiceResponse<ShareTokenData>> ValidateTokenForPlanAsync(Guid travelPlanId, string token)
        {
            var result = await ValidateTokenAsync(token);
            if (!result.Success)
                return result;

            var planValidationError = _validationService.ValidateTokenBelongsToPlan(result.Data!, travelPlanId);
            if (planValidationError is not null)
                return ServiceResponse<ShareTokenData>.Fail(planValidationError);

            return result;
        }

        public async Task DeleteTokensByPlanAsync(Guid travelPlanId)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var tokens = await db.ShareTokens
                .Where(t => t.TravelPlanId == travelPlanId)
                .ToListAsync();

            if (tokens.Count == 0)
                return;

            db.ShareTokens.RemoveRange(tokens);
            await db.SaveChangesAsync();
        }
    }
}

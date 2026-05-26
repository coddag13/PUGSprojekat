using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TravelPlanner.WebApi.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst(JwtRegisteredClaimNames.Sub);
            return Guid.Parse(claim!.Value);
        }

        public static bool IsAdminUser(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }
    }
}

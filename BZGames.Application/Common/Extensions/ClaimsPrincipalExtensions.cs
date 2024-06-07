using System.Security.Claims;

namespace BZGames.Application.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst("userName")?.Value;
        }
    }
}

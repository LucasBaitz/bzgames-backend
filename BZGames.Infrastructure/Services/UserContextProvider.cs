using BZGames.Application.Interfaces.Services;
using BZGames.Infrastructure.Models;
using System.Security.Claims;

namespace BZGames.Infrastructure.Services
{
    public class UserContextProvider : IUserContextProvider
    {
        public async Task<UserContext> GetUserContext(ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst("id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new InvalidDataException("Missing credentials.");
            }

            return new UserContext() { UserId = userId };
        }
    }

}

using BZGames.Infrastructure.Models;
using System.Security.Claims;

namespace BZGames.Application.Interfaces.Services
{
    public interface IUserContextProvider
    {
        Task<UserContext> GetUserContext(ClaimsPrincipal userClaims);
    }

}

using BZGames.Application.DTOs.Auth;

namespace BZGames.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> Register(UserRegistration registration);
        Task<JwtData> Login(UserCredentials credentials);
    }

}

using BZGames.Domain.Entities;

namespace BZGames.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}

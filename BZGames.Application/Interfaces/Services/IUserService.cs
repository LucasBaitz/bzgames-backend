using BZGames.Application.DTOs.Auth;
using BZGames.Domain.Entities;
using BZGames.Infrastructure.Models;

namespace BZGames.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task DeleteUser(Guid userId);
        Task<User?> GetUserById(Guid id);
        Task UpdateUser(UserContext userContext, UserInfo updateDto);
        Task<IEnumerable<User>> GetAllUsers();
        Task<UserInfo> ReadAccountInfo(UserContext userContext);
    }
}

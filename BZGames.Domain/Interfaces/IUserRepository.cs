using BZGames.Domain.Entities;

namespace BZGames.Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetUserById(Guid id);
        public Task DeleteUser(User user);
        public Task UpdateUser(User updatedUser);
        public Task<IEnumerable<User>> GetAllUsers();
    }
}

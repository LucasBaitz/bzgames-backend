using BZGames.Domain.Entities;
using BZGames.Domain.Interfaces;
using BZGames.Infrastructure.Common.Interfaces;
using BZGames.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BZGames.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbSet<User> _users;
        private readonly IUnitOfWork _unitOfWork;

        public UserRepository(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _users = context.Users;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteUser(User user)
        {
            _users.Remove(user);

            await _unitOfWork.SaveChanges();
            _unitOfWork.Dispose();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _users.ToListAsync();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateUser(User updatedUser)
        {
            _users.Update(updatedUser);

            await _unitOfWork.SaveChanges();
            _unitOfWork.Dispose();
        }
    }
}

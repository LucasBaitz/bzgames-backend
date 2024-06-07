using AutoMapper;
using BZGames.Application.DTOs.Auth;
using BZGames.Application.Interfaces.Services;
using BZGames.Domain.Entities;
using BZGames.Domain.Interfaces;
using BZGames.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;


namespace BZGames.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(UserManager<User> userManager, IUserRepository userRepository, IMapper mapper)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task DeleteUser(Guid userId)
        {
            var user = await _userRepository.GetUserById(userId);
            await _userRepository.DeleteUser(user);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<UserInfo> ReadAccountInfo(UserContext userContext)
        {
            var user = await _userRepository.GetUserById(userContext.UserId);

            var userInfo = _mapper.Map<UserInfo>(user);

            return userInfo;
        }

        public async Task UpdateUser(UserContext userContext, UserInfo updatedInfo)
        {
            var user = await _userRepository.GetUserById(userContext.UserId);

            user.UserName = updatedInfo.UserName;
            user.Email = updatedInfo.Email;
            user.Image = updatedInfo.Image;

            await _userRepository.UpdateUser(user);
        }
    }
}

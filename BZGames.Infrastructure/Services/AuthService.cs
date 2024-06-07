using AutoMapper;
using BZGames.Application.DTOs.Auth;
using BZGames.Application.Interfaces.Services;
using BZGames.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BZGames.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenGenerator _tokenService;
        private readonly IMapper _mapper;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IJwtTokenGenerator tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        public async Task<bool> Register(UserRegistration registrationData)
        {
            var newUser = _mapper.Map<User>(registrationData);
            newUser.Rating = 5.0;

            var createResult = await _userManager.CreateAsync(newUser, registrationData.Password);

            return createResult == IdentityResult.Success;
        }

        public async Task<JwtData> Login(UserCredentials credentials)
        {
            var loginAttempt = await _signInManager.PasswordSignInAsync(credentials.UserName, credentials.Password, false, false);

            if (!loginAttempt.Succeeded)
                throw new Exception();

            var user = await _signInManager.UserManager.Users
                .FirstOrDefaultAsync(user => user.NormalizedUserName == credentials.UserName.ToUpper())
                ?? throw new Exception();

            string token = _tokenService.GenerateToken(user);

            JwtData jwwData = new (user.UserName, user.Email, user.Image, token);

            return jwwData;
        }
        
    }

}

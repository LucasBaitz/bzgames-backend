using BZGames.Application.DTOs.Auth;
using BZGames.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BZGames.API.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserCredentials credentials)
        {
            JwtData jwt = await _authService.Login(credentials);

            return Ok(jwt);
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration(UserRegistration registrationData)
        {
            var result = await _authService.Register(registrationData);

            return result ? Ok() : BadRequest();
        }


        [HttpPost]
        [Authorize]
        [Route("TestAuth")]
        public async Task<IActionResult> IsAuthorized()
        {
            return Ok();
        }
    }
}

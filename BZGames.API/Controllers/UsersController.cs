using BZGames.Application.DTOs.Auth;
using BZGames.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BZGames.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserContextProvider _userContextProvider;
        private readonly string _imageStoragePath;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IUserContextProvider userContextProvider, IConfiguration configuration, ILogger<UsersController> logger)
        {
            _userService = userService;
            _userContextProvider = userContextProvider;
            _imageStoragePath = configuration.GetSection("ConnectionStrings:ImageStorage").Value!;
            _logger = logger;
        }

        [HttpGet]
        [Route("Get/All")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsers();

            return Ok(users);
        }

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = _userService.GetAllUsers();

            return Ok(user);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateUser(UserInfo userInfo)
        {
            var userContext = await _userContextProvider.GetUserContext(this.User);

            await _userService.UpdateUser(userContext, userInfo);

            return NoContent();
        }

        [HttpPost]
        [Route("Upload/Picture")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile image)
        {
            var userContext = await _userContextProvider.GetUserContext(this.User);
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded");
            }

            try
            {
                if (!Directory.Exists(_imageStoragePath))
                {
                    Directory.CreateDirectory(_imageStoragePath);
                }

                string fileExtension = Path.GetExtension(image.FileName);
                string uniqueFileName = $"{userContext.UserId}{fileExtension}";
                string filePath = Path.Combine(_imageStoragePath, uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var user = await _userService.GetUserById(userContext.UserId);

                // Construct image URL using a relative path
                string relativePath = Path.Combine("UsersPictures", uniqueFileName);
                string imageUrl = $"{Request.Scheme}://{Request.Host}/{relativePath.Replace("\\", "/")}";

                _logger.LogInformation(imageUrl); // Use LogInformation for regular logging

                var userUpdate = new UserInfo(user.UserName, user.Email, imageUrl);
                await _userService.UpdateUser(userContext, userUpdate);

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

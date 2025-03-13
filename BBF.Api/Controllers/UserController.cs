using BBF.Api.DTOs;
using BBF.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBF.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserProfileService userProfileService, ILogger<UserController> logger)
        {
            _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            try
            {
                var token = Request.Headers.Authorization.ToString().Replace("Bearer", "");

                var userProfile = await _userProfileService.GetUserProfileAsync(userId, token);

                if (userProfile == null)
                {
                    return NotFound($"Usuario con ID {userId} no encontrado");
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el perfil del usuario: {Message}", ex.Message);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserListDto))]
        public async Task<IActionResult> GetAllUserProfiles([FromQuery] int page = 1)
        {
            try
            {
                var token = Request.Headers.Authorization.ToString().Replace("Bearer", "");

                var userProfiles = await _userProfileService.GetAllUserProfilesAsync(page, token);
                return Ok(userProfiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los perfiles de usuario: {Message}", ex.Message);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}

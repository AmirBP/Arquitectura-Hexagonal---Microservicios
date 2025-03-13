using Microsoft.AspNetCore.Mvc;
using UserDataService.Application.DTOs;
using UserDataService.Domain.Ports;

namespace UserDataService.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserDataController : ControllerBase
    {
        private readonly IUserDataService _userDataService;
        private readonly ILogger<UserDataController> _logger;

        public UserDataController(IUserDataService userDataService, ILogger<UserDataController> logger)
        {
            _userDataService = userDataService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDataResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var userData = await _userDataService.GetUserByIdAsync(id);

                if (userData == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                var response = new UserDataResponseDto
                {
                    Id = userData.Id,
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                    Avatar = userData.Avatar
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por ID {id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserListResponseDto))]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1)
        {
            try
            {
                var users = await _userDataService.GetAllUsersAsync(page);

                var response = new UserListResponseDto
                {
                    Page = page,
                    TotalPages = 2,
                    Users = users.Select(u => new UserDataResponseDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Avatar = u.Avatar
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}

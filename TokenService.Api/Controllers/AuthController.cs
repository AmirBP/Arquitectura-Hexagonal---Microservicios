using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TokenService.Application.DTOs;
using TokenService.Application.Services;

namespace TokenService.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                if (response == null)
                {
                    _logger.LogWarning("Login fallido para: {Email}", request.Email);
                    return Unauthorized("Credenciales inválidas");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login: {Message}", ex.Message);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("refresh")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request);
                if (response == null)
                    return Unauthorized("Refresh Token inválido");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al refrescar token: {Message}", ex.Message);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}

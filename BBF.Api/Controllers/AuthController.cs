using BBF.Api.DTOs;
using BBF.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBF.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenServiceClient _tokenServiceClient;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ITokenServiceClient tokenServiceClient, ILogger<AuthController> logger)
        {
            _tokenServiceClient = tokenServiceClient ?? throw new ArgumentNullException(nameof(tokenServiceClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                _logger.LogInformation("Intento de login para usuario: {Email}", request.Email);

                var response = await _tokenServiceClient.LoginAsync(request);

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var response = await _tokenServiceClient.RefreshTokenAsync(request);

                if (response == null)
                    return Unauthorized("Refresh Token inválido");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el Refresh Token: {Message}", ex.Message);
                return StatusCode(500, "Error interno del servidor");
            }
        }


    }
}

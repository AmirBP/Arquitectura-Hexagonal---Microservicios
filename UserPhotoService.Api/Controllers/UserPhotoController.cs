using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UserPhotoService.Application.DTOs;
using UserPhotoService.Domain.Ports;

namespace UserPhotoService.Api.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class UserPhotoController : Controller
    {
        private readonly IUserPhotoService _userPhotoService;
        private readonly ILogger<UserPhotoController> _logger;

        public UserPhotoController(IUserPhotoService userPhotoService, ILogger<UserPhotoController> logger)
        {
            _userPhotoService = userPhotoService;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserPhotoResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GetUserPhoto(int userId)
        {
            try
            {
                var userPhoto = await _userPhotoService.GetUserPhotoAsync(userId);

                if(userPhoto == null)
                {
                    return NotFound($"Foto del usuario con ID {userId} no encontrada");
                }

                var response = new UserPhotoResponseDto
                {
                    UserId = userPhoto.UserId,
                    PhotoBase64 = $"data:{userPhoto.PhotoType}; base64,{userPhoto.PhotoBase64}",
                    PhotoType = userPhoto.PhotoType
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener foto del usuario {Id}", userId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserPhotoResponseDto>))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GetAllUserPhotos([FromQuery] int page = 1)
        {
            try
            {
                var userPhotos = await _userPhotoService.GetAllUserPhotosAsync(page);

                var response = userPhotos.Select(p => new UserPhotoResponseDto
                {
                    UserId = p.UserId,
                    PhotoBase64 = $"data:{p.PhotoType}; base64,{p.PhotoBase64}",
                    PhotoType = p.PhotoType
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener fotos de los usuarios, página {Page}", page);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}

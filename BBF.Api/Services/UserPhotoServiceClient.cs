using System.Net.Http.Headers;
using BBF.Api.DTOs;
using BBF.Api.Services.Interfaces;

namespace BBF.Api.Services
{
    public class UserPhotoServiceClient : IUserPhotoServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserPhotoServiceClient> _logger;

        public UserPhotoServiceClient(HttpClient httpClient, ILogger<UserPhotoServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserPhotoDto?> GetUserPhotoAsync(int userId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"api/photos/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserPhotoDto>();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Demasiadas solicitudes al servicio de fotos de usuario {UserId}",userId);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener foto de usuario para el ID {UserId}", userId);
                return null;
            }
        }

        public async Task<IEnumerable<UserPhotoDto>> GetAllUserPhotosAsync(int page, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"api/photos?page={page}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<UserPhotoDto>>() ?? new List<UserPhotoDto>();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Demasiadas solicitudes al servicio de fotos de usuario para la página {Page}", page);
                }

                return new List<UserPhotoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las fotos de usuario para la página {Page}", page);
                return new List<UserPhotoDto>();
            }
        }
    }
}

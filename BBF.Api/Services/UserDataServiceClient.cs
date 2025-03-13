using System.Net.Http.Headers;
using BBF.Api.DTOs;
using BBF.Api.Services.Interfaces;
using UserDataService.Application.DTOs;

namespace BBF.Api.Services
{
    public class UserDataServiceClient : IUserDataServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserDataServiceClient> _logger;

        public UserDataServiceClient(HttpClient httpClient, ILogger<UserDataServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserDataDto?> GetUserByIdAsync(int id, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"api/user/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDataDto>();
                }

                _logger.LogWarning("Error al obtener usuario por ID, Status Code {StatusCode}", response.StatusCode);
                return null;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos de usuario para el ID {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<UserDataDto>> GetAllUsersAsync(int page, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"api/user?page={page}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserListResponseDto>();
                    return result?.Users ?? new List<UserDataDto>();
                }

                _logger.LogWarning("Error al obtener los usuarios, Status Code {StatusCode}", response.StatusCode);
                return new List<UserDataDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos de usuarios para la página {Page}", page);
                return new List<UserDataDto>();
            }
        }

        private class UserListResponseDto
        {
            public int Page { get; set; }
            public int TotalPages { get; set; }
            public List<UserDataDto> Users { get; set; } = new List<UserDataDto>();
        }
    }
}

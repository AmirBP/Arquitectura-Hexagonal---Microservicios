using System.Net.Http.Headers;
using BBF.Api.DTOs;
using BBF.Api.Services.Interfaces;

namespace BBF.Api.Services
{
    public class TokenServiceClient : ITokenServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TokenServiceClient> _logger;

        public TokenServiceClient(HttpClient httpClient, ILogger<TokenServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TokenResponseDto>();
                }

                _logger.LogWarning("Error de Inicio de Sesión, Status Code {StatusCode}", response.StatusCode);

                return null;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error en la solicitud de inicio de sesión");
                return null;
            }
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TokenResponseDto>();
                }

                _logger.LogWarning("Error en el Refresh Token, Status Code {StatusCode}",response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la solicitud de Refresh Token");
                return null;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("api/auth/validate");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la solicitud de validación de token");
                return false;
            }
        }
    }
}

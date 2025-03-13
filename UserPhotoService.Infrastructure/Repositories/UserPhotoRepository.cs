using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using UserPhotoService.Domain.Entities;
using UserPhotoService.Domain.Ports;
using UserPhotoService.Infrastructure.DTOs;

namespace UserPhotoService.Infrastructure.Repositories
{
    public class UserPhotoRepository : IUserPhotoRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserPhotoRepository> _logger;

        public UserPhotoRepository(HttpClient httpClient, IMemoryCache cache, ILogger<UserPhotoRepository> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://reqres.in/api/");
        }

        public async Task<UserPhoto?> GetUserPhotoAsync(int userId)
        {
            try
            {
                string cacheKey = $"UserPhoto_{userId}";

                if(_cache.TryGetValue(cacheKey, out UserPhoto cachedPhoto))
                {
                    return cachedPhoto;
                }

                var response = await _httpClient.GetAsync($"users/{userId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<ReqResUserResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if(userResponse?.Data == null || string.IsNullOrEmpty(userResponse.Data.Avatar))
                {
                    return null;
                }

                var photoData = await ConvertImageToBase64(userResponse.Data.Avatar);

                var userPhoto = new UserPhoto(
                    userId,
                    photoData.Base64Content,
                    photoData.MimeType
                );

                _cache.Set(cacheKey, userPhoto, TimeSpan.FromHours(1));

                return userPhoto;
            }
            catch(HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al obtener foto de usuario con ID: {Id}", userId);
                return null;
            }
        }

        public async Task<IEnumerable<UserPhoto>> GetAllUserPhotosAsync(int page = 1)
        {
            try
            {
                string cacheKey = $"AllUsersPhotos_Page{page}";

                if (_cache.TryGetValue(cacheKey, out IEnumerable<UserPhoto> cachedPhotos))
                {
                    return cachedPhotos;
                }

                var response = await _httpClient.GetAsync($"users?page={page}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var listResponse = JsonSerializer.Deserialize<ReqResListResponse<ReqResUserDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (listResponse?.Data == null)
                {
                    return new List<UserPhoto>();
                }

                var userPhotos = new List<UserPhoto>();

                foreach(var userData in listResponse.Data)
                {
                    if (string.IsNullOrEmpty(userData.Avatar))
                        continue;

                    var photoData = await ConvertImageToBase64(userData.Avatar);

                    userPhotos.Add( new UserPhoto(
                    userData.Id,
                    photoData.Base64Content,
                    photoData.MimeType
                    ));  
                }

                _cache.Set(cacheKey, userPhotos, TimeSpan.FromHours(1));

                return userPhotos;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al obtener fotos de usuarios, página : {Page}", page);
                return new List<UserPhoto>(); ;
            }
        }

        private async Task<(string Base64Content, string MimeType)> ConvertImageToBase64(string imageUrl)
        {
            string cacheKey = $"Image64_{imageUrl}";
            if (_cache.TryGetValue(cacheKey, out (string, string) cachedBase64))
            {
                return cachedBase64;
            }

            try
            {
                using var imageClient = new HttpClient();
                var imageResponse = await imageClient.GetAsync(imageUrl);
                imageResponse.EnsureSuccessStatusCode();

                var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
                var ImageBase64 = Convert.ToBase64String(imageBytes);

                string mimeType = "image/jpg";

                string base64Result = $"data:{mimeType};base64,{ImageBase64}";

                var result = (base64Result, mimeType);

                _cache.Set(cacheKey, base64Result, TimeSpan.FromHours(1));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al convertir la imagen a base64: {Url}", imageUrl);
                return (string.Empty, "image/jpg");
            }
        }
    }
}

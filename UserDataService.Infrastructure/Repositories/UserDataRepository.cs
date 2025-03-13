using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using UserDataService.Domain.Entities;
using UserDataService.Domain.Ports;
using UserDataService.Infrastructure.DTOs;

namespace UserDataService.Infrastructure.Repositories
{
    public class UserDataRepository : IUserDataRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserDataRepository> _logger;
        private readonly IMemoryCache _cache;

        public UserDataRepository(HttpClient httpClient, ILogger<UserDataRepository> logger, IMemoryCache cache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));

            _httpClient.BaseAddress = new Uri("https://reqres.in/api/");
        }

        public async Task<UserData?> GetUserByIdAsync(int id)
        {
            try
            {
                string cacheKey = $"User_{id}";
                if (_cache.TryGetValue(cacheKey, out UserData cachedUser))
                {
                    return cachedUser;
                }

                var response = await _httpClient.GetAsync($"users/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<ReqResUserResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if(userResponse?.Data == null)
                {
                    return null;
                }

                string base64Avatar = await ConvertImageToBase64(userResponse.Data.Avatar);

                var User = new UserData(
                    userResponse.Data.Id,
                    userResponse.Data.First_name,
                    userResponse.Data.Last_name,
                    base64Avatar
                );

                _cache.Set(cacheKey, User, TimeSpan.FromHours(1));

                return User;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID : {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<UserData>> GetAllUsersAsync(int page = 1)
        {
            try
            {
                string cacheKey = $"AllUser_Page{page}";
                if (_cache.TryGetValue(cacheKey, out IEnumerable<UserData> cachedUsers))
                {
                    return cachedUsers;
                }

                var response = await _httpClient.GetAsync($"users?page={page}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var listResponse = JsonSerializer.Deserialize<ReqResListResponse<ReqResUserDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if(listResponse?.Data == null)
                {
                    return new List<UserData>();
                }

                var userDataList = new List<UserData>();

                foreach(var userData in listResponse.Data)
                {
                    string base64Avatar = await ConvertImageToBase64(userData.Avatar);

                    userDataList.Add(new UserData(
                        userData.Id,
                        userData.First_name,
                        userData.Last_name,
                        base64Avatar
                    ));
                }

                _cache.Set(cacheKey, userDataList, TimeSpan.FromHours(1));

                return userDataList;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios, página : {Page}", page);
                return new List<UserData>();
            }
        }

        private async Task<string> ConvertImageToBase64(string imageUrl)
        {
            string cacheKey = $"Avatar_{imageUrl}";
            if (_cache.TryGetValue(cacheKey, out string cachedBase64))
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

                _cache.Set(cacheKey, base64Result, TimeSpan.FromHours(1));

                return base64Result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al convertir la imagen a base64: {Url}", imageUrl);
                return string.Empty;
            }
        }

    }
}

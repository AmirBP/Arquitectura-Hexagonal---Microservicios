using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TokenService.Domain.Entities;
using TokenService.Domain.Ports;
using TokenService.Infrastructure.DTOs;

namespace TokenService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserRepository> _logger;
        private readonly Dictionary<int, User> _cache = new();

        public UserRepository(HttpClient httpClient, ILogger<UserRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            foreach (var user in _cache.Values)
            {
                if (user.Email == email)
                {
                    return Task.FromResult<User?>(user);
                }
            }

            _logger.LogWarning($"Usuario con Email {email} no encontrado en Cache.");
            return Task.FromResult<User?>(null);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            // Verificar caché primero
            if (_cache.TryGetValue(id, out var cachedUser))
            {
                return cachedUser;
            }

            try
            {
                var response = await _httpClient.GetAsync($"users/{id}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var reqresResponse = JsonSerializer.Deserialize<ReqResResponse>(json);

                if (reqresResponse?.Data != null)
                {
                    var user = new User(
                        reqresResponse.Data.Id,
                        reqresResponse.Data.Email,
                        "$2a$12$dummyHashForTesting"
                    );

                    _cache[id] = user;
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al buscar usuario {id}: {ex.Message}");
                return null;
            }
        }

        public Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            if (_cache.TryGetValue(userId, out var user))
            {
                user.SetRefreshToken(refreshToken, refreshTokenExpiryTime);
                return Task.CompletedTask;
            }

            _logger.LogWarning($"No se encontró usuario con ID {userId} para actualizar el refresh token.");
            return Task.CompletedTask;
        }
    }
}

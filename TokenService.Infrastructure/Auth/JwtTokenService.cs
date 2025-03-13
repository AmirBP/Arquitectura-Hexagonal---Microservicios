using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TokenService.Domain.Entities;
using TokenService.Domain.Ports;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TokenService.Infrastructure.Auth
{
    public class JwtTokenService : ITokenService
    {
        public readonly JwtSettings _jwtSettings;
        public readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings, ILogger<JwtTokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public (string refreshToken, DateTime expiryTime) GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            var refreshToken = Convert.ToBase64String(randomNumber);
            var expiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            return (refreshToken, expiryTime);

        }

        public Token GenerateToken(User user)
        {
            // Generar solo el accessToken aquí
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                claims: claims,
                signingCredentials: credentials,
                expires: expiresAt
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            // Usar el refreshToken existente del usuario o generar uno nuevo
            var (refreshToken, _) = GenerateRefreshToken();

            return new Token(accessToken, refreshToken, expiresAt);
        }

        public int? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                }, out _);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Validacion de Token Fallida: {ex.Message}");
                return false;
            }

        }
    }
}

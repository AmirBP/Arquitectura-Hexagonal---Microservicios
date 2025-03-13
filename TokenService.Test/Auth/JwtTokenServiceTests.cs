using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TokenService.Domain.Entities;
using TokenService.Infrastructure.Auth;

namespace TokenService.Test.Auth
{
    public class JwtTokenServiceTests
    {
        private readonly JwtTokenService _jwtTokenService;

        public JwtTokenServiceTests()
        {
            var jwtSettings = new JwtSettings
            {
                Secret = "x9fU7GZPzKUu5iA7Ws1DqMvlVqLXE9FVo/kT5F+r54I=\r\n",
                ExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            };

            var jwtOptions = Options.Create(jwtSettings);
            var loggerMock = new Mock<ILogger<JwtTokenService>>();

            _jwtTokenService = new JwtTokenService(jwtOptions, loggerMock.Object);
        }

        [Fact]
        public void GenerateToken_ValidUser_ReturnsValidToken()
        {
            var user = new User(1, "test@example.com", "hashed_password");

            var token = _jwtTokenService.GenerateToken(user);

            Assert.NotNull(token);
            Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(token.RefreshToken));

            var userId = _jwtTokenService.GetUserIdFromToken(token.AccessToken);
            Assert.Equal(user.Id, userId);
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsValidRefreshToken()
        {
            var (refreshToken, expiryTime) = _jwtTokenService.GenerateRefreshToken();

            Assert.False(string.IsNullOrWhiteSpace(refreshToken));
            Assert.True(expiryTime > DateTime.UtcNow);
        }

        [Fact]
        public void GetUserIdFromToken_ValidToken_ReturnsUserId()
        {
            var user = new User(1, "test@example.com", "hashed_password");
            var token = _jwtTokenService.GenerateToken(user);

            var userId = _jwtTokenService.GetUserIdFromToken(token.AccessToken);

            Assert.Equal(user.Id, userId);
        }

        [Fact]
        public void ValidateToken_ValidToken_ReturnsTrue()
        {
            var user = new User(1, "test@example.com", "hashed_password");
            var token = _jwtTokenService.GenerateToken(user);

            var isValid = _jwtTokenService.ValidateToken(token.AccessToken);

            Assert.True(isValid);
        }

        [Fact]
        public void ValidateToken_InvalidToken_ReturnsFalse()
        {
            var invalidToken = "invalid_token_string";

            var isValid = _jwtTokenService.ValidateToken(invalidToken);

            Assert.False(isValid);
        }
    }


}

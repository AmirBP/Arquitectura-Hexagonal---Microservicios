using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TokenService.Application.DTOs;
using TokenService.Application.Services;
using TokenService.Domain.Entities;
using TokenService.Domain.Ports;
using TokenService.Infrastructure.Auth;

namespace TokenService.Test.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly AuthService _authService;
        
        public AuthServiceTests()
        {
            _authService = new AuthService(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _passwordHasherMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_RetunrsToken()
        {
            var user = new User(1, "test@example.com", "hashed_password");

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(hasher => hasher.VerifyPassword("valid_password", user.PasswordHash))
                .Returns(true);

            var Token_Esperado = new Token("access_token", "refresh_token", DateTime.UtcNow.AddMinutes(15));

            _tokenServiceMock
                .Setup(TokenService => TokenService.GenerateToken(user))
                .Returns(Token_Esperado);

            _tokenServiceMock
                .Setup(tokenService => tokenService.GenerateRefreshToken())
                .Returns(("mock_refresh_token", DateTime.UtcNow.AddDays(7)));

            var result = await _authService.LoginAsync(new LoginRequestDto
            {
                Email = user.Email,
                Password = "valid_password"
            });

            Assert.NotNull(result);
            Assert.Equal(Token_Esperado.AccessToken, result.AccessToken);
            Assert.Equal(Token_Esperado.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ReturnsNull()
        {
            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync("invalid@example.com"))
                .ReturnsAsync((User?)null);

            var result = await _authService.LoginAsync(new LoginRequestDto
            {
                Email = "invalid@example.com",
                Password = "password_invalid"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_RetunrsNull()
        {
            var user = new User(1, "test@example.com","hashed_password");

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(hasher => hasher.VerifyPassword("wrong_password", user.PasswordHash))
                .Returns(false);

            var result = await _authService.LoginAsync(new LoginRequestDto
            {
                Email = user.Email,
                Password = "wrong_password"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewToken()
        {
            var user = new User(1, "test@example.com", "hashed_password");
            user.SetRefreshToken("valid_refresh_token", DateTime.UtcNow.AddDays(7));

            _tokenServiceMock
                .Setup(service => service.GetUserIdFromToken("valid_access_token"))
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            var newToken = new Token("new_access_token", "new_refresh_token", DateTime.UtcNow.AddMinutes(15));

            _tokenServiceMock
                .Setup(service => service.GenerateToken(user))
                .Returns(newToken);

            _tokenServiceMock
                .Setup(service => service.GenerateRefreshToken())
                .Returns(("new_refresh_token", DateTime.UtcNow.AddDays(7)));

            var result = await _authService.RefreshTokenAsync(new RefreshTokenRequestDto
            {
                AccessToken = "valid_access_token",
                RefreshToken = "valid_refresh_token"
            });

            Assert.NotNull(result);
            Assert.Equal(newToken.AccessToken, result.AccessToken);

            _userRepositoryMock.Verify(
                repo => repo.UpdateRefreshTokenAsync(
                    user.Id,
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()),
                Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_InvalidToken_ReturnsNull()
        {
            _tokenServiceMock
                .Setup(service => service.GetUserIdFromToken("invalid_token"))
                .Returns((int?)null);

            var result = await _authService.RefreshTokenAsync(new RefreshTokenRequestDto
            {
                AccessToken = "invalid_token",
                RefreshToken = "any_refresh_token"
            });

            Assert.Null(result);

            _userRepositoryMock.Verify(
                repo => repo.UpdateRefreshTokenAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }
    }
}

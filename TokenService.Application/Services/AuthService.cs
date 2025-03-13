using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TokenService.Application.DTOs;
using TokenService.Domain.Ports;

namespace TokenService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash)) return null;

            var token = _tokenService.GenerateToken(user);

            var (refreshToken, expiryTime) = _tokenService.GenerateRefreshToken();

            user.SetRefreshToken(refreshToken, expiryTime);

            await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, expiryTime);

            return new TokenResponseDto
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                ExpirationAt = token.ExpiresAt
            };
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var userId = _tokenService.GetUserIdFromToken(request.AccessToken);

            if (!userId.HasValue) return null;

            var user = await _userRepository.GetByIdAsync(userId.Value);

            if (user == null || !user.IsValidRefreshToken(request.RefreshToken)) return null;

            var newToken = _tokenService.GenerateToken(user);

            var (newRefreshToken, expiryTime) = _tokenService.GenerateRefreshToken();

            user.SetRefreshToken(newRefreshToken, expiryTime);

            await _userRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, expiryTime);

            return new TokenResponseDto
            {
                AccessToken = newToken.AccessToken,
                RefreshToken = newToken.RefreshToken,
                ExpirationAt = newToken.ExpiresAt
            };
        }

        public bool ValidateToken(string token)
        {
            return _tokenService.ValidateToken(token);
        }
    }
}

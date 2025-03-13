using BBF.Api.DTOs;

namespace BBF.Api.Services.Interfaces
{
    public interface ITokenServiceClient
    {
        Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<bool> ValidateTokenAsync(string token);
    }
}

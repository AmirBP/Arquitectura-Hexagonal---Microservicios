using BBF.Api.DTOs;

namespace BBF.Api.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto?> GetUserProfileAsync(int userId, string token);
        Task<UserListDto> GetAllUserProfilesAsync(int page, string token);
    }
}

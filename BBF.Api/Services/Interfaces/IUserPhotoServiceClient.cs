using BBF.Api.DTOs;

namespace BBF.Api.Services.Interfaces
{
    public interface IUserPhotoServiceClient
    {
        Task<UserPhotoDto?> GetUserPhotoAsync(int userId, string token);
        Task<IEnumerable<UserPhotoDto>> GetAllUserPhotosAsync(int page, string token);
    }
}

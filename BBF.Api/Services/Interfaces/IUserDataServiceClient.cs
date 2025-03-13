using BBF.Api.DTOs;

namespace BBF.Api.Services.Interfaces
{
    public interface IUserDataServiceClient
    {
        Task<UserDataDto?> GetUserByIdAsync(int id, string token);
        Task<IEnumerable<UserDataDto>> GetAllUsersAsync(int page, string token);
    }
}

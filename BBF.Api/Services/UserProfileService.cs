using BBF.Api.DTOs;
using BBF.Api.Services.Interfaces;

namespace BBF.Api.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserDataServiceClient _userDataServiceClient;
        private readonly IUserPhotoServiceClient _userPhotoServiceClient;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(IUserDataServiceClient userDataServiceClient, IUserPhotoServiceClient userPhotoServiceClient, ILogger<UserProfileService> logger)
        {
            _userDataServiceClient = userDataServiceClient;
            _userPhotoServiceClient = userPhotoServiceClient;
            _logger = logger;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId, string token)
        {
            try
            {
                var userDataTask = _userDataServiceClient.GetUserByIdAsync(userId, token);
                var userPhotoTask = _userPhotoServiceClient.GetUserPhotoAsync(userId, token);

                await Task.WhenAll(userDataTask, userPhotoTask);

                var userData = await userDataTask;
                var userPhoto = await userPhotoTask;

                if(userData == null)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado", userId);
                    return null;
                }

                return new UserProfileDto
                {
                    Id = userData.Id,
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                    PhotoBase64 = userPhoto?.PhotoBase64 ?? string.Empty
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil de usuario para el ID {UserId}", userId);
                return null;
            }
        }

        public async Task<UserListDto> GetAllUserProfilesAsync(int page, string token)
        {
            try
            {
                var userDataTask = _userDataServiceClient.GetAllUsersAsync(page, token);
                var userPhotosTask = _userPhotoServiceClient.GetAllUserPhotosAsync(page, token);

                await Task.WhenAll(userDataTask, userPhotosTask);

                var usersData = (await userDataTask).ToList();
                var userPhotos = (await userPhotosTask).ToDictionary(p => p.UserId);

                var userProfiles = new List<UserProfileDto>();

                foreach (var userData in usersData)
                {
                    userProfiles.Add(new UserProfileDto
                    {
                        Id = userData.Id,
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        PhotoBase64 = userPhotos.TryGetValue(userData.Id, out var photo) ? photo.PhotoBase64 : string.Empty
                    });
                }

                return new UserListDto
                {
                    Page = page,
                    TotalPages = 2,
                    Users = userProfiles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfiles de usuario para la página {Page}", page);
                return new UserListDto { Page = page, TotalPages = 0, Users = new List<UserProfileDto>() };
            }
        }
    }
}

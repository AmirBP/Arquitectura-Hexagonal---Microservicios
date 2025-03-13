using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserPhotoService.Domain.Entities;
using UserPhotoService.Domain.Ports;

namespace UserPhotoService.Application.Services
{
    public class UserPhotoService : IUserPhotoService
    {
        private readonly IUserPhotoRepository _userPhotoRepository;
        private readonly ILogger<UserPhotoService> _logger;

        public UserPhotoService(IUserPhotoRepository userPhotoRepository, ILogger<UserPhotoService> logger)
        {
            _userPhotoRepository = userPhotoRepository;
            _logger = logger;
        }

        public async Task<UserPhoto?> GetUserPhotoAsync(int userId)
        {
            _logger.LogInformation("Obteniendo foto de usuario con ID {iD}", userId);
            return await _userPhotoRepository.GetUserPhotoAsync(userId);
        }

        public Task<IEnumerable<UserPhoto>> GetAllUserPhotosAsync(int page = 1)
        {
            _logger.LogInformation("Obteniendo todas las fotos de usuario, página: {Page}", page);
            return _userPhotoRepository.GetAllUserPhotosAsync(page);
        }

    }
}

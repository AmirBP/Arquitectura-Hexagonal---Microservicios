using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserDataService.Domain.Entities;
using UserDataService.Domain.Ports;

namespace UserDataService.Application.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly IUserDataRepository _userDataRepository;
        private readonly ILogger<UserDataService> _logger;

        public UserDataService(IUserDataRepository userDataRepository, ILogger<UserDataService> logger)
        {
            _userDataRepository = userDataRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserData>> GetAllUsersAsync(int page = 1)
        {
            _logger.LogInformation("Obteniendo lista de usuarios, página : {Page}",page);

            return await _userDataRepository.GetAllUsersAsync(page);
        }

        public async Task<UserData?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Obteniendo usuario con ID : {Id}", id);

            return await _userDataRepository.GetUserByIdAsync(id);
        }
    }
}

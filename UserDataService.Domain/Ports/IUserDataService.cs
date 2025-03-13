using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDataService.Domain.Entities;

namespace UserDataService.Domain.Ports
{
    public interface IUserDataService
    {
        Task<UserData?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserData>> GetAllUsersAsync(int page = 1);
    }
}

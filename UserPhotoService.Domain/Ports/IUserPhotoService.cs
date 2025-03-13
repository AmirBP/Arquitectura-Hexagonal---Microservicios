using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserPhotoService.Domain.Entities;

namespace UserPhotoService.Domain.Ports
{
    public interface IUserPhotoService
    {
        Task<UserPhoto?> GetUserPhotoAsync(int userId);
        Task<IEnumerable<UserPhoto>> GetAllUserPhotosAsync(int page = 1);
    }
}

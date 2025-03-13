using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TokenService.Domain.Entities;

namespace TokenService.Domain.Ports
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime refreshTokenExpiryTime);
    }
}

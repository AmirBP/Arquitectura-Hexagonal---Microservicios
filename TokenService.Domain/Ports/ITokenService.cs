using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenService.Domain.Entities;

namespace TokenService.Domain.Ports
{
    public interface ITokenService
    {
        Token GenerateToken(User user);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        (string refreshToken, DateTime expiryTime) GenerateRefreshToken();
    }
}

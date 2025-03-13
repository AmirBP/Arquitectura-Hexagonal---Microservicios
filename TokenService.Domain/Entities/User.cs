using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string RefreshToken {  get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        public User(int id, string email, string passwordHash) 
        {
            Id = id;
            Email = email?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        }

        public void SetRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshToken = refreshToken?? throw new ArgumentNullException(nameof(refreshToken));
            RefreshTokenExpiryTime = expiryTime;
        }

        public bool IsValidRefreshToken(string refreshToken)
        {
            return  RefreshToken == refreshToken &&
                    RefreshTokenExpiryTime.HasValue &&
                    DateTime.UtcNow <= RefreshTokenExpiryTime.Value;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService.Domain.Entities
{
    public class Token
    {
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public Token(string accessToken, string refreshToken, DateTime expiresAt)
        {
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));

            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));

            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresAt = expiresAt;
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    }
}

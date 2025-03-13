using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService.Infrastructure.Auth
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
}

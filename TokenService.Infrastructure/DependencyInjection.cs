using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TokenService.Application.Services;
using TokenService.Domain.Entities;
using TokenService.Domain.Ports;
using TokenService.Infrastructure.Auth;
using TokenService.Infrastructure.Repositories;
using TokenService.Infrastructure.Security;
using System.Net.Http;

namespace TokenService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddSingleton<ITokenService, JwtTokenService>();
            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            services.AddHttpClient<IUserRepository, UserRepository>();

            return services;
        }
    }
}

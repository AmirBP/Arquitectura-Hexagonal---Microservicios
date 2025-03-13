using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserDataService.Domain.Ports;
using UserDataService.Infrastructure.Repositories;

namespace UserDataService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient<IUserDataRepository, UserDataRepository>();
            services.AddMemoryCache();
            return services;
        }
    }
}

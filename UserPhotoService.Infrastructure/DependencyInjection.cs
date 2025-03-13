using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UserPhotoService.Domain.Ports;
using UserPhotoService.Infrastructure.Repositories;

namespace UserPhotoService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient<IUserPhotoRepository, UserPhotoRepository>();
            services.AddMemoryCache();

            return services;
        }
    }
}

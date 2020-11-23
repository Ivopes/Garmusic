using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Repositories;
using Garmusic.Services.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Services
{
    public static class DependencyInjectionCollection
    {
        public static IServiceCollection AddDependency(this IServiceCollection services)
        {
            services.AddTransient<ISongService, SongService>();
            services.AddTransient<ISongRepository, SongRepository>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IMigrationService, MigrationService>();
            services.AddTransient<IMigrationRepository, MigrationRepository>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IAuthRepository, AuthRepository>();
            services.AddTransient<IPlaylistService, PlaylistService>();
            services.AddTransient<IPlaylistRepository, PlaylistRepository>();

            return services;
        }
    }
}

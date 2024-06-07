using BZGames.Application.Interfaces.Services;
using BZGames.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BZGames.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}

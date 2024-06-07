using BZGames.Domain.Entities;
using BZGames.Domain.Interfaces;
using BZGames.Application.Interfaces.Services;
using BZGames.Infrastructure.Persistence.Data;
using BZGames.Infrastructure.Security.Token;
using BZGames.Infrastructure.Services;
using BZGames.Infrastructure.Repositories;
using BZGames.Infrastructure.Common.Interfaces;
using BZGames.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace BZGames.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddIdentity<User, IdentityRole<Guid>>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TH3@B357!53CUR17Y--K3Y$%3VERM4DEF0RSUR3")),
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                };

                // Configure the Authority to the expected value for
                // the authentication provider. This ensures the token
                // is appropriately validated.
                //options.Authority = "Authority URL"; // TODO: Update URL

                options.Events = new JwtBearerEvents
                {

                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/rpsGame") || (path.StartsWithSegments("/c4Game") || (path.StartsWithSegments("/tttGame")))))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            var connectionString = configuration.GetConnectionString("SqliteConnectionString");

            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseLazyLoadingProxies().UseSqlite(connectionString,
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });


            services.AddScoped<IUserRepository, UserRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserContextProvider, UserContextProvider>();


            return services;
        }
    }

}

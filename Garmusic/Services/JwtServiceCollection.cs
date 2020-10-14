using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Garmusic.Services
{
    public static class JwtServiceCollection
    {
        public static IServiceCollection ConfigureJwt(this IServiceCollection services)
        {
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                           .AddJwtBearer(options =>
                           {
                               options.TokenValidationParameters = new TokenValidationParameters
                               {
                                   ValidateIssuer = true,
                                   ValidateAudience = true,
                                   ValidateLifetime = true,
                                   ValidateIssuerSigningKey = true,
                                   ValidIssuer = "http://localhost:5000",
                                   ValidAudience = "http://localhost:5000",
                                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey&&12345"))
                               };
                           }
                        );

            return services;
        }
    }
}

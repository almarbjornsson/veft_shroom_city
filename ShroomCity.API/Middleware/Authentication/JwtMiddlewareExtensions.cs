using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShroomCity.Repositories.Interfaces;

namespace ShroomCity.API.Middleware.Authentication;

public static class JwtMiddlewareExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var signingKey = configuration.GetValue<string>("TokenAuthentication:Secret");
        if (string.IsNullOrEmpty(signingKey))
        {
            throw new InvalidOperationException("The signing key is null or empty!");
        }

        
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // Remember to set this to true in production
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = configuration.GetValue<string>("TokenAuthentication:Audience"),
                    ValidIssuer = configuration.GetValue<string>("TokenAuthentication:Issuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        if (context.SecurityToken is JwtSecurityToken token)
                        {
                            // Access your TokenRepository here and check if the token is blacklisted.
                            var tokenRepository = context.HttpContext.RequestServices.GetRequiredService<ITokenRepository>();
                            
                            // Get TokenId from claims
                            var tokenId = token.Claims.FirstOrDefault(c => c.Type == "TokenId")?.Value;
                            
                            var tokenIdInt = int.Parse(tokenId ?? throw new InvalidOperationException("TokenId claim is missing."));
                            
                            if (await tokenRepository.IsTokenBlacklisted(tokenIdInt))
                            {
                                context.Fail("This token has been blacklisted.");
                            }
                        }
                    }
                };
            });

        return services;
    }
}
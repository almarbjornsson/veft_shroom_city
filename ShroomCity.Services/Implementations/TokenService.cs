using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShroomCity.Models.Dtos;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class JwtConfiguration
{
    /// <summary>
    /// The secret used to sign the JWT token
    /// </summary>
    public string Secret { get; set; } = "";
    /// <summary>
    /// Expiration in minutes for the JWT token.
    /// </summary>
    public string ExpirationInMinutes { get; set; } = "";
    /// <summary>
    /// The issuer of the JWT token. If the issuer is not a known enity, the JWT token should be rejected. In our
    /// example this API is the issuer - but that is not always the case.
    /// </summary>
    public string Issuer { get; set; } = "";
    /// <summary>
    /// The audience of the token. The services which are expected to receive and use the token. In our example
    /// this API is the audience - but that is not always the case.
    /// </summary>
    public string Audience { get; set; } = "";
}

public class TokenService : ITokenService
{
    
    private readonly JwtConfiguration _jwtConfiguration = new JwtConfiguration();
    
    private readonly ITokenRepository _tokenRepository;
    public TokenService(IConfiguration configuration, ITokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
        
        _jwtConfiguration.Audience = configuration.GetSection("TokenAuthentication").GetSection("Audience").Value;
        _jwtConfiguration.ExpirationInMinutes = configuration.GetSection("TokenAuthentication").GetSection("ExpirationInMinutes").Value;
        _jwtConfiguration.Issuer = configuration.GetSection("TokenAuthentication").GetSection("Issuer").Value;
        _jwtConfiguration.Secret = configuration.GetSection("TokenAuthentication").GetSection("SigningKey").Value;
    }

    public string GenerateJwtToken(UserDto user)
    {

        var principal = new ClaimsPrincipal();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.EmailAddress),
            new Claim("TokenId", user.TokenId.ToString()),
        };
        // Add all permissions as claims
        claims.AddRange(user.Permissions.Select(permission => new Claim("permissions", permission)));
        
        var claimsIdentity = new ClaimsIdentity(claims, "Token");
        
        principal.AddIdentity(claimsIdentity);
        
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        
        var token = new JwtSecurityToken(_jwtConfiguration.Issuer,
            _jwtConfiguration.Audience,
            principal.Claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtConfiguration.ExpirationInMinutes)),
            signingCredentials: credentials);
        

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task BlacklistToken(int tokenId)
    {
        return _tokenRepository.BlacklistToken(tokenId);
    }

    public Task<bool> IsTokenBlacklisted(int tokenId)
    {
        return _tokenRepository.IsTokenBlacklisted(tokenId);
    }
}
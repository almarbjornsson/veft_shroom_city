using ShroomCity.Models.Dtos;
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
    public string GenerateJwtToken(UserDto user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenBlacklisted(int tokenId)
    {
        throw new NotImplementedException();
    }
}
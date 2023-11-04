using ShroomCity.Models.Dtos;

namespace ShroomCity.Services.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(UserDto user);
    Task BlacklistToken(int tokenId);
    Task<bool> IsTokenBlacklisted(int tokenId);
}
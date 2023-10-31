using ShroomCity.Models.Dtos;

namespace ShroomCity.Services.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(UserDto user);
    Task<bool> IsTokenBlacklisted(int tokenId);
}
namespace ShroomCity.Repositories.Interfaces;

public interface ITokenRepository
{
    Task<int> CreateToken();
    Task BlacklistToken(int tokenId);
    Task<bool> IsTokenBlacklisted(int tokenId);
}
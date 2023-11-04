using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Entities;
using ShroomCity.Models.Exceptions;
using ShroomCity.Repositories.Interfaces;

namespace ShroomCity.Repositories.Implementations;

public class TokenRepository : ITokenRepository
{
    private readonly ShroomCityDbContext _context;
    
    public TokenRepository(ShroomCityDbContext context)
    {
        _context = context;
    }
    
    public async Task BlacklistToken(int tokenId)
    {
        var token = await _context.JwtTokens.FirstOrDefaultAsync(t => t.Id == tokenId);
        if (token is null)
        {
            throw new TokenNotFoundException($"Token with id {tokenId} not found");
        }
        token.Blacklisted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<int> CreateToken()
    {
        var token = new JwtToken
        {
            Blacklisted = false
        };
        await _context.JwtTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        
        return token.Id;
    }

    public async Task<bool> IsTokenBlacklisted(int tokenId)
    {
        var token = await _context.JwtTokens.FirstOrDefaultAsync(t => t.Id == tokenId);
        if (token is null)
        {
            throw new TokenNotFoundException($"Token with id {tokenId} not found");
        }
        return token.Blacklisted;
    }
}
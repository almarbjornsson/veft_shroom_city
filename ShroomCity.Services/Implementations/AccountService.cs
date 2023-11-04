using ShroomCity.Models.Dtos;
using ShroomCity.Models.Exceptions;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Interfaces;
using ShroomCity.Utilities.Hasher;

namespace ShroomCity.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITokenService _tokenService;
    
    public AccountService(IAccountRepository accountRepository, ITokenService tokenService)
    {
        _accountRepository = accountRepository;
        _tokenService = tokenService;
    }
    public Task<UserDto?> Register(RegisterInputModel inputModel)
    {
        return _accountRepository.Register(inputModel);
    }

    public async Task<UserDto?> SignIn(LoginInputModel inputModel)
    {
        var userDto = await _accountRepository.SignIn(inputModel);
        
        return userDto;
    }

    public Task SignOut(int tokenId)
    {
        // We need to blacklist the token
        return _tokenService.BlacklistToken(tokenId);
    }
}
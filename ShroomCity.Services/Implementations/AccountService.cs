using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class AccountService : IAccountService
{
    public Task<UserDto?> Register(RegisterInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> SignIn(LoginInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public Task SignOut(int tokenId)
    {
        throw new NotImplementedException();
    }
}
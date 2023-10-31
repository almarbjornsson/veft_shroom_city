using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;

namespace ShroomCity.Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    public Task<UserDto?> Register(RegisterInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> SignIn(LoginInputModel inputModel)
    {
        throw new NotImplementedException();
    }
}
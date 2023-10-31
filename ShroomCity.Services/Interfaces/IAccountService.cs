using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;

namespace ShroomCity.Services.Interfaces;

public interface IAccountService
{
    Task<UserDto?> Register(RegisterInputModel inputModel);
    Task<UserDto?> SignIn(LoginInputModel inputModel);
    Task SignOut(int tokenId);
}
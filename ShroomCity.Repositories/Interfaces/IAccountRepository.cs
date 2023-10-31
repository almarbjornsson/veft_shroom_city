using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;

namespace ShroomCity.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<UserDto?> Register(RegisterInputModel inputModel);
    Task<UserDto?> SignIn(LoginInputModel inputModel);
}
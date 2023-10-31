using ShroomCity.Models;
using ShroomCity.Models.Dtos;

namespace ShroomCity.Services.Interfaces;

public interface IExternalMushroomService
{
    Task<Envelope<ExternalMushroomDto>?> GetMushrooms(int pageSize, int pageNumber);
    Task<ExternalMushroomDto?> GetMushroomByName(string name);
}
using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class ExternalMushroomService : IExternalMushroomService
{
    public Task<ExternalMushroomDto?> GetMushroomByName(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Envelope<ExternalMushroomDto>?> GetMushrooms(int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }
}
using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class MushroomService : IMushroomService
{
    public Task<int> CreateMushroom(string researcherEmailAddress, MushroomInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteMushroomById(int mushroomId)
    {
        throw new NotImplementedException();
    }

    public Task<Envelope<MushroomDto>?> GetLookupMushrooms(int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task<MushroomDetailsDto?> GetMushroomById(int id)
    {
        throw new NotImplementedException();
    }

    public Envelope<MushroomDto>? GetMushrooms(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel, bool performLookup)
    {
        throw new NotImplementedException();
    }
}
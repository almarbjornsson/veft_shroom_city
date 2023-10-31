using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;

namespace ShroomCity.Repositories.Implementations;

public class MushroomRepository : IMushroomRepository
{
    public Task<int> CreateMushroom(MushroomInputModel mushroom, string researcherEmailAddress, List<AttributeDto> attributes)
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

    public Task<MushroomDetailsDto?> GetMushroomById(int id)
    {
        throw new NotImplementedException();
    }

    public (int totalPages, IEnumerable<MushroomDto> mushrooms) GetMushroomsByCriteria(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel)
    {
        throw new NotImplementedException();
    }
}
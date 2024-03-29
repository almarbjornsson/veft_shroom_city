using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;

namespace ShroomCity.Repositories.Interfaces;

public interface IMushroomRepository
{
    Task<int> CreateMushroom(MushroomInputModel mushroom, string researcherEmailAddress, List<AttributeDto> attributes);
    Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel);
    Task<bool> DeleteMushroomById(int mushroomId);
    Task<MushroomDetailsDto?> GetMushroomById(int id);
    (int totalPages, IEnumerable<MushroomDto> mushrooms) GetMushroomsByCriteria(GetMushroomsInputModel inputModel);
    Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel);
}
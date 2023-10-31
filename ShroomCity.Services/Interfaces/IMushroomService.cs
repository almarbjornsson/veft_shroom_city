using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;

namespace ShroomCity.Services.Interfaces;

public interface IMushroomService
{
    Task<int> CreateMushroom(string researcherEmailAddress, MushroomInputModel inputModel);
    Envelope<MushroomDto>? GetMushrooms(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber);
    Task<MushroomDetailsDto?> GetMushroomById(int id);
    Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel, bool performLookup);
    Task<bool> DeleteMushroomById(int mushroomId);
    Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel);
    Task<Envelope<MushroomDto>?> GetLookupMushrooms(int pageSize, int pageNumber);
}
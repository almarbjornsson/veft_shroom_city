using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;

namespace ShroomCity.Repositories.Interfaces;

public interface IResearcherRepository
{
    Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel);
    Task<IEnumerable<ResearcherDto>?> GetAllResearchers();
    Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress);
    Task<ResearcherDto?> GetResearcherById(int id);
}

using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;

namespace ShroomCity.Services.Interfaces
{
    public interface IResearcherService
    {
        Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel);
        Task<IEnumerable<ResearcherDto>?> GetAllResearchers();
        Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress);
        Task<ResearcherDto?> GetResearcherById(int id);
    }
}
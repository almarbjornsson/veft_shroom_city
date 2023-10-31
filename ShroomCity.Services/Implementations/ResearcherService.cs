using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class ResearcherService : IResearcherService
{
    public Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ResearcherDto>?> GetAllResearchers()
    {
        throw new NotImplementedException();
    }

    public Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress)
    {
        throw new NotImplementedException();
    }

    public Task<ResearcherDto?> GetResearcherById(int id)
    {
        throw new NotImplementedException();
    }
}
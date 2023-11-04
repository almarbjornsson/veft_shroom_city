using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class ResearcherService : IResearcherService
{
    private readonly IResearcherRepository _researcherRepository;
    
    public ResearcherService(IResearcherRepository researcherRepository)
    {
        _researcherRepository = researcherRepository;
    }
    
    public Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel)
    {
        return _researcherRepository.CreateResearcher(createdBy, inputModel);
    }

    public Task<IEnumerable<ResearcherDto>?> GetAllResearchers()
    {
        return _researcherRepository.GetAllResearchers();
    }

    public Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress)
    {
        return _researcherRepository.GetResearcherByEmailAddress(emailAddress);
    }

    public Task<ResearcherDto?> GetResearcherById(int id)
    {
        return _researcherRepository.GetResearcherById(id);
    }
}
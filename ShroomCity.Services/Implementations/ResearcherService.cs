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
        // TODO: Check permissions at service layer in addition to policy checks at controller layer.
        // // Policy check in endpoint should have validated that the user is allowed to create a researcher.
        //
        // // But defense in depth is a good thing, so we'll check again.
        // var potentialCreator = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress == createdBy);
        // if (potentialCreator is null)
        // {
        //     throw new UserNotFoundException($"User with email address {createdBy} not found");
        // }
        // var creatorRole = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Code == "write:researchers");
        //
        // if (creatorRole is null)
        // {
        //     throw new UserNotAuthorizedException("write:researchers permission not found");
        // }

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
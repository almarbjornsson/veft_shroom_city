using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Constants;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Entities;
using ShroomCity.Models.Exceptions;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;

namespace ShroomCity.Repositories.Implementations;

public class ResearcherRepository : IResearcherRepository
{
    private readonly ShroomCityDbContext _dbContext;

    public ResearcherRepository(ShroomCityDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel)
    {

        // Add the researcher role to the user.
        var user = await _dbContext.Users
            .Where(u => u.EmailAddress == inputModel.EmailAddress)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync();
        if (user is null)
        {
            throw new UserNotFoundException($"User with email address {inputModel.EmailAddress} not found");
        }
        // Check if the user already has the role Researcher
        if (user.Roles.Any(r => r.Name == RoleConstants.Researcher))
        {
            throw new UserAlreadyHasRoleException("User already has the role Researcher");
        }
        
        
        var researcherRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.Researcher);
        if (researcherRole is null)
        {
            throw new RoleNotFoundException("Researcher role not found");
        }
        
        
        user.Roles.Add(researcherRole);
        await _dbContext.SaveChangesAsync();
        return user.Id;
    }


    public async Task<IEnumerable<ResearcherDto>?> GetAllResearchers()
    {
        // Everyone with the role Researcher and Admin
        var researchers = await _dbContext.Users
            .Where(u => u.Roles.Any(r => r.Name == RoleConstants.Researcher || r.Name == RoleConstants.Admin))
            .ToListAsync();

        var researcherDtos = new List<ResearcherDto>();
        
        foreach (var researcher in researchers)
        {
            var researcherDto = await PopulateAssociatedMushrooms(researcher);
            researcherDtos.Add(researcherDto);
        }

        return researcherDtos;
    }

    public async Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress)
    {
        var researcher = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);
        
        if (researcher is null)
        {
            return null;
        }
        return await GetResearcherById(researcher.Id);
    }

    public async Task<ResearcherDto?> GetResearcherById(int id)
    {
        // Everyone with the role Researcher and Admin
        var researcher = await _dbContext.Users
            .Where(u => u.Roles.Any(r => r.Name == RoleConstants.Researcher || r.Name == RoleConstants.Admin))
            .FirstOrDefaultAsync(u => u.Id == id);

        if (researcher is null)
        {
            return null;
        }
        
        var researcherDto = await PopulateAssociatedMushrooms(researcher);
        
        return researcherDto;
    }

    private async Task<ResearcherDto> PopulateAssociatedMushrooms(User researcher)
    {
        var mushrooms = await _dbContext.Mushrooms.Where(m => m.Attributes.Any(a => a.RegisteredById == researcher.Id))
            .Select(m => new MushroomDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
            })
            .ToListAsync();

        var researcherDto = new ResearcherDto
        {
            Id = researcher.Id,
            EmailAddress = researcher.EmailAddress,
            Name = researcher.Name,
            Bio = researcher.Bio,
            AssociatedMushrooms = mushrooms,
        };
        return researcherDto;
    }
}


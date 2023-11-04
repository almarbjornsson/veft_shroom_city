using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Dtos;
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
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress);
        if (user is null)
        {
            throw new UserNotFoundException($"User with email address {inputModel.EmailAddress} not found");
        }
        var researcherRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Researcher");
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
            .Where(u => u.Roles.Any(r => r.Name == "Researcher" || r.Name == "Admin"))
            .Select(u => new
            {
                User = u,
                Mushrooms = _dbContext.Attributes
                    .Where(a => a.RegisteredById == u.Id)
                    .SelectMany(a => a.Mushrooms)
                    .Select(m => new MushroomDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                    }).ToList()
            })
            .ToListAsync();

        return researchers.Select(r => new ResearcherDto
        {
            Id = r.User.Id,
            EmailAddress = r.User.EmailAddress,
            Name = r.User.Name,
            Bio = r.User.Bio,
            AssociatedMushrooms = r.Mushrooms,
        }).ToList();
    }

    public Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress)
    {
        var researcher = _dbContext.Users
            .Where(u => u.EmailAddress == emailAddress)
            .Select(u => new ResearcherDto
            {
                Id = u.Id,
                EmailAddress = u.EmailAddress,
                Name = u.Name,
                Bio = u.Bio,
                AssociatedMushrooms = _dbContext.Attributes
                    .Where(a => a.RegisteredById == u.Id)
                    .SelectMany(a => a.Mushrooms)
                    .Select(m => new MushroomDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                    }).ToList()
            })
            .FirstOrDefaultAsync();
        return researcher;
    }

    public Task<ResearcherDto?> GetResearcherById(int id)
    {
        var researcher = _dbContext.Users
            .Where(u => u.Id == id)
            .Select(u => new ResearcherDto
            {
                Id = u.Id,
                EmailAddress = u.EmailAddress,
                Name = u.Name,
                Bio = u.Bio,
                AssociatedMushrooms = _dbContext.Attributes
                    .Where(a => a.RegisteredById == u.Id)
                    .SelectMany(a => a.Mushrooms)
                    .Select(m => new MushroomDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                    }).ToList()
            })
            .FirstOrDefaultAsync();
        return researcher;
    }
}


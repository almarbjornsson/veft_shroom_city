using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Constants;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Entities;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using Attribute = ShroomCity.Models.Entities.Attribute;

namespace ShroomCity.Repositories.Implementations;

public class MushroomRepository : IMushroomRepository
{
    private readonly ShroomCityDbContext _dbContext;
    private readonly IResearcherRepository _researcherRepository;
    public MushroomRepository(ShroomCityDbContext dbContext, IResearcherRepository researcherRepository)
    {
        _dbContext = dbContext;
        _researcherRepository = researcherRepository;
    }
    
    
    public async Task<int> CreateMushroom(MushroomInputModel mushroom, string researcherEmailAddress, List<AttributeDto> attributeDtos)
    {
        // Get the researcher, we need the id
        var researcher = await _researcherRepository.GetResearcherByEmailAddress(researcherEmailAddress);
        if (researcher == null)
        {
            throw new KeyNotFoundException("Researcher not found with the provided email address.");
        }

        // Retrieve all attributes at once by their Ids
        var attributeIds = attributeDtos.Select(a => a.Id).ToList();
        var associatedAttributes = await _dbContext.Attributes
            .Where(a => attributeIds.Contains(a.Id))
            .ToListAsync();

        // Create new Mushroom entity
        var mushroomEntity = new Mushroom
        {
            Name = mushroom.Name,
            Description = mushroom.Description,
            Attributes = associatedAttributes
        };

        // Add to context and save
        _dbContext.Mushrooms.Add(mushroomEntity);
        await _dbContext.SaveChangesAsync();

        return mushroomEntity.Id; // Return the new mushroom's Id
    }

    public async Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        // Validate the researcher
        var researcher = await _dbContext.Users
            .Include(u => u.Roles) 
            .FirstOrDefaultAsync(r =>
                r.EmailAddress == researcherEmailAddress &&
                r.Roles.Any(role => role.Name == RoleConstants.Researcher)); 

        if (researcher == null)
        {
            return false; // Researcher not found or does not have the Researcher role
        }


        // Validate the mushroom
        var mushroom = await _dbContext.Mushrooms
            .Include(m => m.Attributes)
            .ThenInclude(attribute =>
                attribute.AttributeType) 
            .FirstOrDefaultAsync(m => m.Id == mushroomId);
        if (mushroom == null)
        {
            return false; // Mushroom not found
        }

        // Validate and process each entry
        foreach (var entry in inputModel.Entries)
        {
            // Check if the attribute type exists
            var attributeType = await _dbContext.AttributeTypes
                .FirstOrDefaultAsync(at => at.Type == entry.Key);
            if (attributeType == null)
            {
                return false; // Attribute type not found
            }
        
      
            // Create a new attribute with the given type and value
            mushroom.Attributes.Add(new Attribute
            {
                AttributeType = attributeType,
                AttributeTypeId = attributeType.Id,
                Value = entry.Value,
                RegisteredBy = researcher,
                RegisteredById = researcher.Id,
            });
        }

        await _dbContext.SaveChangesAsync();

        return true;
    }


    public async Task<bool> DeleteMushroomById(int mushroomId)
    {
        var mushroom = await _dbContext.Mushrooms
            .FirstOrDefaultAsync(m => m.Id == mushroomId);

        if (mushroom == null)
        {
            return false;
        }
        
        _dbContext.Mushrooms.Remove(mushroom);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<MushroomDetailsDto?> GetMushroomById(int id)
    {
        var mushroom = await _dbContext.Mushrooms
            .Include(m => m.Attributes).ThenInclude(attribute => attribute.RegisteredBy)
            .Include(mushroom => mushroom.Attributes).ThenInclude(attribute => attribute.AttributeType)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (mushroom == null)
        {
            return null;
        }

        var mushroomDetailsDto = new MushroomDetailsDto
        {
            Id = mushroom.Id,
            Name = mushroom.Name,
            Description = mushroom.Description,
            Attributes = mushroom.Attributes.Select(a => new AttributeDto
            {
                Id = a.Id,
                Value = a.Value,
                Type = a.AttributeType.Type,
                RegisteredBy = a.RegisteredBy.EmailAddress,
                RegistrationDate = a.RegistrationDate,
            }).ToList()
        };

        return mushroomDetailsDto;

    }

    public (int totalPages, IEnumerable<MushroomDto> mushrooms) GetMushroomsByCriteria(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        var mushrooms = _dbContext.Mushrooms
            .Include(m => m.Attributes)
            .ThenInclude(attribute => attribute.AttributeType)
            .Where(m =>
                (name == null || m.Name.Contains(name)) &&
                (stemSizeMinimum == null || m.Attributes.Any(a => a.AttributeType.Type == "StemSize" && int.Parse(a.Value) >= stemSizeMinimum)) &&
                (stemSizeMaximum == null || m.Attributes.Any(a => a.AttributeType.Type == "StemSize" && int.Parse(a.Value) <= stemSizeMaximum)) &&
                (capSizeMinimum == null || m.Attributes.Any(a => a.AttributeType.Type == "CapSize" && int.Parse(a.Value) >= capSizeMinimum)) &&
                (capSizeMaximum == null || m.Attributes.Any(a => a.AttributeType.Type == "CapSize" && int.Parse(a.Value) <= capSizeMaximum)) &&
                (color == null || m.Attributes.Any(a => a.AttributeType.Type == "Color" && a.Value == color))
            )
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(m => new MushroomDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description
            })
            .ToList();

        var totalPages = (int) Math.Ceiling((double) _dbContext.Mushrooms.Count() / pageSize);

        return (totalPages, mushrooms);
    }

    public async Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel)
    {
        var mushroom = await _dbContext.Mushrooms
            .FirstOrDefaultAsync(m => m.Id == mushroomId);

        if (mushroom == null)
            return false;
        
        mushroom.Name = inputModel.Name;
        
        if (inputModel.Description != null)
            mushroom.Description = inputModel.Description;

        await _dbContext.SaveChangesAsync();

        return true;
    }
}
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Constants;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Entities;
using ShroomCity.Models.Enums;
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
        
        // Create attributes
        var associatedAttributes = attributeDtos.Select(a => new Attribute
        {
            AttributeType = _dbContext.AttributeTypes.FirstOrDefault(at => at.Type == a.Type) ?? throw new InvalidOperationException(),
            Value = a.Value,
            RegisteredById = researcher.Id,
        }).ToList();
        
        
        // Create new Mushroom entity
        var mushroomEntity = new Mushroom
        {
            Name = mushroom.Name,
            Description = mushroom.Description,
            Attributes = associatedAttributes
        };

        // Add to context and save
        _dbContext.Attributes.AddRange(associatedAttributes);
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
                r.Roles.Any(role => role.Name == RoleConstants.Researcher || role.Name == RoleConstants.Admin)); 

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
        var mushroom = await _dbContext.Mushrooms.Include(mushroom => mushroom.Attributes)
            .FirstOrDefaultAsync(m => m.Id == mushroomId);

        if (mushroom == null)
        {
            return false;
        }
        // According to the requirements, we should also delete the associated attributes.
        // We do this even though the attributes could be associated with other mushrooms.
        
        // Delete the attributes
        _dbContext.Attributes.RemoveRange(mushroom.Attributes);
        
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

    public (int totalPages, IEnumerable<MushroomDto> mushrooms) GetMushroomsByCriteria(
    GetMushroomsInputModel inputModel)
{
    
    // The database design is flawed, because the value is a string, and cannot be calculated in the database.
    // We can't do the filtering in the database, because we need to filter on the average values.
    // So we need to fetch all the mushrooms, and then filter them.
    var mushrooms = _dbContext.Mushrooms
        .Include(m => m.Attributes)
        .ThenInclude(a => a.AttributeType)
        .Where(m => inputModel.Name == null || m.Name == inputModel.Name)
        .ToList();
    
    // There is no guarantee that the value is a number, so this filtering might blow up.
    
    // Filter on the average stem size
    if (inputModel.StemSizeMinimum != null || inputModel.StemSizeMaximum != null)
    {
        mushrooms = mushrooms.Where(m =>
        {
            var stemSizes = m.Attributes.Where(a => a.AttributeType.Type == AttributeTypeEnum.StemSize.ToString())
                .Select(a => int.Parse(a.Value))
                .ToList();
            
            // Mushroom will not be included if there are no stem size attributes
            if (!stemSizes.Any())
            {
                return false;
            }

            var stemSizeAvg = stemSizes.Average();
            return (inputModel.StemSizeMinimum == null || stemSizeAvg >= inputModel.StemSizeMinimum) &&
                   (inputModel.StemSizeMaximum == null || stemSizeAvg <= inputModel.StemSizeMaximum);
        }).ToList();
    }
    
    // Filter on the average cap size
    if (inputModel.CapSizeMinimum != null || inputModel.CapSizeMaximum != null)
    {
        mushrooms = mushrooms.Where(m =>
        {
            
            var capSizes = m.Attributes.Where(a => a.AttributeType.Type == AttributeTypeEnum.CapSize.ToString())
                .Select(a => int.Parse(a.Value))
                .ToList();
            // Mushroom will not be included if there are no cap size attributes
            if (!capSizes.Any())
            {
                return false;
            }

            var capSizeAvg = capSizes.Average();
            return (inputModel.CapSizeMinimum == null || capSizeAvg >= inputModel.CapSizeMinimum) &&
                   (inputModel.CapSizeMaximum == null || capSizeAvg <= inputModel.CapSizeMaximum);
        }).ToList();
    }


    // Filter on the color
    if (inputModel.Color != null)
    {
        mushrooms = mushrooms.Where(m =>
        {
            var colors = m.Attributes.Where(a => a.AttributeType.Type == AttributeTypeEnum.Color.ToString())
                .Select(a => a.Value)
                .ToList();
            // Mushroom will not be included if there are no color attributes
            if (!colors.Any())
            {
                return false;
            }

            return colors.Contains(inputModel.Color);
        }).ToList();
    }
    
    // Pagination
    mushrooms = mushrooms
        .Skip((inputModel.PageNumber - 1) * inputModel.PageSize)
        .Take(inputModel.PageSize)
        .ToList();
    
    var mushroomDtos = mushrooms.Select(m => new MushroomDto
    {
        Id = m.Id,
        Name = m.Name,
        Description = m.Description,
    });

    var totalPages = (int)Math.Ceiling((double)_dbContext.Mushrooms.Count() / inputModel.PageSize);

    return (totalPages, mushroomDtos);

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
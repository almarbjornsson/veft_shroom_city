using System.Net.Http.Json;
using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class MushroomService : IMushroomService
{
    private readonly IMushroomRepository _mushroomRepository;
    private readonly IExternalMushroomService _externalMushroomService;
    public MushroomService(IMushroomRepository mushroomRepository, IExternalMushroomService externalMushroomService)
    {
        _mushroomRepository = mushroomRepository;
        _externalMushroomService = externalMushroomService;
    }

    public async Task<int> CreateMushroom(string researcherEmailAddress, MushroomInputModel inputModel)
    {
        var externalMushroom = await _externalMushroomService.GetMushroomByName(inputModel.Name);
        if (externalMushroom == null)
        {
            throw new KeyNotFoundException("Mushroom not found with the provided name.");
        }
        
        // Add description
        inputModel.Description = externalMushroom.Description;
        
        // Add attributes
        var attributeDtos = new List<AttributeDto>();
        
        // Colors
        externalMushroom.Colors.ForEach(color => attributeDtos.Add(new AttributeDto
        {
            Type = "Color",
            Value = color,
            RegisteredBy = researcherEmailAddress,
        }));
        
        // Shapes
        externalMushroom.Shapes.ForEach(shape => attributeDtos.Add(new AttributeDto
        {
            Type = "Shape",
            Value = shape,
            RegisteredBy = researcherEmailAddress,
        }));
        
        // Surfaces
        externalMushroom.Surfaces.ForEach(surface => attributeDtos.Add(new AttributeDto
        {
            Type = "Surface",
            Value = surface,
            RegisteredBy = researcherEmailAddress,
        }));
        
        return await _mushroomRepository.CreateMushroom(inputModel, researcherEmailAddress, attributeDtos);        
    }

    public Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        return _mushroomRepository.CreateResearchEntry(mushroomId, researcherEmailAddress, inputModel);
    }

    public Task<bool> DeleteMushroomById(int mushroomId)
    {
        return _mushroomRepository.DeleteMushroomById(mushroomId);
    }

    public async Task<Envelope<MushroomDto>?> GetLookupMushrooms(int pageSize, int pageNumber)
    {
        var externalMushrooms = await _externalMushroomService.GetMushrooms(pageSize, pageNumber);
        
        if (externalMushrooms == null)
        {
            return null;
        }
        
        var mushrooms = externalMushrooms.Items.Select(externalMushroom => new MushroomDto
        {
            Name = externalMushroom.Name, 
            Description = externalMushroom.Description,
        }).ToList();

        return new Envelope<MushroomDto>
        {
            Items = mushrooms,
            PageNumber = externalMushrooms.PageNumber,
            PageSize = externalMushrooms.PageSize,
            TotalPages = externalMushrooms.TotalPages,
        };
    }

    public Task<MushroomDetailsDto?> GetMushroomById(int id)
    {
        return _mushroomRepository.GetMushroomById(id);
    }

    public Envelope<MushroomDto>? GetMushrooms(GetMushroomsInputModel inputModel)
    {
        var mushroomsByCriteria = _mushroomRepository.GetMushroomsByCriteria(inputModel);
        
        
        return new Envelope<MushroomDto>
        {
            Items = mushroomsByCriteria.mushrooms,
            PageNumber = inputModel.PageNumber,
            PageSize = inputModel.PageSize,
            TotalPages = mushroomsByCriteria.totalPages,
        };
    }

    public async Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel, bool performLookup)
    {
        if (performLookup)
        {
            var externalMushroom = await _externalMushroomService.GetMushroomByName(inputModel.Name);
        
            if (externalMushroom == null)
            {
                return false;
            }
            inputModel.Name = externalMushroom.Name;
            inputModel.Description = externalMushroom.Description; 
        }
        


        return await _mushroomRepository.UpdateMushroomById(mushroomId, inputModel);
    }
}
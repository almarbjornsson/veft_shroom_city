using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MushroomsController : ControllerBase
{
    private readonly IMushroomService _mushroomService;
    
    public MushroomsController(IMushroomService mushroomService)
    {
        _mushroomService = mushroomService;
    }



    [Authorize(Policy = "role:analyst")]
    [HttpGet]
    public ActionResult<Envelope<MushroomDto>> GetMushrooms([FromQuery] GetMushroomsInputModel inputModel)
    {
        
        var filteredMushrooms = _mushroomService.GetMushrooms(inputModel);
        
        if (filteredMushrooms == null)
        {
            return NotFound();
        }

        return filteredMushrooms;
    }
    
    
    [Authorize(Policy = "role:analyst")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MushroomDetailsDto>> GetMushroomById(int id)
    {
        var mushroom = await _mushroomService.GetMushroomById(id);
        
        if (mushroom == null)
        {
            return NotFound();
        }

        return Ok(mushroom);
    }
    
    
    [Authorize(Policy = "role:analyst")]
    [HttpGet("/api/Mushrooms/lookup")]
    public async Task<ActionResult<Envelope<MushroomDto>>> GetMushroomsFromExternalApi(
        [FromQuery] int pageSize = 25,
        [FromQuery] int pageNumber = 1)
    {
        var mushrooms = await _mushroomService.GetLookupMushrooms(pageSize, pageNumber);
        
        if (mushrooms == null)
        {
            return NotFound();
        }

        return Ok(mushrooms);
    }
    
    
    [Authorize(Policy = "role:researcher")]
    [HttpPost]
    public async Task<ActionResult<int>> CreateMushroom(MushroomInputModel mushroom)
    {
        var researcherEmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (researcherEmailAddress == null)
        {
            return Unauthorized();
        }
        
        var mushroomId = await _mushroomService.CreateMushroom(researcherEmailAddress, mushroom);
        return CreatedAtAction(nameof(GetMushroomById), new {id = mushroomId}, mushroomId);
    }
    
    
    [Authorize(Policy = "role:researcher")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateMushroom(int id, MushroomUpdateInputModel inputModel, [FromQuery] bool performLookup)
    {
        // Only check if the mushroom exists if we're not performing a lookup
        if (performLookup == false)
        {
            var mushroom = await _mushroomService.GetMushroomById(id);
            if (mushroom == null)
            {
                return NotFound();
            }
        }
        
        var isUpdated = await _mushroomService.UpdateMushroomById(id, inputModel, performLookup);
        if (!isUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    
    [Authorize(Policy = "role:researcher")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteMushroom(int id)
    {
        var mushroom = await _mushroomService.GetMushroomById(id);
        if (mushroom == null)
        {
            return NotFound();
        }
        
        var isDeleted = await _mushroomService.DeleteMushroomById(id);
        if (!isDeleted)
        {
            return BadRequest();
        }

        return NoContent();
    }
    
    [Authorize(Policy = "role:researcher")]
    [HttpPost("{id:int}/research-entries")]
    public async Task<ActionResult> CreateResearchEntry(int id, ResearchEntryInputModel inputModel)
    {
        var mushroom = await _mushroomService.GetMushroomById(id);
        if (mushroom == null)
        {
            return NotFound();
        }
        
        var researcherEmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (researcherEmailAddress == null)
        {
            return Unauthorized();
        }
        
        var isCreated = await _mushroomService.CreateResearchEntry(id, researcherEmailAddress, inputModel);
        if (!isCreated)
        {
            return BadRequest();
        }

        return NoContent();
    }
}
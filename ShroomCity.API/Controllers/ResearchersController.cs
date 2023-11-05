using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.Constants;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ResearchersController : ControllerBase
{
    private readonly IResearcherService _researcherService;
    
    public ResearchersController(IResearcherService researcherService)
    {
        _researcherService = researcherService;
    }
    
    [Authorize(Policy = RoleConstants.Analyst)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResearcherDto>>> GetAllResearchers()
    {
        var researchers = await _researcherService.GetAllResearchers();
        
        if (researchers is null)
        {
            return NotFound();
        }
        
        return Ok(researchers);
    }
    
    
    [Authorize(Policy = RoleConstants.Admin)]
    [HttpPost]
    public async Task<ActionResult<int>> CreateResearcher([FromBody] ResearcherInputModel inputModel)
    {
        // Get email from claims
        var createdBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        
        
        var userId = await _researcherService.CreateResearcher(createdBy, inputModel);
        
        if (userId is null)
        {
            return BadRequest();
        }
        
        return CreatedAtAction(nameof(GetResearcherById), new { id = userId }, userId);
    }
    
    
    [Authorize(Policy = RoleConstants.Analyst)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResearcherDto>> GetResearcherById(int id)
    {
        var researcher = await _researcherService.GetResearcherById(id);
        
        if (researcher is null)
        {
            return NotFound();
        }
        
        return Ok(researcher);
    }
    
    [Authorize(Policy = RoleConstants.Researcher)]
    [HttpGet("self")]
    public async Task<ActionResult<ResearcherDto>> GetResearcherByEmailAddress()
    {
        // Get email from claims
        var emailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        
        var researcher = await _researcherService.GetResearcherByEmailAddress(emailAddress);
        
        if (researcher is null)
        {
            return NotFound();
        }
        
        return Ok(researcher);
    }
    

}
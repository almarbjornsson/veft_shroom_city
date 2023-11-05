using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.Constants;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ITokenService _tokenService;
    
    
    public AccountController(IAccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService;
        _tokenService = tokenService;
    }
    
    
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginInputModel inputModel)
    {
        // Check if user is already signed in (get token id)
        var tokenId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypeConstants.TokenIdClaimType)?.Value;
        
        // If user is already signed in, sign them out to blacklist the token
        if (tokenId != null)
        {
            var tokenIdInt = int.Parse(tokenId);
            await _accountService.SignOut(tokenIdInt);
        }
        
        
        var userDto = await _accountService.SignIn(inputModel);
        
        // This should be a "catch-all" because we don't want to give away information about any existing users
        // Example: we don't want to return a 404 if the email is correct but the password is wrong
        if (userDto is null)
        {
            return Unauthorized();
        }
        
        // If userDto is not null, we have the correct email and password
        var jwtToken = _tokenService.GenerateJwtToken(userDto);
        
        
        return Ok(jwtToken);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterInputModel inputModel)
    {
        var userDto = await _accountService.Register(inputModel);
        

        if (userDto is null)
        {
            // TODO: What to do if the user is null?
            return BadRequest();

        }
        
        // If userDto is not null, we have the correct email and password
        var jwtToken = _tokenService.GenerateJwtToken(userDto);
        
        
        return Ok(jwtToken);
    }
    
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // Get tokenID claim from token, if not found, return 401
        var tokenId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypeConstants.TokenIdClaimType)?.Value;
        if (tokenId is null)
        {
            return Unauthorized();
        }
        var tokenIdInt = int.Parse(tokenId);
        await _accountService.SignOut(tokenIdInt);

        return Ok();
    }

    // profile
    [HttpGet("profile")]
    [Authorize]
    public IActionResult Profile()
    {
        var claims = User.Claims
            .Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            })
            .ToList();

        // Returns a list of claims associated with the authenticated user
        return Ok(claims);
    }
}

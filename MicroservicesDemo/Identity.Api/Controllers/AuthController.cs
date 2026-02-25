using Azure.Core;
using Identity.Api.Models;
using Identity.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }
        else
        {
            return BadRequest(result.Errors);
        }          

        var token = _jwtTokenService.GenerateTokenAsync(user);

        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Unauthorized();

        var isValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isValid)
            return Unauthorized();

        var token = _jwtTokenService.GenerateTokenAsync(user);

        return Ok(new { token });
    }

    [HttpGet("roles")]
    public async Task<IActionResult> Roles(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    [HttpGet("addAdminRole")]
    public async Task<IActionResult> AddAdminRole(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        await _userManager.AddToRoleAsync(user, "Admin");

        return Ok(user);
    }


}
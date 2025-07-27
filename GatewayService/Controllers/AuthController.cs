using GatewayService.Dtos;
using GatewayService.Protos;
using GatewayService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = GatewayService.Protos.LoginRequest;
using RegisterRequest = GatewayService.Protos.RegisterRequest;

namespace GatewayService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return result is not null ? Ok(result) : Unauthorized("Invalid credentials.");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshAccessTokenAsync(request);
        return result is not null ? Ok(result) : Unauthorized("Invalid or expired refresh token.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var result = await _authService.LogoutAsync(request.RefreshToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

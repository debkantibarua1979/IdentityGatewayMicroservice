using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GatewayService.Clients;
using GatewayService.Dtos;
using GatewayService.Protos;
using GatewayService.Repositories.Interfaces;
using GatewayService.Services.Interfaces;
using IdentityService.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LoginRequestProto = GatewayService.Dtos.LoginRequest;
using RegisterRequest = GatewayService.Dtos.RegisterRequest;

namespace GatewayService.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly GrpcUserClient _userClient;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        GrpcUserClient userClient,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtOptions> jwtOptions,
        IHttpContextAccessor httpContextAccessor)
    {
        _userClient = userClient;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtOptions = jwtOptions.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponse?> LoginAsync(Protos.LoginRequest request)
    {
        var user = await _userClient.LoginAsync(request);
        if (user == null || string.IsNullOrEmpty(user.Id))
            return null;

        var permissions = await _userClient.GetUserPermissionsAsync(user.Id);
        var ipAddress = GetCurrentIpAddress();

        var accessToken = GenerateJwtToken(user, permissions, ipAddress);
        var refreshToken = Guid.NewGuid().ToString();

        await _refreshTokenRepository.StoreAsync(Guid.Parse(user.Id), refreshToken, ipAddress, _jwtOptions.RefreshTokenExpireDays);

        return new AuthResponse
        {
            UserId = Guid.Parse(user.Id),
            Username = user.Username,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Permissions = permissions.Permissions.Select(p => p.PermissionName).ToList()
        };
    }

    

    public async Task<AuthResponse?> RefreshAccessTokenAsync(RefreshTokenRequest request)
    {
        var userId = ValidateJwtAndGetUserId(request.RefreshToken);
        if (userId == null)
            return null;

        var isValidRefresh = await _refreshTokenRepository.ValidateAsync(userId.Value, request.RefreshToken);
        if (!isValidRefresh)
            return null;

        var user = await _userClient.GetUserByIdAsync(userId.Value.ToString());
        if (user == null)
            return null;

        var permissions = await _userClient.GetUserPermissionsAsync(user.Id);
        var ipAddress = GetCurrentIpAddress();

        var newAccessToken = GenerateJwtToken(user, permissions, ipAddress);
        var newRefreshToken = Guid.NewGuid().ToString();

        await _refreshTokenRepository.StoreAsync(Guid.Parse(user.Id), newRefreshToken, ipAddress, _jwtOptions.RefreshTokenExpireDays);

        return new AuthResponse
        {
            UserId = Guid.Parse(user.Id),
            Username = user.Username,
            Email = user.Email,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Permissions = permissions.Permissions.Select(p => p.PermissionName).ToList()
        };
    }

    

    public async Task<GenericResult> RegisterAsync(Protos.RegisterRequest request)
    {
        try
        {
            var result = await _userClient.RegisterAsync(request);
            if (string.IsNullOrEmpty(result.Id))
                return GenericResult.Fail("Registration failed");

            return GenericResult.Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            return GenericResult.Fail(ex.Message);
        }
    }

    private string GenerateJwtToken(LoginResponse user, UserPermissionsResponse permissions, string ipAddress)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("ip", ipAddress)
        };

        claims.AddRange(permissions.Permissions.Select(p =>
            new Claim("permission", p.PermissionName)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private Guid? ValidateJwtAndGetUserId(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var sub = jwtToken?.Subject;

        return Guid.TryParse(sub, out var id) ? id : null;
    }

    private string GetCurrentIpAddress()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}


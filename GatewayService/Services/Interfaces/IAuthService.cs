using GatewayService.Dtos;
using LoginRequest = GatewayService.Protos.LoginRequest;
using RegisterRequest = GatewayService.Protos.RegisterRequest;

namespace GatewayService.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RefreshAccessTokenAsync(RefreshTokenRequest request);
    Task<GenericResult> RegisterAsync(RegisterRequest request);
    Task<GenericResult> LogoutAsync(string refreshToken);

}

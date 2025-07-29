using GatewayService.Dtos;
using UserManagementService.Dtos;

namespace UserManagementService.Services.Interfaces;

using UserManagementService.Entities;

public interface IUserService
{
    Task<UserResponseDto?> GetByIdAsync(Guid id);
    Task<UserResponseDto?> GetByUsernameAsync(string username);
    Task<UserResponseDto?> GetUserByEmailAsync(string email);

    Task<List<UserResponseDto>> GetAllAsync();

    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);

    Task<UserResponseDto?> LoginAsync(LoginRequestDto request);
    Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request);

    Task<UserWithPermissionsDto> GetUserPermissionsAsync(string userId);

}

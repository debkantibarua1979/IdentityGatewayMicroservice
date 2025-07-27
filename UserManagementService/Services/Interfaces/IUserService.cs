using GatewayService.Dtos;

namespace UserManagementService.Services.Interfaces;

using UserManagementService.Entities;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<List<RolePermission>> GetUserPermissionsAsync(Guid userId);
    Task<bool> ValidatePasswordAsync(string username, string password);
    Task<User?> LoginAsync(string email, string password);
    Task<User> RegisterAsync(RegisterRequest request);
}

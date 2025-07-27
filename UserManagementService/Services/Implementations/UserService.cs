using GatewayService.Dtos;
using UserManagementService.Entities;
using UserManagementService.Repositories.Interfaces;

namespace UserManagementService.Services.Implementations;


public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _userRepository.ExistsByUsernameAsync(username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _userRepository.ExistsByEmailAsync(email);
    }

    public async Task<List<RolePermission>> GetUserPermissionsAsync(Guid userId)
    {
        return await _userRepository.GetUserPermissionsAsync(userId);
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        return await _userRepository.ValidatePasswordAsync(username, password);
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        return await _userRepository.LoginAsync(email, password);
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            RoleId = request.RoleId
        };

        return await _userRepository.RegisterAsync(user, request.Password);
    }
}

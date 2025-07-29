using UserManagementService.Dtos;
using UserManagementService.Entities;
using UserManagementService.Extensions;
using UserManagementService.Repositories.Interfaces;
using UserManagementService.Services.Interfaces;

namespace UserManagementService.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.ToDto();
    }

    public async Task<UserResponseDto?> GetByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user?.ToDto();
    }

    public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user?.ToDto();
    }

    public async Task<List<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.ToDto()).ToList();
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _userRepository.ExistsByUsernameAsync(username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _userRepository.ExistsByEmailAsync(email);
    }

    public async Task<UserResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return user.ToDto();
    }

    public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        if (await ExistsByEmailAsync(request.Email) || await ExistsByUsernameAsync(request.Username))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = request.RoleId
        };

        await _userRepository.AddAsync(user);

        return new RegisterResponseDto
        {
            Id = user.Id.ToString()
        };
    }

    public async Task<UserWithPermissionsDto> GetUserPermissionsAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        return user?.ToWithPermissionsDto() ?? new UserWithPermissionsDto();
    }
}


using Microsoft.EntityFrameworkCore;
using UserManagementService.Data;
using UserManagementService.Entities;
using UserManagementService.Repositories.Interfaces;

namespace UserManagementService.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.UserName == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<List<RolePermission>> GetUserPermissionsAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Role?.RoleRolePermissions
            .Select(rrp => rrp.RolePermission)
            .ToList() ?? new List<RolePermission>();
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        var user = await GetByUsernameAsync(username);
        if (user == null) return false;
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        return user;
    }

    public async Task<User> RegisterAsync(User user, string password)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
}


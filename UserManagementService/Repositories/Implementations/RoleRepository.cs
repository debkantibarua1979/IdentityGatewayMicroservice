using Microsoft.EntityFrameworkCore;
using UserManagementService.Data;
using UserManagementService.Entities;
using UserManagementService.Repositories.Interfaces;

namespace UserManagementService.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public async Task<List<RolePermission>> GetPermissionsAsync(Guid roleId)
    {
        return await _context.RoleRolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.RolePermission)
            .Select(rp => rp.RolePermission)
            .ToListAsync();
    }

    public async Task AssignPermissionWithAncestorsAsync(Guid roleId, RolePermission permission)
    {
        var ancestors = await _context.RolePermissions
            .Where(rp => rp.Id == permission.Id || rp.Id == permission.ParentId)
            .ToListAsync();

        foreach (var perm in ancestors)
        {
            bool alreadyAssigned = await _context.RoleRolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.RolePermissionId == perm.Id);
            if (!alreadyAssigned)
            {
                await _context.RoleRolePermissions.AddAsync(new RoleRolePermission
                {
                    RoleId = roleId,
                    RolePermissionId = perm.Id
                });
            }
        }
    }

    public async Task RemovePermissionAsync(Guid roleId, Guid rolePermissionId)
    {
        var mapping = await _context.RoleRolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.RolePermissionId == rolePermissionId);

        if (mapping != null)
        {
            _context.RoleRolePermissions.Remove(mapping);
        }
    }

    public async Task<bool> HasPermissionAsync(Guid roleId, Guid rolePermissionId)
    {
        return await _context.RoleRolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.RolePermissionId == rolePermissionId);
    }
}

using Microsoft.EntityFrameworkCore;
using UserManagementService.Data;
using UserManagementService.Entities;
using UserManagementService.Repositories.Interfaces;

namespace UserManagementService.Repositories.Implementations;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AppDbContext _context;

    public RolePermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RolePermission?> GetByIdAsync(Guid id)
    {
        return await _context.RolePermissions.FindAsync(id);
    }

    public async Task<List<RolePermission>> GetAllAsync()
    {
        return await _context.RolePermissions.ToListAsync();
    }

    public async Task AddAsync(RolePermission rolePermission)
    {
        await _context.RolePermissions.AddAsync(rolePermission);
    }

    public async Task<List<RolePermission>> GetAllWithHierarchyAsync()
    {
        return await _context.RolePermissions
            .Include(p => p.Children)
            .Include(p => p.Parent)
            .ToListAsync();
    }

    public async Task<List<RolePermission>> GetAncestorsAsync(Guid permissionId)
    {
        var result = new List<RolePermission>();
        var current = await _context.RolePermissions.FindAsync(permissionId);

        while (current?.ParentId != null)
        {
            var parent = await _context.RolePermissions.FindAsync(current.ParentId);
            if (parent != null)
            {
                result.Add(parent);
                current = parent;
            }
            else break;
        }

        return result;
    }

    public async Task<List<RolePermission>> GetDescendantsAsync(Guid permissionId)
    {
        var result = new List<RolePermission>();
        var stack = new Stack<RolePermission>();

        var root = await _context.RolePermissions
            .Include(p => p.Children)
            .FirstOrDefaultAsync(p => p.Id == permissionId);

        if (root != null)
            stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            foreach (var child in current.Children)
            {
                result.Add(child);
                var fullChild = await _context.RolePermissions
                    .Include(p => p.Children)
                    .FirstOrDefaultAsync(p => p.Id == child.Id);

                if (fullChild != null)
                    stack.Push(fullChild);
            }
        }

        return result;
    }
    
    public async Task<RolePermission> AddWithParentAsync(RolePermission rolePermission, Guid parentId)
    {
        var parent = await _context.RolePermissions
            .Include(p => p.Children)
            .FirstOrDefaultAsync(p => p.Id == parentId);

        if (parent == null)
            throw new InvalidOperationException($"Parent RolePermission with ID {parentId} not found.");

        rolePermission.ParentId = parentId;
        parent.Children.Add(rolePermission); // Optional, if tracking children explicitly

        await _context.RolePermissions.AddAsync(rolePermission);
        return rolePermission;
    }

    
}

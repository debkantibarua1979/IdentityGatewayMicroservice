namespace UserManagementService.Repositories.Interfaces;

using UserManagementService.Entities;

public interface IRolePermissionRepository
{
    Task<RolePermission?> GetByIdAsync(Guid id);
    Task<List<RolePermission>> GetAllAsync();
    Task AddAsync(RolePermission rolePermission);
    Task<List<RolePermission>> GetAllWithHierarchyAsync();
    Task<List<RolePermission>> GetAncestorsAsync(Guid permissionId);
    Task<List<RolePermission>> GetDescendantsAsync(Guid permissionId);
}

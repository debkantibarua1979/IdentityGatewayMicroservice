namespace UserManagementService.Entities;

public class RoleRolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public Guid RolePermissionId { get; set; }
    public RolePermission RolePermission { get; set; } = default!;
}

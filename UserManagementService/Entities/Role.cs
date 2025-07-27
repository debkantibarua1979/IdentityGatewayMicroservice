namespace UserManagementService.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<RoleRolePermission> RoleRolePermissions { get; set; } = new List<RoleRolePermission>();
}

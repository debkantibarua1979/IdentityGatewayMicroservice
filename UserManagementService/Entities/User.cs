namespace UserManagementService.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;
}

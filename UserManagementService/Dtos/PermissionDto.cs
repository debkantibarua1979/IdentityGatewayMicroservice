namespace UserManagementService.Dtos;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string PermissionName { get; set; } = default!;
}

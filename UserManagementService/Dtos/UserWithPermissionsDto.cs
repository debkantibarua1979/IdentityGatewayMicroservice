namespace UserManagementService.Dtos;

public class UserWithPermissionsDto
{
    public UserResponseDto User { get; set; } = default!;
    public List<PermissionDto> Permissions { get; set; } = new();
}

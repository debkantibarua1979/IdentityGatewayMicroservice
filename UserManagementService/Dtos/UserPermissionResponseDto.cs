namespace UserManagementService.Dtos;

public class UserPermissionsResponseDto
{
    public List<UserPermissionDto> Permissions { get; set; } = new();
}

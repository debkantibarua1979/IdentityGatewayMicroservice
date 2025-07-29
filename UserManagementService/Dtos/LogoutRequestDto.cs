namespace UserManagementService.Dtos;

public class LogoutRequestDto
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}

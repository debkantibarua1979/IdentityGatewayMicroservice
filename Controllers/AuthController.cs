namespace DefaultNamespace;

using GatewayService.Helpers;
using UserManagement.Protos;



public class AuthService
{
    private readonly JwtHelper _jwtHelper;
    private readonly UserPermissionService.UserPermissionServiceClient _permissionClient;

    public AuthService(JwtHelper jwtHelper, UserPermissionService.UserPermissionServiceClient permissionClient)
    {
        _jwtHelper = jwtHelper;
        _permissionClient = permissionClient;
    }

    public async Task<string> AuthenticateAsync(string userId, string userName)
    {
        // Call UserManagementService via gRPC to get permissions
        var permissionsResponse = await _permissionClient.GetUserPermissionsAsync(new UserPermissionRequest
        {
            UserId = userId
        });

        var permissions = permissionsResponse.Permissions.Select(p => p.PermissionName);

        // Generate JWT token with permissions
        return _jwtHelper.GenerateToken(userId, userName, permissions);
    }
}

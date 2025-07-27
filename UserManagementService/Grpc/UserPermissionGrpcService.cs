using Grpc.Core;
using UserManagement.Protos;

namespace UserManagementService.Grpc;

public class UserPermissionGrpcService: UserPermissionService.UserPermissionServiceBase
{
    public override Task<UserPermissionResponse> GetUserPermissions(UserPermissionRequest request, ServerCallContext context)
    {
        // Mocked permissions for demonstration
        var response = new UserPermissionResponse
        {
            UserId = request.UserId
        };

        response.Permissions.Add(new Permission
        {
            PermissionId = "perm-1",
            PermissionName = "ViewDepartment",
            Ancestors = { "BasePermission" }
        });

        response.Permissions.Add(new Permission
        {
            PermissionId = "perm-2",
            PermissionName = "UpdateDepartment",
            Ancestors = { "ViewDepartment", "BasePermission" }
        });

        return Task.FromResult(response);
    }
}
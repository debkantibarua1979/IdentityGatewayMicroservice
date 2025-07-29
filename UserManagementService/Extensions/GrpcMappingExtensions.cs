using UserManagement.Protos;
using UserManagementService.Dtos;
using UserManagementService.Entities;

namespace UserManagementService.Extensions;

public static class GrpcMappingExtensions
{
    // LoginRequest -> DTO
    public static LoginRequestDto ToDto(this LoginRequest request)
    {
        return new LoginRequestDto
        {
            Email = request.Email,
            Password = request.Password
        };
    }

    // LoginResponse <- DTO
    public static LoginResponse ToProto(this UserResponseDto dto)
    {
        return new LoginResponse
        {
            Id = dto.Id.ToString(),
            Username = dto.Username,
            Email = dto.Email
        };
    }

    // RegisterRequest -> DTO
    public static RegisterRequestDto ToDto(this RegisterRequest request)
    {
        return new RegisterRequestDto
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            RoleId = Guid.Parse(request.RoleId)
        };
    }

    // RegisterResponse <- DTO
    public static RegisterResponse ToProto(this RegisterResponseDto dto)
    {
        return new RegisterResponse
        {
            Id = dto.Id.ToString()
        };
    }

    // UserIdRequest -> Guid
    public static Guid ToGuid(this UserIdRequest request)
    {
        return Guid.Parse(request.UserId);
    }

    // EmailRequest -> string
    public static string ToEmail(this EmailRequest request)
    {
        return request.Email;
    }

    // LogoutRequest -> DTO
    public static LogoutRequestDto ToDto(this LogoutRequest request)
    {
        return new LogoutRequestDto
        {
            UserId = Guid.Parse(request.UserId),
            RefreshToken = request.RefreshToken
        };
    }

    // UserWithPermissionsDto -> UserPermissionsResponse
    public static UserPermissionsResponse ToPermissionsResponse(this UserWithPermissionsDto dto)
    {
        var response = new UserPermissionsResponse();
        response.Permissions.AddRange(dto.Permissions.Select(p =>
            new Permission
            {
                Id = p.Id.ToString(),
                PermissionName = p.PermissionName
            }));

        return response;
    }
    
    public static UserResponseDto ToDto(this User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email
        };
    }
    
    public static UserWithPermissionsDto ToWithPermissionsDto(this User user)
    {
        return new UserWithPermissionsDto
        {
            User = user.ToDto(),
            Permissions = user.Role?.RoleRolePermissions?
                .Select(rp => rp.RolePermission)
                .Where(rp => rp != null)
                .Select(rp => new PermissionDto
                {
                    Id = rp.Id,
                    PermissionName = rp.Name
                })
                .ToList() ?? new List<PermissionDto>()
        };
    }

}



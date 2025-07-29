using Grpc.Core;
using UserManagement.Protos;
using UserManagementService.Extensions;
using UserManagementService.Services.Interfaces;

namespace UserManagementService.GrpcServers;

public class UserServiceGrpc : UserServiceProto.UserServiceProtoBase
{
    private readonly IUserService _userService;

    public UserServiceGrpc(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await _userService.LoginAsync(request.ToDto());
        return result?.ToProto() ?? new LoginResponse();
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await _userService.RegisterAsync(request.ToDto());
        return result?.ToProto() ?? new RegisterResponse();
    }

    public override async Task<UserPermissionsResponse> GetUserPermissions(UserIdRequest request, ServerCallContext context)
    {
        var result = await _userService.GetUserPermissionsAsync(request.UserId);
        return result.ToPermissionsResponse();
    }

    public override async Task<LoginResponse> GetUserByEmail(EmailRequest request, ServerCallContext context)
    {
        var result = await _userService.GetUserByEmailAsync(request.Email);
        return result?.ToProto() ?? new LoginResponse();
    }

    public override async Task<LoginResponse> GetUserById(UserIdRequest request, ServerCallContext context)
    {
        var result = await _userService.GetByIdAsync(Guid.Parse(request.UserId));
        return result?.ToProto() ?? new LoginResponse();
    }
}


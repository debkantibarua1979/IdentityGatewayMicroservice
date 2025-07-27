using GatewayService.Protos;

namespace GatewayService.Clients;

using Grpc.Net.Client;
public class GrpcUserClient
{
    private readonly UserServiceProto.UserServiceProtoClient _client;

    public GrpcUserClient(IConfiguration configuration)
    {
        var channel = GrpcChannel.ForAddress(configuration["GrpcSettings:UserManagementUrl"]!);
        _client = new UserServiceProto.UserServiceProtoClient(channel);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            return await _client.LoginAsync(request);
        }
        catch
        {
            return null;
        }
    }

    public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            return await _client.RegisterAsync(request);
        }
        catch
        {
            return null;
        }
    }

    public async Task<LoginResponse?> GetUserByIdAsync(string userId)
    {
        try
        {
            return await _client.GetUserByIdAsync(new UserIdRequest { UserId = userId });
        }
        catch
        {
            return null;
        }
    }

    public async Task<LoginResponse?> GetUserByEmailAsync(string email)
    {
        try
        {
            return await _client.GetUserByEmailAsync(new EmailRequest { Email = email });
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserPermissionsResponse?> GetUserPermissionsAsync(string userId)
    {
        try
        {
            return await _client.GetUserPermissionsAsync(new UserIdRequest { UserId = userId });
        }
        catch
        {
            return null;
        }
    }
}



namespace DefaultNamespace;

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GatewayService.Grpc;
using Grpc.Net.Client;
using UserManagement;

namespace GatewayService.Clients;

public class GrpcUserClient
{
    private readonly UserService.UserServiceClient _client;

    public GrpcUserClient(UserService.UserServiceClient client)
    {
        _client = client;
    }

    public async Task<UserResponse?> RegisterAsync(RegisterRequestGrpc request)
    {
        return await _client.RegisterAsync(request);
    }

    public async Task<UserResponse?> LoginAsync(LoginRequestGrpc request)
    {
        return await _client.LoginAsync(request);
    }

    public async Task<UserResponse?> GetUserByEmailAsync(string email)
    {
        return await _client.GetUserByEmailAsync(new EmailRequest { Email = email });
    }

    public async Task<UserPermissionsResponse> GetUserPermissionsAsync(string userId)
    {
        return await _client.GetUserPermissionsAsync(new UserIdRequest { UserId = userId });
    }
}

namespace GatewayService.Dtos;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
    public string IpAddress { get; set; }
}
namespace GatewayService.Repositories.Interfaces;

// File: Repositories/Interfaces/IRefreshTokenRepository.cs

public interface IRefreshTokenRepository
    {
        Task StoreAsync(Guid userId, string token, string ipAddress, int expireInDays);
        Task<bool> ValidateAsync(Guid userId, string token);
        Task RevokeAsync(Guid userId, string token);
    }


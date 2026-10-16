using WorkHub.Api.Domain.Entities;

namespace WorkHub.Api.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);
        Task<bool> TryRotateAsync(int currentTokenId,RefreshToken replacement,DateTime nowUtc,CancellationToken cancellationToken);
    }
}

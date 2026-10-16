using WorkHub.Api.DTOs.Auth;

namespace WorkHub.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest loginRequest,string? ipAddress, CancellationToken cancellationToken);        
        Task<LoginResponse?> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    }
}

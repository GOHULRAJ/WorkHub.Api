using WorkHub.Api.Domain.Entities;
using WorkHub.Api.DTOs.Users;

namespace WorkHub.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserListResponse> GetAllUserAsync(int pageNumber,int pageSize,CancellationToken cancellationToken);
        Task<UserResponse?> GetByIdAsync(int userId, CancellationToken cancellationToken);
        Task<bool> UpdateStatusAsync(int userid, bool isActive, CancellationToken cancellationToken);

        Task<UserResponse?> CreateAsync( CreateUserRequest request,CancellationToken cancellationToken);
    }
}

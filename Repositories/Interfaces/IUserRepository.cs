using WorkHub.Api.Domain.Entities;
using WorkHub.Api.DTOs.Users;

namespace WorkHub.Api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUserAsync(int pageNumber,int pageSize,CancellationToken cancellationToken);

        Task<int> GetTotalCountAsync(CancellationToken cancellationToken);

        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);


        Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken);

        Task<User> CreateAsync(User user,CancellationToken cancellationToken);

        Task<bool> UpdateStatusAsync(int user, bool isActive, CancellationToken cancellationToken);
    }
}

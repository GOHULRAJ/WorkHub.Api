using WorkHub.Api.Domain.Entities;

namespace WorkHub.Api.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetAccessibleByIdAsync(int projectId, int userId, CancellationToken cancellationToken);
    }
}

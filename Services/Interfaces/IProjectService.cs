using WorkHub.Api.Domain.Entities;
using WorkHub.Api.DTOs.Projects;

namespace WorkHub.Api.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectResponse?> GetAccessibleProjectAsync(int projectId,int userId,CancellationToken cancellationToken);
    }
}

using WorkHub.Api.DTOs.Projects;
using WorkHub.Api.Repositories.Interfaces;
using WorkHub.Api.Services.Interfaces;

namespace WorkHub.Api.Services
{
    public class ProjectService:IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectResponse?> GetAccessibleProjectAsync(int projectId, int userId, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAccessibleByIdAsync(projectId,userId,cancellationToken);

            if (project is null)
            {
                return null;
            }

            return new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAtUtc = project.CreatedAtUtc,
                CreatedByUserId = project.CreatedByUserId
            };
        }
    }
}

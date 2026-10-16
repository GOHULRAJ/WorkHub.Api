using Microsoft.EntityFrameworkCore;
using WorkHub.Api.Data;
using WorkHub.Api.Domain.Entities;
using WorkHub.Api.Repositories.Interfaces;

namespace WorkHub.Api.Repositories
{
    public class ProjectRepository:IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetAccessibleByIdAsync(int projectId,int userid, CancellationToken cancellationToken)
        {
            var response = await _context.Projects
                            .AsNoTracking().Where(p => p.Id == projectId &&
                                (p.CreatedByUserId == userid || p.Members.Any(pm => pm.UserId == userid)))
                            .FirstOrDefaultAsync(cancellationToken);                        
         
            return response;
        }
    }
}

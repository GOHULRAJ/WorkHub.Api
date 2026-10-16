using Microsoft.EntityFrameworkCore;
using WorkHub.Api.Data;
using WorkHub.Api.Domain.Entities;
using WorkHub.Api.Repositories.Interfaces;

namespace WorkHub.Api.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUserAsync(int pageNumber,int pageSize, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }



        public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
        {
            return await _context.Users.CountAsync(cancellationToken);
        } 

        public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u=> u.Id==userId);
        }


        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == email,
                    cancellationToken);
        }

        public async Task<User> CreateAsync(User user,CancellationToken cancellationToken)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<bool> UpdateStatusAsync(int userid, bool isActive, CancellationToken cancellationToken){

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);

            if (user is null)
            {
                return false;
            }

            user.IsActive = isActive;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

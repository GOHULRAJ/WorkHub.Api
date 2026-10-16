using Microsoft.AspNetCore.Identity;
using WorkHub.Api.Domain.Entities;
using WorkHub.Api.DTOs.Users;
using WorkHub.Api.Repositories.Interfaces;
using WorkHub.Api.Services.Interfaces;

namespace WorkHub.Api.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserListResponse> GetAllUserAsync(int pageNumber,int pagesize,  CancellationToken cancellationToken)
        {
            var usersCount = await _userRepository.GetTotalCountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling( (double) usersCount / (double) pagesize);
            var userList = await _userRepository.GetAllUserAsync( pageNumber, pagesize ,cancellationToken);
            var userresposnelist = userList.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                IsActive = u.IsActive
            }).ToList();

            return new UserListResponse
            {
                users= userresposnelist,
                PageNumber = totalPages,
                PageSize = pagesize,
                TotalPages= totalPages,
                TotalRecords= usersCount
            };
        }
        public async Task<UserResponse?> GetByIdAsync( int userId,CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user is null)
                return null;

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }

        public async Task<UserResponse?> CreateAsync(CreateUserRequest request,CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var existinguser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existinguser !=null)
            {
                return null;
            }

            var user = new User
            {
                Name = request.Name.Trim(),
                Email = email,
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

            return new UserResponse
            {
                Id = createdUser.Id,
                Name = createdUser.Name,
                Email = createdUser.Email,
                IsActive = createdUser.IsActive
            };

        }
        public async Task<bool> UpdateStatusAsync(int userId,bool isActive,CancellationToken cancellationToken)
        {
            return await _userRepository.UpdateStatusAsync(userId, isActive, cancellationToken);
        }
    }
}

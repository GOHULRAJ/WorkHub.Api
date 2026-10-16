using WorkHub.Api.Domain.Entities;

namespace WorkHub.Api.DTOs.Users
{
    public class UserListResponse
    {
        public List<UserResponse> users { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}

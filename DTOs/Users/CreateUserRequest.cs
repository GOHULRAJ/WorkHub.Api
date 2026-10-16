using System.ComponentModel.DataAnnotations;

namespace WorkHub.Api.DTOs.Users
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
        //public bool IsActive { get; set; } = true;
    }
}

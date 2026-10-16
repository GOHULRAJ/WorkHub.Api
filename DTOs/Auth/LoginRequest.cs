using System.ComponentModel.DataAnnotations;

namespace WorkHub.Api.DTOs.Auth
{
    public class LoginRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}

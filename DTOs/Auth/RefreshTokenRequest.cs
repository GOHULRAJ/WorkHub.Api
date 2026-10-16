using System.ComponentModel.DataAnnotations;

namespace WorkHub.Api.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WorkHub.Api.DTOs.Auth;
using WorkHub.Api.Security;
using WorkHub.Api.Services.Interfaces;

namespace WorkHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [EnableRateLimiting("login")]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            try
            {            
                var result = await _authService.LoginAsync(
                    request,
                    ipAddress,
                    cancellationToken);

                if (result is null)
                {
                    return Unauthorized(new
                    {
                        message = "Invalid credentials or inactive account."
                    });
                }

                return Ok(result);
            }
            catch (LoginRateLimitException)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    message = "Too many login attempts. Please try again later."
                });
            }
        }



        [EnableRateLimiting("login")]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshAsync(
                request.RefreshToken,
                cancellationToken);

            if (result is null)
            {
                return Unauthorized(new
                {
                    message = "Invalid, expired, or revoked refresh token."
                });
            }

            return Ok(result);
        }

    }
}

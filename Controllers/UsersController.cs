using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.Api.DTOs.Users;
using WorkHub.Api.Services.Interfaces;

namespace WorkHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var userIdvalue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(!int.TryParse(userIdvalue,out int userId) || userId <= 0)
            {
                return Unauthorized(
                    new
                    {
                        message = "Invalid user Identity in access token."
                    });
            }

            var user = await _userService.GetByIdAsync(userId,cancellationToken);

            if (user is null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(user);
        }

        [Authorize(Roles = "admin,super_admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber=1, [FromQuery] int pageSize=10,CancellationToken cancellationToken=default)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new
                {
                    message = "Page number must be positive and page size must be between 1 and 100."
                });
            }
            var response =  await _userService.GetAllUserAsync(pageNumber, pageSize, cancellationToken);
            return Ok(response);
        }

        [Authorize(Roles = "admin,super_admin")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid user ID." });

            var user = await _userService.GetByIdAsync(
                id,
                cancellationToken);

            if (user is null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest request , CancellationToken cancellationToken)
        {
            var user = await _userService.CreateAsync(request, cancellationToken);

            if (user is null)
            {
                return Conflict(new
                {
                    message = "An account with this email already exists."
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = user.Id},user);
        }

        [Authorize(Roles = "admin,super_admin")]        
        [HttpPatch("{userId:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]       
        public async Task<IActionResult> UpdateStatus(int userId,[FromBody] UpdateUserStatusRequest request,CancellationToken cancellationToken)
        {

            var restult = await _userService.UpdateStatusAsync(userId,request.IsActive,cancellationToken);

            if (!restult)
            {
                return NotFound(new { message = "Data with userid :{"+ $" {userId}" + "} not found!" });
            }

            return Ok(new
            {
                message = request.IsActive? "User Active successfully." : "User Deactivated Successfully"
            });
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using WorkHub.Api.Data;

namespace WorkHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseHealthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DatabaseHealthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Check(CancellationToken cancellationToken)
        {
            bool canconnect = await _context.Database.CanConnectAsync(cancellationToken);

            if (!canconnect)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Database is unavailable." });
            }

            return Ok( new { message = "Database Connected Successfully."});
        } 
    }
}

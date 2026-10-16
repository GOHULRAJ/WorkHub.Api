using System.Net;
using WorkHub.Api.Responses;

namespace WorkHub.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(OperationCanceledException) when(context.RequestAborted.IsCancellationRequested)
            {
                // The client disconnected or cancelled the request.
                // Do not treat this as an unexpected server failure
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}",context.TraceIdentifier);

                if (context.Response.HasStarted)
                {
                    // Headers have already been sent; a new error response
                    // cannot safely replace the existing response.
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object?>
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    Data = null,
                    Errors = null,
                    TraceId = context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(
                response,
                context.RequestAborted);
            }
        }

    }
}

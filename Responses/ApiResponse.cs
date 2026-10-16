namespace WorkHub.Api.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; } 
        public object? Errors { get; init; }
        public string? TraceId { get; init; }
    }
}

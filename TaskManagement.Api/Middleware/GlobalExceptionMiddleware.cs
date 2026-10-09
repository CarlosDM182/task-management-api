
namespace TaskManagement.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found.");

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/problem+json";

                var response = new
                {
                    type = "https://httpstatuses.com/404",
                    title = "Resource Not Found",
                    status = StatusCodes.Status404NotFound,
                    detail = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict occurred.");

                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/problem+json";

                var response = new
                {
                    type = "https://httpstatuses.com/409",
                    title = "Conflict",
                    status = StatusCodes.Status409Conflict,
                    detail = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An unhandled exception occurred while processing the request.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var response = new
                {
                    type = "https://httpstatuses.com/500",
                    title = "Internal Server Error",
                    status = StatusCodes.Status500InternalServerError,
                    detail = "An unexpected error occurred."
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}

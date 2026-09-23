using System.Text.Json;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Middleware;
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var statusCode = ex switch
                {
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                    InvalidOperationException => StatusCodes.Status400BadRequest,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                // File/console logging (via ILogger — configure file provider in Program.cs)
                _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);

                // DB logging
                try
                {
                    dbContext.ErrorLogs.Add(new ErrorLog
                    {
                        UserId = context.User?.Identity?.IsAuthenticated == true
                            ? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            : null,
                        Path = context.Request.Path,
                        ExceptionType = ex.GetType().Name,
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        StatusCode = statusCode
                    });
                    await dbContext.SaveChangesAsync();
                }
                catch
                {
                    // If DB logging itself fails, don't let that crash the error response
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var response = new { message = ex.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

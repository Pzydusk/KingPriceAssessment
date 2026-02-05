using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UserManagement.API.ExceptionHandler
{
    public class UserManagementExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserManagementExceptionHandler> _logger;

        public UserManagementExceptionHandler(RequestDelegate next, ILogger<UserManagementExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/problem+json";

                var (statusCode, title) = ex switch
                {
                    ArgumentException => ((int)HttpStatusCode.BadRequest, "Bad request"),
                    InvalidOperationException => ((int)HttpStatusCode.BadRequest, "Invalid operation"),
                    KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Not found"),
                    _ => ((int)HttpStatusCode.InternalServerError, "Server error")
                };

                context.Response.StatusCode = statusCode;

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = statusCode == 500
                        ? "An unexpected error occurred."
                        : ex.Message,
                    Instance = context.Request.Path
                };

                problem.Extensions["traceId"] = context.TraceIdentifier;

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}

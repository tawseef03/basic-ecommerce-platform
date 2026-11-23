using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using basic_ecommerce_platform.Models;
using basic_ecommerce_platform.Exceptions;

namespace basic_ecommerce_platform.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problem = exception switch
            {
                BadRequestException ex => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = ex.Message,
                    Extensions = { ["errors"] = ex.Errors }
                },
                UnauthorizedException ex => new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = ex.Message
                },
                NotFoundException ex => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = ex.Message
                },
                ConflictException ex => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = ex.Message
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred."
                }
            };

            if (problem.Status == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "An unexpected error occurred.");

            httpContext.Response.StatusCode = problem.Status!.Value;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
    }
}

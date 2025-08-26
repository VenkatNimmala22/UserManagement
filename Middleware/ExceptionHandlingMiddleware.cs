using System.Net;
using System.Text.Json;
using UserManagementApp.Services;

namespace UserManagementApp.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public ExceptionHandlingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var logService = scope.ServiceProvider.GetRequiredService<ILogService>();

            // Log the exception
            await logService.LogErrorAsync(exception,
                $"Unhandled exception in {context.Request.Method} {context.Request.Path}");

            // Prepare response
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new
            {
                message = "An error occurred while processing your request.",
                statusCode = (int)HttpStatusCode.InternalServerError
            };

            switch (exception)
            {
                case ArgumentException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { message = "Invalid argument provided.", statusCode = response.StatusCode };
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new { message = "Unauthorized access.", statusCode = response.StatusCode };
                    break;

                case FileNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new { message = "Resource not found.", statusCode = response.StatusCode };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
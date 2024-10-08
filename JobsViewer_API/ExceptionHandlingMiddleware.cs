using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace JobsViewer_API
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = context.Response.StatusCode;
            var message = "An error occurred while processing your request.";

            switch (exception)
            {
                case ArgumentNullException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = "Bad Request. Please check your input.";
                    break;
                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "Unauthorized. Please check your credentials.";
                    break;
                // Añadir más casos según sea necesario
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "Internal Server Error. Please try again later.";
                    break;
            }

            var response = new
            {
                StatusCode = statusCode,
                Message = message,
                Detailed = exception.Message // Puedes eliminar esto en producción para no exponer detalles internos
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
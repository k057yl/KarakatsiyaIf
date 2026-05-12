using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Karakatsiya.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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
                _logger.LogError(ex, "Ахтунг! В контроллере пиздец: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var validationErrors = validationException.Errors
                    .Select(e => new
                    {
                        Field = e.PropertyName,
                        ErrorKey = e.ErrorMessage
                    })
                    .ToList();

                var validationResult = JsonSerializer.Serialize(new
                {
                    error = "ERRORS.VALIDATION_FAILED",
                    details = validationErrors
                }, _jsonOptions);

                return context.Response.WriteAsync(validationResult);
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var fatalResult = JsonSerializer.Serialize(new
            {
                error = "ERRORS.INTERNAL_SERVER_ERROR"
            }, _jsonOptions);

            return context.Response.WriteAsync(fatalResult);
        }
    }
}

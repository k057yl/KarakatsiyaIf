using FluentValidation;
using Karakatsiya.Constants;
using System.Net;
using System.Text.Json;

namespace Karakatsiya.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppConstants.Errors.MIDDLEWARE_FATAL_LOG, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = AppConstants.MimeTypes.APPLICATION_JSON;

            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var details = validationException.Errors.Select(e => new { Field = e.PropertyName, ErrorKey = e.ErrorMessage });

                return context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = AppConstants.Errors.VALIDATION_FAILED,
                    details
                }, _jsonOptions));
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = AppConstants.Errors.INTERNAL_SERVER_ERROR
            }, _jsonOptions));
        }
    }
}
namespace Karakatsiya.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Karakatsiya.Middleware.ExceptionHandlingMiddleware>();
        }
    }
}

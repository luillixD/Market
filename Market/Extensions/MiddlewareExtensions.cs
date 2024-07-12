public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlerMiddleware>();
    }
}


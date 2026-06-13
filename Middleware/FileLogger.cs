using Serilog;

namespace Shekordo.UI.Middleware;

public class FileLogger(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        await next(httpContext);

        var code = httpContext.Response.StatusCode;
        if (code / 100 != 2)
        {
            Log.Logger.Information("---> request {Path} returns {Code}",
                httpContext.Request.Path, code);
        }
    }
}

public static class FileLoggerExtensions
{
    public static IApplicationBuilder UseFileLogger(this IApplicationBuilder builder)
        => builder.UseMiddleware<FileLogger>();
}

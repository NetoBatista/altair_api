using System.Diagnostics;
using Altair.Domain.Abstraction;

namespace Altair.Middleware;


public class DefaultMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DefaultMiddleware> _logger;

    public DefaultMiddleware(RequestDelegate next, ILogger<DefaultMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await _next(context);

            stopwatch.Stop();
            var timeExecution = (int)stopwatch.Elapsed.TotalMilliseconds;

            _logger.LogInformation($"Executing {context.Request.Path} {context.Request.Method} - {DateTime.Now} - Time {timeExecution}ms - StatusCode {context.Response.StatusCode}");
        }
        catch (Exception ex)
        {
            var statusCode = ex is UseCaseException ? 400 : 500;
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var data = new ErrorResult(ex.Message, statusCode);
            await context.Response.WriteAsJsonAsync(data);

            LogError(context, ex);
        }
    }

    private void LogError(HttpContext context, Exception ex)
    {
        _logger.LogInformation($"Error executing {context.Request.Path} - {DateTime.UtcNow} - {ex}");
    }
}

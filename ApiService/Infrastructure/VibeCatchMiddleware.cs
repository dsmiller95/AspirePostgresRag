using System.Runtime.ExceptionServices;
using ApiService.Application.ErrorRecovery;
using Domain.ErrorRecovery;

namespace ApiService.Infrastructure;

public class VibeCatchMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
{
    private ILogger<VibeCatchMiddleware> Logger { get; } = loggerFactory.CreateLogger<VibeCatchMiddleware>();
    public async Task Invoke(HttpContext context)
    {
        ExceptionDispatchInfo? edi = null;
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
            edi = ExceptionDispatchInfo.Capture(exception);
        }
        
        if (edi != null)
        {
            Logger.LogError(edi.SourceException, "Exception caught in VibeCatchMiddleware for request {Path}", context.Request.Path);
            
            var errorRecoveryService = context.RequestServices.GetRequiredService<IErrorRecoveryService>();
            await HandleException(context, edi, errorRecoveryService);
        }
    }

    private async Task HandleException(HttpContext context, ExceptionDispatchInfo edi, IErrorRecoveryService errorRecoveryService)
    {
        var path = context.GetEndpoint()?.DisplayName;
        var recoveryRequest = new ErrorRecoveryRequest(path ?? "Unknown", edi.SourceException, "Error caught at middleware layer");
        var handleResult = await errorRecoveryService.GetRecovery(recoveryRequest);
        
        await WriteResponseAsync(context, handleResult);
    }
    
    private async Task WriteResponseAsync(HttpContext context, ErrorRecoveryResult result)
    {
        context.Response.Clear();
        
        context.Response.StatusCode = result.Status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result.ResultJson);
    }
}

public static class VibeCatchMiddlewareExtensions
{
    public static IApplicationBuilder UseVibeCatch(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<VibeCatchMiddleware>();
    }
}

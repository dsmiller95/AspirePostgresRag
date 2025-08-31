using ApiService.Application.ErrorRecovery;
using Domain.ErrorRecovery;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Endpoints;

public static class ErrorRecoveryEndpoints
{
    public static WebApplication MapErrorRecoveryEndpoints(this WebApplication app)
    {
        app.MapGet("/vibeCatch/cleanup", async ([FromServices] IErrorRecoveryCleanupJobRunner cleanupRunner) =>
            Results.Ok(await cleanupRunner.CleanupJob(new ErrorRecoveryCleanupRequest())))
            .WithDefaults();
        
        app.MapPost("/vibeCatch/simulate", (SimulatedError error) =>
            {
                throw new Exception(error.ErrorDescription);
            })
            .WithDefaults();

        return app;
    }
    
    private static RouteHandlerBuilder WithDefaults(this RouteHandlerBuilder builder)
    {
        return builder
            .WithTags("VibeCatch");
    }

    private record SimulatedError(string ErrorDescription);
}

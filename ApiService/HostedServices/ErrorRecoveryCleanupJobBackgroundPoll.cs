using ApiService.Application.ErrorRecovery;
using Domain.ErrorRecovery;

namespace ApiService.HostedServices;

public class ErrorRecoveryCleanupJobBackgroundPoll(
    IServiceProvider services,
    ILogger<ErrorRecoveryCleanupJobBackgroundPoll> logger) : BackgroundService
{
    private readonly TimeSpan _delay = TimeSpan.FromMinutes(10);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_delay, stoppingToken);
            try
            {
                using var scope = services.CreateScope();
                var jobRunner = scope.ServiceProvider.GetRequiredService<IErrorRecoveryCleanupJobRunner>();
                await jobRunner.CleanupJob(new ErrorRecoveryCleanupRequest());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing ErrorRecoveryCleanupJobBackgroundPoll");
            }
        }
    }
}

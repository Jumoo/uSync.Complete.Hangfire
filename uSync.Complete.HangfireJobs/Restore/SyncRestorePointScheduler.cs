using Microsoft.Extensions.Logging;

using uSync.Expansions.Core.Restore.Services;

namespace uSync.Complete.HangfireJobs.Restore;
internal class SyncRestorePointScheduler : ISyncRestorePointScheduler
{
    private readonly ISyncRestorePointService _restorePointService;
    private readonly ILogger<SyncRestorePointScheduler> _logger;

    public SyncRestorePointScheduler(
        ISyncRestorePointService restorePointService,
        ILogger<SyncRestorePointScheduler> logger)
    {
        _restorePointService = restorePointService;
        _logger = logger;
    }

    public void CreateRestorePoint(string name)
    {
        var attempt = _restorePointService.QueueCreate(
            id: Guid.NewGuid(),
            name: $"{name} {DateTime.Now:yyyyMMdd}",
            source: "Hangfire Job",
            includeMedia: true,
            user: "a background user");

        if (attempt.Success)
        {
            _logger.LogInformation("Restore point tiggered");
        }
        else
        {
            _logger.LogError(attempt.Exception, "Failed to queue restore");
        }
    }
}

using uSync.Expansions.Core.Restore;
using uSync.Expansions.Core.Restore.Strategies.Models;

namespace uSync.Complete.Hangfire.Restore;

internal class RestorePointScheduler : IRestorePointScheduler
{
    private readonly IProcessingScheduler _scheduler;

    public RestorePointScheduler(IProcessingScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public async Task<bool> CreateRestorePoint(string name, bool includeMedia)
    {
        var options = new RestorePointProcessingOptions
        {
            Source = "Hangfire Job",
            Title = $"{name} {DateTime.Now:hh_MM_dd_HHmmss}",
            IncludeMedia = includeMedia,
            IsInline = true, // inline restores happen with no ui.
        };

        return await _scheduler.ProcessPipeline(
            uSyncRestorePoints.Pipeline, uSyncRestorePoints.Strategies.Create, options);
    }
}

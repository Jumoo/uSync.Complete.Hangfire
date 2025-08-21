using Hangfire;

using Umbraco.Cms.Core.DependencyInjection;

namespace uSync.Complete.Hangfire.Restore;
public static class SyncRestoreJobs
{
    public static void CreateDailyRestorePointJob(
        this IUmbracoBuilder builder,
        string name,
        int hour,
        int minute)
    {
        // Logic to add restore point job
        RecurringJob.AddOrUpdate<RestorePointScheduler>(
            name,
            scheduler => scheduler.CreateRestorePoint(name, false),
            Cron.Daily(hour, minute));
    }

    public static void CreateDailyRestoreWithMedia(IUmbracoBuilder builder, 
        string name, 
        int hour, 
        int minute)
    {
        // Logic to add restore point job with media
        RecurringJob.AddOrUpdate<RestorePointScheduler>(
            name,
            scheduler => scheduler.CreateRestorePoint(name, true),
            Cron.Daily(hour, minute));
    }
}

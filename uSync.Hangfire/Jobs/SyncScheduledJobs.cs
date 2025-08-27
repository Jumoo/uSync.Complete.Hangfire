using Hangfire;

using Umbraco.Cms.Core.DependencyInjection;

using uSync.Hangfire.Actions;

namespace uSync.Hangfire.Jobs;

public static class SyncScheduledJobs
{
    /// <summary>
    ///  create a daily job that exports all the uSync items to the standard folder. 
    /// </summary>
    public static void AddDailySyncExportJob(this IUmbracoBuilder builder, string name, int hour, int minute)
    {
        // Logic to add export job
        RecurringJob.AddOrUpdate<ISyncActionScheduler>(
            name,
            scheduler => scheduler.Export("all"),
            Cron.Daily(hour, minute));
    }

    /// <summary>
    ///  create a daily job that imports all the uSync items from the standard folder.
    /// </summary>
    public static void AddDailySyncImportJob(this IUmbracoBuilder builder, string name, int hour, int minute)
    {
        // Logic to add import job
        RecurringJob.AddOrUpdate<ISyncActionScheduler>(
            name,
            scheduler => scheduler.Import("all", false),
            Cron.Daily(hour, minute));
    }

    /// <summary>
    ///  create a daily job that exports everything to a dated folder 
    /// </summary>
    public static void AddDailyDatedFolderExportJob(this IUmbracoBuilder builder, string name, string folder, int hour, int minute)
    {
        // Logic to add folder export job
        RecurringJob.AddOrUpdate<ISyncActionScheduler>(
            name,
            scheduler => scheduler.Export($"{folder.TrimEnd('/')}/{DateTime.Now:yyyy_MM_dd_HHmm}", "default", "all"),
            Cron.Daily(hour, minute));
    }

}

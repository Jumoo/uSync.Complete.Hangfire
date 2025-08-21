using Hangfire;

using Umbraco.Cms.Core.DependencyInjection;

using uSync.Core.Dependency;

namespace uSync.Complete.Hangfire.Publisher;
public static class SyncPublisherJobs
{
    public static void CreatePullAllContentAndMediaJob(
        this IUmbracoBuilder builder,
        string jobName,
        string server,
        int hour,
        int minute)
    {
        RecurringJob.AddOrUpdate<IPublisherScheduler>(
            jobName,
            scheduler => scheduler.PullAllContentAndMedia(server),
            Cron.Daily(hour, minute));
    }

    public static void CreatePushAllContentAndMediaJob(
        this IUmbracoBuilder builder,
        string jobName,
        string server,
        int hour,
        int minute)

    {
        RecurringJob.AddOrUpdate<IPublisherScheduler>(
            jobName,
            scheduler => scheduler.PushAllContentAndMedia(server),
            Cron.Daily(hour, minute));
    }

    public static void CreatePullContentJob(
        this IUmbracoBuilder builder,
        string jobName,
        string server,
        Guid root,
        DependencyFlags flags,
        int hour,
        int minute)

    {
        RecurringJob.AddOrUpdate<IPublisherScheduler>(
            jobName,
            scheduler => scheduler.PullContent(server, root, flags),
            Cron.Daily(hour, minute));
    }

    public static void CreatePullMediaJob(
        this IUmbracoBuilder builder,
        string jobName,
        string server,
        Guid root,
        DependencyFlags flags,
        int hour,
        int minute)

    {
        RecurringJob.AddOrUpdate<IPublisherScheduler>(
            jobName,
            scheduler => scheduler.PullMedia(server, root, flags),
            Cron.Daily(hour, minute));
    }

    public static void CreatePushContentJob(
        this IUmbracoBuilder builder,
        string jobName,
        string server,
        Guid root,
        DependencyFlags flags,
        int hour,
        int minute)

    {
        RecurringJob.AddOrUpdate<IPublisherScheduler>(
            jobName,
            scheduler => scheduler.PushContent(server, root, flags),
            Cron.Daily(hour, minute));
    }

    public static void CreatePushMediaJob(
        this IUmbracoBuilder builder,
        string jobName,
        string server,
        Guid root,
        DependencyFlags flags,
        int hour,
        int minute)

    {
        RecurringJob.AddOrUpdate<IPublisherScheduler>(
            jobName,
            scheduler => scheduler.PushMedia(server, root, flags),
            Cron.Daily(hour, minute));
    }

    public static void CreatePushAllMediaJob(
        this IUmbracoBuilder builder,
        string jobName,
        string server,
        int hour,
        int minute)
    {
        RecurringJob.AddOrUpdate<IPublisherScheduler>(
            jobName,
            scheduler => scheduler.PushMedia(server, Guid.Empty, DependencyFlags.IncludeChildren | DependencyFlags.IncludeMedia | DependencyFlags.IncludeAncestors),
            Cron.Daily(hour, minute));
    }

    public static void CreatePullAllMediaJob(
        this IUmbracoBuilder builder,
        string jobName,
        string server,
        int hour,
        int minute)
    {
        RecurringJob.AddOrUpdate<IPublisherScheduler>(
            jobName,
            scheduler => scheduler.PullMedia(server, Guid.Empty, DependencyFlags.IncludeChildren | DependencyFlags.IncludeMedia),
            Cron.Daily(hour, minute));
    }
}

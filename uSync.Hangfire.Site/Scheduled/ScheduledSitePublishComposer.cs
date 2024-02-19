using Hangfire;

using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Pkcs;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;

using uSync.Complete;
using uSync.Complete.HangfireJobs;
using uSync.Complete.HangfireJobs.Publisher;
using uSync.Complete.HangfireJobs.Restore;
using uSync.Core.Dependency;
using uSync.Core.Sync;
using uSync.Expansions.Core;
using uSync.Publisher;
using uSync.Publisher.Models;

using static Umbraco.Cms.Core.Constants;

namespace uSync.Hangfire.Site.Scheduled;

/// <summary>
///  ensure we add uSyncHangfire after everything else. 
/// </summary>
[ComposeAfter(typeof(uSyncPublishComposer))]
public class ScheduledSitePublishComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // add uSync hangfire support
        builder.AdduSyncHangfire();

        // add our own custom code that fires 
        builder.Services.AddSingleton<uSyncScheduledJobs>();

        RecurringJob.AddOrUpdate<ISyncPublisherScheduler>(
            "Publish Site Content", x =>
                x.PublishContent(-1, "target", PublishMode.Push,
                new SyncSchedulerOptions
                {
                    DependencyFlags = DependencyFlags.IncludeChildren
                }, -1), Cron.Daily());

        RecurringJob.AddOrUpdate<uSyncScheduledJobs>(
            "Publish Content", x => x.PublishWholeSite(), Cron.Hourly());

        RecurringJob.AddOrUpdate<uSyncScheduledJobs>(
            "Publish Media", x => x.PublishAllMedia(), Cron.Hourly());

        RecurringJob.AddOrUpdate<ISyncRestorePointScheduler>(
            "Create restore point", x => x.CreateRestorePoint("Daily."), Cron.Daily());
    }


}

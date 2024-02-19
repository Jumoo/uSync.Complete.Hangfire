using Umbraco.Cms.Core;

using uSync.Complete.HangfireJobs.Publisher;
using uSync.Core.Dependency;
using uSync.Core.Sync;
using uSync.Expansions.Core;
using uSync.Publisher.Models;

using static Umbraco.Cms.Core.Constants;

namespace uSync.Hangfire.Site.Scheduled;

/// <summary>
///  our custom class of scheduled job items. 
/// </summary>
public class uSyncScheduledJobs
{
    private readonly ISyncPublisherScheduler _publisherScheduler;

    public uSyncScheduledJobs(ISyncPublisherScheduler publisherScheduler)
    {
        _publisherScheduler = publisherScheduler;
    }


    /// <summary>
    ///  publish content -1, with all children 
    /// </summary>
    /// <remarks>
    ///     Include children - all child content items.
    ///     Include dependencies - all doctypes, datatypes ,etc needed to publish content.
    /// </remarks>
    /// <returns></returns>
    public async Task PublishWholeSite()
    {
        await _publisherScheduler.PublishContent(-1, "target", PublishMode.Push,
            new SyncSchedulerOptions
            {
                DependencyFlags = DependencyFlags.IncludeChildren | DependencyFlags.IncludeDependencies

            }, -1);
    }

    /// <summary>
    ///  publish media, (and all children)
    /// </summary>
    /// <returns></returns>
    public async Task PublishAllMedia()
    {
        await _publisherScheduler.PublishMedia(-1, "target", PublishMode.Push,
            new SyncSchedulerOptions
            {
                DependencyFlags = DependencyFlags.IncludeChildren
            }, -1);
    }

    /// <summary>
    ///  push all the doctypes. 
    /// </summary>
    /// <param name="scheduler"></param>
    /// <returns></returns>
    public async Task PublishDocTypes(ISyncPublisherScheduler scheduler)
    {
        var item = new SyncItem
        {
            Udi = Udi.Create(UdiEntityType.DocumentType),
            Name = "All document types",
            Flags = DependencyFlags.IncludeChildren | DependencyFlags.IncludeDependencies
        };

        await scheduler.PublishItem(item, "target", PublishMode.Push,
            new SyncSchedulerOptions
            {
                EntityType = UdiEntityType.DocumentType,
                Options = new SyncPackOptions
                {
                    CreateRestorePoint = true,
                    IncludeFiles = true,
                    IncludeContent = true,
                }
            },
            -1);
    }
   
}

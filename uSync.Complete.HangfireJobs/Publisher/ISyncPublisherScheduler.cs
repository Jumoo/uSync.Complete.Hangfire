using uSync.Core.Sync;
using uSync.Publisher.Models;

namespace uSync.Complete.HangfireJobs.Publisher;
public interface ISyncPublisherScheduler
{
    /// <summary>
    ///  find a piece of content and push it (and potentially its children) in to
    ///  the publish queue.
    /// </summary>
    Task<bool> PublishContent(int contentId, string serverAlias, PublishMode mode, SyncSchedulerOptions options, int userId = -1);

    /// <summary>
    ///  Publish and item between servers. 
    /// </summary>
    Task<bool> PublishItem(SyncItem item, string serverAlias, PublishMode mode, SyncSchedulerOptions options, int userId);

    /// <summary>
    ///  publish a collection of items between servers
    /// </summary>
    Task<bool> PublishItems(IEnumerable<SyncItem> items, string serverAlias, PublishMode mode, SyncSchedulerOptions options, int userId);
    Task<bool> PublishMedia(int mediaId, string serverAlias, PublishMode mode, SyncSchedulerOptions options, int userId = -1);
}
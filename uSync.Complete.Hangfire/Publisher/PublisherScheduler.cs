using Umbraco.Cms.Core;

using uSync.Core.Dependency;
using uSync.Core.Sync;
using uSync.Publisher.Models;
using uSync.Publisher.Process.Models;
using uSync.Publisher.Strategies.Models;

using static Umbraco.Cms.Core.Constants;

namespace uSync.Complete.Hangfire.Publisher;
internal class PublisherScheduler : IPublisherScheduler
{
    private readonly IProcessingScheduler _scheduler;

    public PublisherScheduler(IProcessingScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public async Task<bool> PushContent(string server, Guid root, DependencyFlags flags)
        => await PerformCommand(server, PublishMode.Push, UdiEntityType.Document, root, flags);

    public async Task<bool> PushMedia(string server, Guid root, DependencyFlags flags)
        => await PerformCommand(server, PublishMode.Push, UdiEntityType.Media, root, flags);

    public async Task<bool> PullContent(string server, Guid root, DependencyFlags flags)
        => await PerformCommand(server, PublishMode.Pull, UdiEntityType.Document, root, flags);

    public async Task<bool> PullMedia(string server, Guid root, DependencyFlags flags)
        => await PerformCommand(server, PublishMode.Pull, UdiEntityType.Media, root, flags);

    public async Task<bool> PushAllContentAndMedia(string server)
        => await PerformCommand(server, PublishMode.Push, UdiEntityType.Document, Guid.Empty,
            DependencyFlags.IncludeChildren | DependencyFlags.IncludeMedia);

    public async Task<bool> PullAllContentAndMedia(string server)
        => await PerformCommand(server, PublishMode.Pull, UdiEntityType.Document, Guid.Empty,
            DependencyFlags.IncludeChildren | DependencyFlags.IncludeMedia);

    private async Task<bool> PerformCommand(string server, PublishMode mode, string entityType, Guid root, DependencyFlags flags)
    {
        var options = new PublisherProcessingOptions
        {
            Mode = mode,
            Server = server,
            EntityType = entityType,
            IsBackgroundRequest = true,
            Items = new List<SyncItem>
            {
                new SyncItem
                {
                   Name = "Start point ",
                   Udi = new GuidUdi(entityType, root),
                   Flags = flags,
                   Icon = "icon-document",
                }
            },
            PublisherOptions = new SyncPublisherOptions
            {
                IncludeChildren = true,
                IncludeMedia = true,
                IncludeAncestors = true,
                IncludeFileHash = true,
                SkipReport = true,
            }
        };

        return await _scheduler.ProcessPipeline(
           "PublisherPipeline",
           "RealtimePushStrategy", options);
    }
}

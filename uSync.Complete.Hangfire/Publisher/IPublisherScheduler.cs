
using uSync.Core.Dependency;

namespace uSync.Complete.Hangfire.Publisher;
public interface IPublisherScheduler
{
    Task<bool> PullAllContentAndMedia(string server);
    Task<bool> PullContent(string server, Guid root, DependencyFlags flags);
    Task<bool> PullMedia(string server, Guid root, DependencyFlags flags);
    Task<bool> PushAllContentAndMedia(string server);
    Task<bool> PushContent(string server, Guid root, DependencyFlags flags);
    Task<bool> PushMedia(string server, Guid root, DependencyFlags flags);
}
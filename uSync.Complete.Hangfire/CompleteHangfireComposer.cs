using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

using uSync.Complete.Hangfire.Publisher;
using uSync.Complete.Hangfire.Restore;

namespace uSync.Complete.Hangfire;

[ComposeAfter(typeof(global::uSync.Expansions.Core.uSyncExpansionsCoreComposer))]
public class CompleteHangfireComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AdduSyncCompleteHangfire();
    }
}

internal static class uSyncCompleteHangfireComposerExtensions
{
    public static IUmbracoBuilder AdduSyncCompleteHangfire(this IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IProcessingScheduler, ProcessingScheduler>();
        builder.Services.AddSingleton<IRestorePointScheduler, RestorePointScheduler>();
        builder.Services.AddSingleton<IPublisherScheduler, PublisherScheduler>();
        return builder;
    }

}

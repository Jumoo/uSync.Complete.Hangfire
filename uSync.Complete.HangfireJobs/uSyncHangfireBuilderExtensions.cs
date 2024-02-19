using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.DependencyInjection;

using uSync.Complete.HangfireJobs.Publisher;
using uSync.Complete.HangfireJobs.Restore;

namespace uSync.Complete.HangfireJobs;
public static class uSyncHangfireBuilderExtensions
{
    public static IUmbracoBuilder AdduSyncHangfire(this IUmbracoBuilder builder)
    {
        // check as we don't need to register things twice.
        if (builder.Services.FirstOrDefault(x => x.ServiceType == typeof(ISyncPublisherScheduler)) != null)
            return builder;
        
        builder.Services.AddSingleton<ISyncPublisherScheduler, SyncPublisherScheduler>();
        builder.Services.AddSingleton<ISyncRestorePointScheduler, SyncRestorePointScheduler>();

        return builder;
    }
}

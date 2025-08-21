using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

using uSync.Hangfire.Actions;

namespace uSync.Hangfire;

[ComposeAfter(typeof(global::uSync.BackOffice.uSyncBackOfficeComposer))]
public class uSyncHangfireComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AdduSyncHangfire();
    }
}

public static class uSyncHangfireBuilderExtensions
{
    public static IUmbracoBuilder AdduSyncHangfire(this IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<ISyncActionScheduler, SyncActionScheduler>();
        return builder;
    }
}

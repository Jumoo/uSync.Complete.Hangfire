using Umbraco.Cms.Core.Composing;

using uSync.Complete.Hangfire.Publisher;
using uSync.Complete.Hangfire.Restore;
using uSync.Hangfire.Jobs;

namespace uSyncSource.Site.Scheduled;

[ComposeAfter(typeof(uSync.Hangfire.uSyncHangfireComposer))]
public class ScheduledJobComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddDailySyncExportJob("Daily Export", 17, 10);
        builder.AddDailySyncImportJob("Daily Import", 17, 15);

        builder.AddDailyDatedFolderExportJob("Daily Backup", "uSync/backup", 01, 20);

        builder.CreateDailyRestorePointJob("Daily Restore Point", 02, 00);

        builder.CreatePushAllMediaJob("Daily media push", "Target", 03, 00);

        builder.CreatePushAllContentAndMediaJob("Daily Site Sync", "Target", 04, 00);
    }
}

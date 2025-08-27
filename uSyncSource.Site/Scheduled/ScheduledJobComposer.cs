using Hangfire;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.ContentEditing;

using uSync.Complete.Hangfire.Publisher;
using uSync.Complete.Hangfire.Restore;
using uSync.Hangfire.Actions;
using uSync.Hangfire.Jobs;

using static Umbraco.Cms.Core.Constants;

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

        string[] entityTypes = [UdiEntityType.DocumentType, UdiEntityType.DataType, UdiEntityType.MediaType];
        
        RecurringJob.AddOrUpdate<ISyncActionScheduler>(
            "ExportSomeTypes",
            s => s.Export("uSync/backups/MyTypes", entityTypes),
            Cron.Daily(5, 30));
    }
}

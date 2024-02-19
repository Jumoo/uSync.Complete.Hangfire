# uSync.Complete.Hangfire.

A collection of helper methods to run uSync.Complete commands as scheduled jobs (via hangfire)

### Example: Push the whole site once a day. 

```cs
RecurringJob.AddOrUpdate<ISyncPublisherScheduler>(
    "Publish Site Content", x =>
    x.PublishContent(-1, "target", PublishMode.Push,
    new SyncSchedulerOptions
    {
        DependencyFlags = DependencyFlags.IncludeChildren
    }, -1),
    Cron.Daily());
```

### Example: Create A Daily restore point 

```cs
RecurringJob.AddOrUpdate<ISyncRestorePointScheduler>(
    "Create restore point", 
    x => x.CreateRestorePoint("Daily."), 
    Cron.Daily());
```


# Getting started. 
You need to register add "uSyncHangfire" to your project.

e.g via a composer :

```cs
[ComposeAfter(typeof(uSyncPublishComposer))]
public class ScheduledSitePublishComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // add uSync hangfire support
        builder.AdduSyncHangfire();

        // your scheduled jobs can be added here....
    }
}
```

See the examples in the repo for more. 

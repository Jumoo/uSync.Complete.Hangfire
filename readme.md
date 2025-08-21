# uSync.Hangfire

Two libraries that help you run uSync commands as part of hangfire jobs.

## uSync.Hangfire

Contains helpers for the core (free) uSync package. that allow you to run
imports or exports via hangfire.

### Example - Add a hangfire job to export once a day.

```cs
builder.AddDailySyncExportJob("Daily Export", 17, 10);
```

## uSync.Complete.Hangfire

Contains helpers for uSync.Complete that let you do cool things such as

- create a restore point
- push or pull content between servers
- push or pull media between servers
- well push or pull anything really.

### Example - Create a restore point

```cs
builder.CreateDailyRestorePointJob("Daily Restore Point", 02, 00);
```

### Example - Push all of the content and media to another site once a day

```cs
builder.CreatePushAllContentAndMediaJob("Daily Site Sync", "Target", 04, 00);
```

## Helpers and sample code.

Yopu can do more complicated things and there are lots of helpers and sample code

[uSyncSource.Site/Scheduled](uSyncSource.Site/Scheduled) contains some basic commands for a website.

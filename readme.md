# uSync.Hangfire

Two libraries that help you run uSync commands as part of hangfire jobs
inside umbraco

**Requires:**

- Umbraco v16
- Cultiv.Hangfire package

## uSync.Hangfire

Contains helpers for the core (free) uSync package. that allow you to run
imports or exports via hangfire.

```
dotnet add pacakge uSync.Hangfire
```

### Example - Add a hangfire job to export once a day.

```cs
builder.AddDailySyncExportJob("Daily Export", 17, 10);
```

## uSync.Complete.Hangfire

Contains helpers for uSync.Complete

```
dotnet add package uSync.Complete.Hangfire
```

that let you do cool things such as

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

You can also do things like specifiy which bit of content or media to use
as the root fpr the push or the pull

```cs
builder.CreatePullMediaJob("Pull Backgrounds folder", // name for Hangfire.
    "Target", // alias of the server.
    Guid.Parse("f2b3c1d4-5e6f-7a8b-9c0d-e1f2g3h4i5j6"),  // guid of the folder we want to use as root.
    DependencyFlags.IncludeChildren, // flags (like include child items)
    05, 00); // time of day (5 AM)
```

## Helpers and sample code.

Yopu can do more complicated things and there are lots of helpers and sample code

[uSyncSource.Site/Scheduled](uSyncSource.Site/Scheduled) contains some basic commands for a website.

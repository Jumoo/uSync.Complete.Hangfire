# uSync.Hangfire

library that helps you run uSync commands as part of hangfire jobs
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

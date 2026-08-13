## What does this change?

<!-- A sentence or two on the change and why it is needed. -->

## Notes for the reviewer

<!-- Anything non-obvious: behaviour changes, things you decided against, areas you want a
     second opinion on. Delete if there is nothing to say. -->

## Checklist

- [ ] `dotnet build uSync.Hangfire/uSync.Hangfire.csproj -c Release` is clean
- [ ] `dotnet build uSync.Complete.Hangfire/uSync.Complete.Hangfire.csproj -c Release` is clean
- [ ] `CHANGELOG.md` updated under **Unreleased**
- [ ] If a job-scheduling method's signature or behaviour changed, the `readme.md` examples
      for that package still match

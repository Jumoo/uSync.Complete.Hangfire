# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Two NuGet packages that schedule [uSync](https://usync.dev) operations as Hangfire background jobs
inside Umbraco:

- **`uSync.Hangfire`** — wraps the free/core uSync import and export commands.
- **`uSync.Complete.Hangfire`** — wraps uSync.Complete: push/pull content and media between
  servers, and creating restore points. Depends on `uSync.Publisher` and
  `uSync.Expansions.Core`.

Both are plugins — they do nothing without the underlying uSync packages, whose types come in
via NuGet and are not in this repo.

## Commands

Build each project directly, not the solution:

```bash
dotnet build uSync.Hangfire/uSync.Hangfire.csproj -c Release
```

```bash
dotnet build uSync.Complete.Hangfire/uSync.Complete.Hangfire.csproj -c Release
```

There are no automated tests. `uSyncSource.Site` and `uSyncTarget.Site` are two local Umbraco
sites used to click-test push/pull between servers by hand — verification for anything that
touches `PublisherScheduler` or `RestorePointScheduler` means running both and watching a job
fire, not just a clean build.

## Repository shape

**Branches are per Umbraco major** — `v17/main` is the default branch and the 17 release
line. There is no plain `main`. Workflow filters use `[ "main", "*/main" ]` and GitVersion
uses `^(v[0-9]+\/)?main$`, so both forms work and the next major needs no CI change.

`uSync.Hangfire.slnx`, `uSyncSource.Site` and `uSyncTarget.Site` **are** committed to this
repo — unlike some other Jumoo repos, they aren't gitignored local-only scaffolding. Still
build and pack the individual **project**, never the solution, matching CI.

Adding `Directory.build.props` here stops any `Directory.build.props` further up the disk
from applying, which is why `NuGetAuditMode` is repeated in it rather than left to inherit.

## The DI split — the thing to get right

`IProcessingScheduler`, `IPublisherScheduler` and `IRestorePointScheduler` are registered in
DI; their concrete implementations (`ProcessingScheduler`, `PublisherScheduler`,
`RestorePointScheduler`) are `internal` and are **not** registered on their own. Hangfire's
job activator resolves whatever type a recurring job is scheduled against through the app's
`IServiceProvider` at execution time, so `RecurringJob.AddOrUpdate<T>()` must always be
called with the interface. Scheduling against the concrete type compiles fine — it's an
internal-to-internal reference within the same assembly — but fails to resolve when the job
actually fires. This exact mistake shipped once in `SyncRestoreJobs`; `SyncPublisherJobs` is
the pattern to copy.

`ProcessPipeline` (on `IProcessingScheduler`) is the single choke point every scheduler routes
through, and it's gated behind a static `SemaphoreSlim` so two overlapping Hangfire jobs can't
drive the same `IPipelineService` pipeline concurrently — a second call fails fast rather than
racing the first.

## Publisher push/pull — things that will catch you out

**The strategy alias has to be derived from `PublishMode`, not hard-coded.** uSync.Publisher's
own strategy alias constants (`PublisherStrategy.Strategies`) are internal to that assembly, so
`PublisherScheduler` keeps its own small `PublishMode -> alias string` mapping rather than
referencing them. If a push and a pull end up passing the same alias, pulls silently run the
push strategy instead — nothing is pulled, and local content goes out instead.

**`DependencyFlags` has to be threaded onto `SyncPublisherOptions`, not just onto the
`SyncItem`.** `SyncPublisherPipelineProcessor.UpdateOptions` (in `uSync.Publisher`) derives
every item's effective `DependencyFlags` from `PublisherOptions.ToDependencyFlags()` and
overwrites whatever was set directly on the item — so setting `SyncItem.Flags` alone has no
effect. Build `IncludeChildren`/`IncludeMedia`/`IncludeAncestors`/etc. on
`SyncPublisherOptions` from the flags the caller actually asked for.

## Versioning and releases

Tags are plain `{major}.{minor}.{patch}` (no `v` prefix) pushed on a release branch — that's
what `release.yml` matches and what it stamps onto the packages via `/p:version=`. There are
no committed `packages.lock.json` files yet, so `--locked-mode` restore in CI is currently a
no-op rather than an enforced lock.

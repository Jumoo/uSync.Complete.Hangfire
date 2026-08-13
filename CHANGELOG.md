# Changelog

Notable changes to `uSync.Hangfire` and `uSync.Complete.Hangfire`. Both ship from this repo
and version together.

## Unreleased

### Added

- Repository standards: `LICENCE.txt`, `README.md`, `CHANGELOG.md`, `SECURITY.md`,
  `CODE_OF_CONDUCT.md`, `.editorconfig`, `global.json`, `Directory.build.props`,
  `GitVersion.yml`, dependabot, and issue and PR templates.
- CI workflows — PR build, package build, release, and CodeQL.

### Fixed

- `PublisherScheduler` always ran the push strategy regardless of `PublishMode`, so
  `PullContent`, `PullMedia` and `PullAllContentAndMedia` silently pushed local content and
  media to the target server instead of pulling from it.
- `PerformCommand` ignored the `DependencyFlags` passed to it and hard-coded
  `IncludeChildren`/`IncludeMedia`/`IncludeAncestors` to `true` on every push or pull, so a
  caller asking for just one item still got the whole subtree and its media.
- `ProcessingScheduler` had no guard against two Hangfire jobs driving the same pipeline
  machinery concurrently. `ProcessPipeline` now serializes behind a short-wait gate, so a
  second overlapping run fails fast instead of racing the first.
- `SyncRestoreJobs` scheduled its daily restore-point jobs against the concrete
  `RestorePointScheduler` type, which isn't registered in DI — only `IRestorePointScheduler`
  is — so those jobs would fail to resolve the scheduler when they actually ran.

## 0.17.2

- Umbraco 17 release line.

[Unreleased]: https://github.com/Jumoo/uSync.Hangfire/compare/0.17.2...HEAD

# uSync.Hangfire

Run [uSync](https://usync.dev) operations as scheduled Hangfire background jobs inside Umbraco.

## Packages

| Package | What it is |
| --- | --- |
| [`uSync.Hangfire`](uSync.Hangfire) | Helpers for the core (free) uSync package — schedule imports and exports. |
| [`uSync.Complete.Hangfire`](uSync.Complete.Hangfire) | Helpers for uSync.Complete — schedule restore points, and push/pull content or media between servers. |

Each package has its own [readme](uSync.Hangfire/readme.md) with usage examples; the
`uSync.Complete.Hangfire` one is [here](uSync.Complete.Hangfire/readme.md).

```bash
dotnet add package uSync.Hangfire
```

```bash
dotnet add package uSync.Complete.Hangfire
```

> Requires the [Cultiv.Hangfire](https://www.nuget.org/packages/Cultiv.Hangfire) package, and
> `uSync.Complete.Hangfire` additionally requires uSync.Complete (`uSync.Publisher` /
> `uSync.Expansions.Core`).

## Repository layout

| Path | What it is |
| --- | --- |
| `uSync.Hangfire` | The core-uSync package — this is what ships |
| `uSync.Complete.Hangfire` | The uSync.Complete package — this is what ships |
| `uSyncSource.Site` / `uSyncTarget.Site` | Two local Umbraco sites for click-testing server-to-server push/pull, and sample scheduled-job code under [`uSyncSource.Site/Scheduled`](uSyncSource.Site/Scheduled) |

## Building

Requires the .NET SDK pinned in [`global.json`](global.json).

```bash
dotnet build uSync.Hangfire/uSync.Hangfire.csproj -c Release
```

```bash
dotnet build uSync.Complete.Hangfire/uSync.Complete.Hangfire.csproj -c Release
```

Shared build and package metadata lives in [`Directory.build.props`](Directory.build.props).

## Releasing

Pushing a `v{version}` tag on `v17/main` publishes both packages to NuGet:

```bash
git tag v0.17.3 && git push origin v0.17.3
```

The tag is the version — `v0.17.3` publishes `0.17.3`. The workflow refuses to run if the
tag isn't a valid version, or if the tagged commit isn't on a release branch.

Authentication is [trusted publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing) —
the job exchanges a GitHub OIDC token for a short-lived NuGet key, so there is no API key
stored in the repository. It depends on a policy on nuget.org naming this repository, the
`release.yml` workflow and the `nuget` environment; if any of those are renamed, the policy
has to be updated to match or publishing stops working.

Every push to `v17/main` also builds and packs both projects and uploads them as a build
artifact, so a release candidate can be tested before a tag is pushed.

## Contributing

Please read [SECURITY.md](SECURITY.md) before reporting anything security related, and
[CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) before taking part.

## Licence

[MPL-2.0](LICENCE.txt).

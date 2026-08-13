# Security Policy

## Supported versions

This repo ships one release line per Umbraco major, matching the packages it wraps
(`uSync`, `uSync.Publisher`, `uSync.Expansions.Core`). Fixes go to the current line, and
to the previous one where the issue is serious and the fix is practical.

| Version | Branch | Supported |
| --- | --- | --- |
| 17.x | `v17/main` | Yes |
| 16.x and earlier | — | No |

## Reporting a vulnerability

Please **do not** open a public issue for a security problem.

Email **info@jumoo.co.uk** with a description of the issue, the version affected, and steps
to reproduce it. We'll acknowledge within a few working days and keep you updated as we
work on it.

These packages schedule uSync import/export, publisher push/pull, and restore-point
operations as Hangfire background jobs — so if the issue involves how a job resolves the
current user, how server-to-server publishing authenticates, or anything that could let a
scheduled job run with more access than intended, say so — those get sequenced first.

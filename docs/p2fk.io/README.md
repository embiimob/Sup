# p2fk.io update — in-flight CLI coalescing

This directory contains updated source files to apply to the
[embiimob/p2fk.io](https://github.com/embiimob/p2fk.io) repository.

## Why

`p2fk.io` calls `SUP.exe` many times per second to service API requests.
Every CLI invocation reads the on-disk cache for the requested address,
advances the cursor, and writes the cache back.  When multiple concurrent
requests arrive for the same address and command (e.g. 20 tabs all loading
the same profile), 20 CLI processes are spawned simultaneously.  They all
race to read the same snapshot, compute the same incremental work, and then
race to write the result back to the same JSON file.

Even with the cross-process named-mutex guards added to `SUP.exe` in this PR
(which serialize the writes), the redundant work still forces the mutex queue
to grow and increases the chance of a cursor regression from a slow process
winning the final write.

The fix applied here: **in-flight request coalescing in `Wrapper.cs`**.

## What changed in `Wrapper.cs`

| Before | After |
|--------|-------|
| Every `RunCommandAsync` call spawned its own `Process`. | Concurrent calls with the same address + command share **one** `Process`; all callers `await` the same `Task<string>`. |
| Global `SemaphoreSlim(8,8)` was the only concurrency guard. | Coalescing key `(executablePath, command verb, address, versionbyte)` is checked first; only one process runs per unique key at a time. |
| A caller's `CancellationToken` was wired directly into the process I/O streams, so an HTTP disconnect could kill the build mid-write. | Per-caller cancellation is scoped to `WaitAsync` only; the underlying process runs with an internal timeout token and always completes, so the disk cache is written even if the originating HTTP request is abandoned. |
| `--skip` and `--qty` variations for the same address all ran as separate processes, each racing to update the same file. | All `--skip` / `--qty` variants coalesce onto a single build because the key excludes those flags. |

## How to apply

Copy `Wrapper.cs` from this directory into the root of your `p2fk.io` repository,
replacing the existing `Wrapper.cs`.

No other files need to change for this fix.  The `SemaphoreSlim(8,8)` global cap
is retained as a backstop for commands that have no `--address` parameter (e.g.
`--getfoundobjects`) and therefore do not coalesce.

## Interaction with the SUP.exe changes in this PR

The two fixes are complementary:

* **SUP.exe** (this PR): cross-process named OS mutexes in `AcquireAddressCacheLock`
  serialise writes from any remaining concurrent CLI processes and guard against
  cursor regression with the `ShouldCommitCache` monotonicity check.
* **p2fk.io** (this directory): in-process coalescing ensures that the API
  never spawns more than one CLI process per address+command in the first place,
  so the mutex in SUP.exe is almost never contested.

Together they give defence-in-depth: the API coalesces at the HTTP layer, and
the CLI serialises at the file-write layer.

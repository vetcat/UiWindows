# R3 MVP Conventions

This document records the `UIW-13` dependency and usage boundary for `Cysharp/R3` in the UI.Windows MVP architecture.

## Dependency Pins

- `NuGetForUnity` is pinned through Unity Package Manager: `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity#v4.5.0`.
- `R3` core is pinned through NuGetForUnity in `Packages/nuget-packages/packages.config` at `1.3.1`.
- `R3.Unity` is pinned through Unity Package Manager: `https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity#1.3.1`.

`R3.Unity` is intentionally paired with the NuGet package. The `R3.Unity.asmdef` on tag `1.3.1` references precompiled `R3.dll`, `Microsoft.Bcl.TimeProvider.dll`, and `Microsoft.Bcl.AsyncInterfaces.dll`, which are supplied by NuGet restore.

`Microsoft.Bcl.AsyncInterfaces` is pinned to NuGet package `6.0.0` because `Microsoft.Bcl.TimeProvider 8.0.0` references assembly version `6.0.0.0`.

## Fresh Checkout Restore

The deterministic restore config lives under `Packages/nuget-packages`:

- `package.json` makes `Packages/nuget-packages` visible to Unity as an embedded package.
- `NuGet.config` selects `nuget.org`, `InPackagesFolder`, `slimRestore`, and NetStandard preference.
- `packages.config` pins `R3` and the NuGet dependencies needed by the Unity package.
- `InstalledPackages` contains the restored NuGet artifacts and `.meta` import settings used by the verified Unity compile.

Fresh checkouts should compile with the committed `InstalledPackages` artifacts. If those artifacts are missing or need to be regenerated, use the NuGetForUnity restore workflow before opening Unity in CI:

```bash
dotnet tool install --global NuGetForUnity.Cli --version 4.5.0
dotnet nugetforunity restore /Users/vitaly/Projects/UiWindows
```

Developer fallback when the CLI is unavailable:

1. Open the project in Unity.
2. If Unity shows initial missing NuGet assembly compiler errors, choose Ignore rather than Safe Mode.
3. Let NuGetForUnity restore packages automatically, or run `NuGet -> Restore Packages`.
4. Wait for Unity to recompile.

This fallback is a known NuGetForUnity first-launch limitation when restored artifacts are absent because Unity may compile scripts before the NuGetForUnity editor package restores `packages.config`.

## MVP Surface Rules

- Owners may use `ReactiveProperty<T>`, `Subject<T>`, and other mutable R3 primitives internally.
- Public model/read-model ports expose read-only surfaces, normally `ReadOnlyReactiveProperty<T>` for state with a current value or `Observable<T>` for event/request streams.
- Mutable R3 types must not be exposed outside the object that owns mutation.
- Mutations cross boundaries through command methods, for example `SetName`, `AddCoins`, `SelectItem`, or `RequestClose`, not by setting a public reactive property.
- Domain/application services must not depend on UI presenters. Visual effects, navigation, and modal requests should be emitted as read-only request streams or routed through narrow project-owned ports.
- Do not introduce UniRx or a project-owned custom Rx framework in parallel with R3.

## Lifetime Rules

For the full pooled window verification pattern, see `docs/uiwindows-mvp-pooling-lifecycle.md`.

- `IDisposable` is the subscription ownership boundary.
- CompositionRoot owns service/model lifetimes and disposes them through `ServiceRegistry`.
- Presenters own presenter-lifetime subscriptions and dispose them when the presenter is finally disposed.
- Window show-scoped subscriptions must be disposed on hide completion or when UI.Windows returns a window to the pool.
- Pooled windows must recreate show-scoped subscriptions on each show and must not accumulate duplicate handlers across show/hide cycles.
- A pooled UI.Windows window keeps its presenter binding for the lifetime of that pooled window instance; only the show scope is recreated per show. Rebind only when UI.Windows has produced a new window instance without an active binding.
- `OnDeInit` remains final cleanup. Do not wait for `OnDeInit` to release show-scoped subscriptions for pooled windows.
- `AddTo(Component)` is acceptable only for subscriptions whose lifetime should match Unity object destruction. It is not a substitute for explicit show/hide cleanup.

## Compile Smoke

`Assets/Scripts/UiWindowsMvp/Runtime/R3Integration/R3MvpSmokeCheck.cs` references:

- `ReactiveProperty<T>`
- `ReadOnlyReactiveProperty<T>`
- `R3.Unity` lifetime extension `AddTo(Component)`

It intentionally contains no presenter adapter, model behavior, windows, or OpenUI port.

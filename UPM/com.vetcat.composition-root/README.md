# Composition Root

Canonical reusable source in vetcat/UiWindows. Assembly: `CompositionRoot.Runtime`.

Add `SceneCompositionRoot` to the scene bootstrap GameObject. Installers implement
`ICompositionInstaller` and register service instances in `Install(IServiceRegistry)`.
Use the ordered serialized installer list for an explicit dependency order; when
that list is empty the root discovers installers on the same GameObject.

The root bootstraps in `Awake` (execution order -10000), installs all services,
then calls `IInitializable.Initialize` in registration order. On destruction or
`Shutdown`, registered `IDisposable` services are disposed in reverse order.
The same object registered under several interfaces participates once.

The root's Unity `Start` calls `Startup()`, which invokes `IStartable.Start` once
in registration order, after active scene objects have completed `Awake`.
Use this phase to start scene workflows that require initialized Unity components.
`IInitializable` still runs in the early `Awake` phase; it does not imply that
other scene components are ready. Startup does not wait for their `Start` methods
or asynchronous work; those dependencies need an explicit readiness contract.

`IsStarted` reports successful root startup. A startup failure shuts down the
registry and rethrows the failure. Manual callers must call `Bootstrap()` before
`Startup()` and ensure their dependencies are ready; both calls are idempotent
within a registry lifetime. `Shutdown()` before Unity `Start` prevents startup.

Construct presenters with explicit dependencies in a project installer/factory.
Keep service resolution at the composition boundary. A scene root owns scene
services; persistent application services need a separately owned persistent root.
There are no UI.Windows, R3 or Quantum dependencies in this package.

Tests: `CompositionRoot.Tests.PlayMode`; enable this package in the consumer
manifest's `testables`. See [installation](https://github.com/vetcat/UiWindows/blob/main/UPM/README.md).

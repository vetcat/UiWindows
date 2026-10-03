# Composition Root

Extracted from vetcat/UiWindows, commit `7b89105`. Assembly: `CompositionRoot.Runtime`.

Add `SceneCompositionRoot` to the scene bootstrap GameObject. Installers implement
`ICompositionInstaller` and register service instances in `Install(IServiceRegistry)`.
Use the ordered serialized installer list for an explicit dependency order; when
that list is empty the root discovers installers on the same GameObject.

The root bootstraps in `Awake` (execution order -10000), installs all services,
then calls `IInitializable.Initialize` in registration order. On destruction or
`Shutdown`, registered `IDisposable` services are disposed in reverse order.
The same object registered under several interfaces participates once.

Construct presenters with explicit dependencies in a project installer/factory.
Keep service resolution at the composition boundary. A scene root owns scene
services; persistent application services need a separately owned persistent root.
There are no UI.Windows, R3 or Quantum dependencies in this package.

Tests: `CompositionRoot.Tests.PlayMode`.

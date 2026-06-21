# Project Architecture Skeleton

This document records the architecture boundary for reusable scene composition and the UI.Windows MVP adapter layer.

## Code Root

All project-owned C# code belongs under `Assets/Scripts`.

Use `Assets/Scripts` as the default search and edit scope for project logic. Keep view assets, prefabs, art, textures, scenes, settings, and other non-code Unity assets outside this tree.

CompositionRoot is reusable infrastructure. It must not depend on UI, MVP, UI.Windows, OpenUI, Zenject, UniRx, or R3.

UiWindowsMvp is the UI.Windows-specific layer. It may depend on `CompositionRoot.Runtime`; `CompositionRoot.Runtime` must not depend on `UiWindowsMvp`.

ProjectContext contains project/application model ports and services that presenters can consume through narrow contracts. Player model code may depend on R3 for read-model state and events, but it must not depend on UI.Windows, Unity UI views, presenter interfaces, OpenUI, Zenject, or UniRx.

## Scene Roles

- `Assets/Scenes/SampleScene.unity` is the canonical runtime/integration scene for UI.Windows MVP vertical slices.
- Runtime launcher, scene installer, `WindowSystem`, and `EventSystem` wiring for vertical slices should be added to `SampleScene` unless a task explicitly requests another runtime scene.
- `Assets/Scenes/Develop/UIDevelopScene.unity` is only the static layout/prefab inspection scene.
- Do not create one runtime demo scene per UI prefab or slice by default. Additional `Assets/Scenes/Develop/*Runtime*` scenes should be exceptional and explicitly justified.

## Folder And Namespace Structure

| Folder | Assembly | Namespace | Ownership |
| --- | --- | --- | --- |
| `Assets/Scripts/CompositionRoot/Runtime` | `CompositionRoot.Runtime` | `CompositionRoot.Runtime` | Scene service/model construction and lifetime. |
| `Assets/Scripts/CompositionRoot/Samples` | `CompositionRoot.Samples` | `CompositionRoot.Samples` | Minimal sample scene services that do not depend on OpenUI or UI.Windows. |
| `Assets/Scripts/CompositionRoot/Tests/PlayMode` | `CompositionRoot.Tests.PlayMode` | `CompositionRoot.Tests.PlayMode` | PlayMode verification for CompositionRoot lifecycle. |
| `Assets/Scripts/ProjectContext/Runtime/Player` | `ProjectContext.Player` | `ProjectContext.Player` | R3-backed player settings, read-model, command ports, XP progression, and service behavior ported from OpenUI without UI/presenter dependencies. |
| `Assets/Scripts/ProjectContext/Runtime/UiRequests` | `ProjectContext.UiRequests` | `ProjectContext.UiRequests` | R3-backed modal, hint, and UI FX request ports/services. No UI.Windows, Unity UI, presenter, DOTween, OpenUI, Zenject, or UniRx dependency. |
| `Assets/Scripts/ProjectContext/Tests/PlayMode` | `ProjectContext.Player.Tests.PlayMode` | `ProjectContext.Player.Tests.PlayMode` | PlayMode/unit verification for player commands, reactive updates, XP progression, level-up events, and subscription disposal. |
| `Assets/Scripts/UiWindowsMvp/Runtime/R3Integration` | `UiWindowsMvp.Reactive` | `UiWindowsMvp.Reactive` | Minimal R3 compile boundary and conventions support. No presenter adapter or model behavior exists in this assembly. |
| `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` | `UiWindowsMvp.UIAdapter` | `UiWindowsMvp.UIAdapter` | UI.Windows presenter lifecycle adapter code: presenter contracts, binding, lifecycle event forwarding, and show-scoped subscription ownership. |

## Ownership Rules

- `SceneCompositionRoot` owns project service/model construction, initialization, and scene-level disposal.
- Services are explicitly constructed by `ICompositionInstaller` implementations; there is no generic DI framework, reflection container, Zenject dependency, or UniRx dependency.
- `ServiceRegistry` initializes `IInitializable` services in registration order and disposes `IDisposable` services once in reverse registration order.
- Failed bootstrap disposes the temporary `ServiceRegistry`, leaves `SceneCompositionRoot` not bootstrapped, and rethrows the original exception.
- UI.Windows owns windows, layouts, loading, unloading, show/hide lifecycle, pooling, and resource management through `WindowSystem.Show` / `ShowSync`, window/handler hide paths, and package lifecycle hooks.
- Presenters own UI behavior and model binding for loaded UI.Windows windows. The initial adapter API is implemented in `UiWindowsMvp.UIAdapter`; see `.agents/skills/uiwindows-mvp-architecture/SKILL.md` and `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md` for current names and lifecycle mapping.
- Project code must not instantiate or destroy UI.Windows windows directly when that would bypass UI.Windows lifecycle, loading, pooling, or resource cleanup.
- Project model services expose narrow read/command ports to presenters and future adapters. They should publish state or request streams instead of depending on UI presenter implementations. Modal, hint, and UI FX requests use `ProjectContext.UiRequests` ports.
- R3 is the deliberate reactive foundation for MVP work. Public ports should prefer read-only reactive surfaces and command methods; mutable subjects/properties stay inside their owning object.
- R3 subscription ownership is still expressed as `IDisposable`. Show-scoped UI subscriptions must be disposed on hide/pool cleanup rather than only on final window deinitialization.
- The reusable pooled UI.Windows MVP lifecycle and verification checklist lives in `docs/uiwindows-mvp-pooling-lifecycle.md`.
- OpenUI migration rules for future modal, hint, FX, and request-port work live in `docs/uiwindows-mvp-openui-migration-guide.md`.

## CompositionRoot Mechanics

`SceneCompositionRoot` is intended to be reusable for any scene. Scene-specific behavior belongs in one or more `ICompositionInstaller` components, not in `SceneCompositionRoot` subclasses or UI-specific code.

Startup sequence:

1. Unity calls `SceneCompositionRoot.Awake()`.
2. `Bootstrap()` creates a temporary `ServiceRegistry`.
3. `SceneCompositionRoot` collects installers from the serialized list, or from the same GameObject when the list is empty.
4. Each installer registers concrete services and models into the registry.
5. `ServiceRegistry.InitializeAll()` calls `IInitializable.Initialize()` in registration order.
6. The root stores the initialized registry only after successful initialization.

Shutdown sequence:

1. Unity calls `SceneCompositionRoot.OnDestroy()`, or project code calls `Shutdown()`.
2. `ServiceRegistry.Dispose()` disposes registered `IDisposable` services once in reverse registration order.
3. The root clears its registry reference and is no longer bootstrapped.

Failure rule:

If installer execution or service initialization throws, `SceneCompositionRoot` disposes the temporary registry, keeps `IsBootstrapped == false`, does not expose the failed registry through `Services`, and rethrows the original exception.

## Bootstrap Path

A scene bootstrap GameObject should contain:

1. `SceneCompositionRoot`
2. One or more `MonoBehaviour` components implementing `ICompositionInstaller`

If the serialized installer list on `SceneCompositionRoot` is empty, installers on the same GameObject are discovered in Unity component order. For stricter ordering, populate the serialized installer list explicitly.

`Assets/Scripts/CompositionRoot/Samples/SampleCompositionInstaller.cs` demonstrates this path by registering a simple model and service without OpenUI, Zenject, UniRx, UI.Windows, or MVP dependencies.

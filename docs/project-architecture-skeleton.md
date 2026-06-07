# Project Architecture Skeleton

This document records the `UIW-3` architecture boundary for reusable scene composition and the future UI.Windows MVP adapter layer.

## Code Root

All project-owned C# code belongs under `Assets/Scripts`.

CompositionRoot is reusable infrastructure. It must not depend on UI, MVP, UI.Windows, OpenUI, Zenject, or UniRx.

UiWindowsMvp is the UI.Windows-specific layer. It may depend on `CompositionRoot.Runtime`; `CompositionRoot.Runtime` must not depend on `UiWindowsMvp`.

## Folder And Namespace Structure

| Folder | Assembly | Namespace | Ownership |
| --- | --- | --- | --- |
| `Assets/Scripts/CompositionRoot/Runtime` | `CompositionRoot.Runtime` | `CompositionRoot.Runtime` | Scene service/model construction and lifetime. |
| `Assets/Scripts/CompositionRoot/Samples` | `CompositionRoot.Samples` | `CompositionRoot.Samples` | Minimal sample scene services that do not depend on OpenUI or UI.Windows. |
| `Assets/Scripts/CompositionRoot/Tests/PlayMode` | `CompositionRoot.Tests.PlayMode` | `CompositionRoot.Tests.PlayMode` | PlayMode verification for CompositionRoot lifecycle. |
| `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` | `UiWindowsMvp.UIAdapter` | `UiWindowsMvp.UIAdapter` | Reserved for future UI.Windows presenter adapter code. No presenter adapter exists in `UIW-3`. |

## Ownership Rules

- `SceneCompositionRoot` owns project service/model construction, initialization, and scene-level disposal.
- Services are explicitly constructed by `ICompositionInstaller` implementations; there is no generic DI framework, reflection container, Zenject dependency, or UniRx dependency.
- `ServiceRegistry` initializes `IInitializable` services in registration order and disposes `IDisposable` services once in reverse registration order.
- Failed bootstrap disposes the temporary `ServiceRegistry`, leaves `SceneCompositionRoot` not bootstrapped, and rethrows the original exception.
- UI.Windows owns windows, layouts, loading, unloading, show/hide lifecycle, pooling, and resource management through `WindowSystem.Show/Hide` and package lifecycle hooks.
- Future presenters will own UI behavior and model binding for loaded UI.Windows windows. Presenters are intentionally not implemented in `UIW-3`.
- Project code must not instantiate or destroy UI.Windows windows directly when that would bypass UI.Windows lifecycle, loading, pooling, or resource cleanup.

## Bootstrap Path

A scene bootstrap GameObject should contain:

1. `SceneCompositionRoot`
2. One or more `MonoBehaviour` components implementing `ICompositionInstaller`

If the serialized installer list on `SceneCompositionRoot` is empty, installers on the same GameObject are discovered in Unity component order. For stricter ordering, populate the serialized installer list explicitly.

`Assets/Scripts/CompositionRoot/Samples/SampleCompositionInstaller.cs` demonstrates this path by registering a simple model and service without OpenUI, Zenject, UniRx, UI.Windows, or MVP dependencies.

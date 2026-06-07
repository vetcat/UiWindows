# Project Architecture Skeleton

This document records the `UIW-3` architecture boundary for the local MVP layer on top of UI.Windows.

## Folder And Namespace Structure

| Folder | Assembly | Namespace | Ownership |
| --- | --- | --- | --- |
| `Assets/UiWindowsMvp/Runtime/Composition` | `UiWindowsMvp.Runtime` | `UiWindowsMvp.Runtime.Composition` | Scene service/model construction and lifetime. |
| `Assets/UiWindowsMvp/Runtime/UIAdapter` | `UiWindowsMvp.UIAdapter` | `UiWindowsMvp.UIAdapter` | Reserved for future UI.Windows presenter adapter code. No presenter adapter exists in `UIW-3`. |
| `Assets/UiWindowsMvp/Samples/CompositionRootSample` | `UiWindowsMvp.Samples.CompositionRoot` | `UiWindowsMvp.Samples.CompositionRootSample` | Minimal sample scene services that do not depend on OpenUI. |
| `Assets/UiWindowsMvp/Tests/PlayMode` | `UiWindowsMvp.Tests.PlayMode` | `UiWindowsMvp.Tests.PlayMode` | PlayMode verification for scene bootstrap lifecycle. |

## Ownership Rules

- `SceneCompositionRoot` owns project service/model construction, initialization, and scene-level disposal.
- Services are explicitly constructed by `ICompositionInstaller` implementations; there is no generic DI framework, reflection container, Zenject dependency, or UniRx dependency.
- `ServiceRegistry` initializes `IInitializable` services in registration order and disposes `IDisposable` services once in reverse registration order.
- UI.Windows owns windows, layouts, loading, unloading, show/hide lifecycle, pooling, and resource management through `WindowSystem.Show/Hide` and package lifecycle hooks.
- Future presenters will own UI behavior and model binding for loaded UI.Windows windows. Presenters are intentionally not implemented in `UIW-3`.
- Project code must not instantiate or destroy UI.Windows windows directly when that would bypass UI.Windows lifecycle, loading, pooling, or resource cleanup.

## Bootstrap Path

A scene bootstrap GameObject should contain:

1. `SceneCompositionRoot`
2. One or more `MonoBehaviour` components implementing `ICompositionInstaller`

If the serialized installer list on `SceneCompositionRoot` is empty, installers on the same GameObject are discovered in Unity component order. For stricter ordering, populate the serialized installer list explicitly.

`Assets/UiWindowsMvp/Samples/CompositionRootSample/SampleCompositionInstaller.cs` demonstrates this path by registering a simple model and service without OpenUI, Zenject, or UniRx.

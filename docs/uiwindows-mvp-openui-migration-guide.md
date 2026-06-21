# UI.Windows MVP OpenUI Migration Guide

This guide records the final migration rules proven by the incremental OpenUI ports through `UIW-11`.

## Source Of Truth

UI.Windows owns window loading, showing, hiding, pooling, layouts, and resource cleanup. Project code opens windows through `WindowSystem.Show` or narrow launchers that delegate to it. Project code closes windows through the UI.Windows hide path. Do not copy OpenUI's `GameObject.SetActive` lifecycle model.

OpenUI remains a behavior reference. Do not import OpenUI infrastructure, Zenject installers, UniRx subscriptions, signal bus wiring, prefab factories, or presenter-to-presenter dependencies.

## Composition Boundary

`CompositionRoot.Runtime` stays reusable and UI-free. Scene installers construct services, request ports, presenter factories, and launchers explicitly.

`ProjectContext` owns model/application ports and services. It may expose R3 read-only state and request streams, but it must not reference UI.Windows windows, Unity UI views, presenter implementations, DOTween, OpenUI, Zenject, or UniRx.

`UiWindowsMvp` owns UI.Windows-specific views, windows, presenters, launchers, and rendering effects. DOTween is allowed here for UI/effects rendering only.

## Request Ports

`ProjectContext.UiRequests` is the project-owned request boundary for modal, hints, and UI FX flows:

- `IUiModalReadModel` exposes `CurrentModal` as read-only R3 state.
- `IUiModalCommands` requests info OK, info OK/Cancel, wait, close, and completion.
- `IUiFeedbackReadModel` exposes hint and FX request streams.
- `IUiFeedbackCommands` publishes hint requests and collect/spend FX requests.

Requests contain stable data such as caption, description, kind, amount, target, anchor, and duration. They do not expose DOTween, UI.Windows, Unity UI, presenter, or OpenUI types.

`PlayerService` may publish representative coin collect/spend FX through `IUiFeedbackCommands`, but the rendering remains in `UiWindowsMvp`.

## Presenter And R3 Rules

Presenters bind to concrete UI.Windows window wrappers after `WindowSystem.Show` has produced a window instance. Use `WindowPresenterBinder` once per pooled window instance.

Presenter show-scoped subscriptions belong in `IUiShowScope`:

- R3 model/read-model subscriptions.
- Modal, hint, and FX request stream subscriptions.
- Unity UI button/listener registrations.
- Timers, frame streams, and temporary animation state tied to one visible show.

`OnHideBegin` or `OnHideEnd` must clear show-scoped view state. `OnDeInitialized` remains final idempotent cleanup. Do not rely on final disposal for pooled-window per-show cleanup.

## Modal Pattern

`UiModalService` owns current modal request state. `UiModalDemoLauncher` subscribes to that state and opens or hides `UiModalWindow` through UI.Windows. `UiModalPresenter` renders the current request into `UiModalView` and completes the request through `IUiModalCommands`.

Info OK, info OK/Cancel, and wait states share one UI.Windows modal window and view mode. This keeps lifecycle and pooling behavior simple while preserving the OpenUI behavior surface.

## Hints And FX Pattern

Hints and FX are lightweight UI.Windows overlay windows. They are opened empty by the scene installer so their presenters can receive transient R3 request streams without buffering. Their content appears only when a request arrives.

`UiHintsPresenter` subscribes to `IUiFeedbackReadModel.HintRequests` in show scope and renders requests through `UiHintsView`.

`UiFxPresenter` subscribes to `IUiFeedbackReadModel.FxRequests` in show scope and renders collect/spend sequences through `UiFxView`.

Hidden overlays must not consume request streams. Reopened pooled overlays reuse the same window instance and presenter binding, but get a fresh show scope.

## DOTween Cleanup

DOTween sequences created by views or presenters must be killed or completed on hide, pool return, and final disposal. `UiHintsView.ClearHint()` kills the active hint sequence. `UiFxView.StopAllFx()` kills active FX sequences and returns active items to the local pool.

DOTween types must not appear in `Assets/Scripts/ProjectContext` public ports or services.

## Prefabs And Scenes

Project-owned UI.Windows view prefabs live under `Assets/Prefabs/UiWindowsMvp`. Supporting non-code assets live under `Assets/Content/UiWindowsMvp`.

`Assets/Scenes/SampleScene.unity` is the runtime integration scene for UI.Windows MVP slices. `Assets/Scenes/Develop/UIDevelopScene.unity` remains a static layout/prefab inspection scene and does not prove UI.Windows lifecycle behavior.

Future ports should add focused PlayMode coverage for presenter behavior plus real `WindowSystem.Show -> Hide -> Reopen` cycles when pooling or show-scoped subscriptions are involved.

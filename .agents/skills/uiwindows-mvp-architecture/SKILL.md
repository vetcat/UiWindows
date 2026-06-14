---
name: uiwindows-mvp-architecture
description: Project-specific UI.Windows MVP architecture guidance for the UiWindows Unity project. Use when designing, implementing, or reviewing UI.Windows MVP presenters, views, model/read-model ports, R3 UI bindings, show/hide subscription lifetimes, OpenUI example ports, or decisions about whether UI logic belongs in a view, presenter, model, service, controller, save adapter, ECS adapter, or navigation/effect port.
---

# UI.Windows MVP Architecture

## Purpose

Use this skill to keep project UI architecture consistent across implementation chats. It captures the project-wide MVP interpretation and the current UI.Windows presenter adapter API.

Treat this as architecture guidance. `UIW-5` established the initial adapter API names and lifecycle details below.

## Current Adapter API

The minimal project-owned adapter lives under `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` in assembly `UiWindowsMvp.UIAdapter`.

- `IUiPresenter` defines presenter initialization, show/hide hooks, and final `IDisposable` cleanup.
- `IWindowPresenter<TWindow>` binds a presenter to a concrete `WindowBase` subtype.
- `IWindowPresenterFactory<TWindow>` creates presenters with explicit constructor dependencies or narrow ports.
- `WindowPresenterBinder.Bind(window, factory)` attaches one presenter binding to a UI.Windows window instance, normally from a `WindowSystem.Show` or `WindowSystem.ShowSync` callback.
- `WindowPresenterBinding<TWindow>` owns presenter lifecycle forwarding and idempotent final cleanup.
- `IUiShowScope` / `WindowPresenterShowScope` own show-scoped `IDisposable` subscriptions.

Verified lifecycle mapping:

- `WindowEvent.OnInitialized` maps to `IUiPresenter.Initialize()`.
- `WindowEvent.OnShowBegin` creates a new show scope and calls `IUiPresenter.OnShowBegin(scope)`.
- `WindowEvent.OnShowEnd` maps to `IUiPresenter.OnShowEnd()`.
- `WindowEvent.OnHideBegin` maps to `IUiPresenter.OnHideBegin()`.
- `WindowEvent.OnHideEnd` maps to `IUiPresenter.OnHideEnd()`, then disposes the show scope.
- `WindowEvent.OnDeInitialized` disposes the binding and presenter idempotently.

The adapter uses `IDisposable` as the runtime subscription boundary rather than depending directly on R3. R3 subscriptions are expected to be added to `IUiShowScope` and are therefore disposed on hide completion or final cleanup.

## Load First

Before making UI MVP decisions, read the local project instructions and these docs if they are relevant to the change:

- `AGENTS.md`
- `_bmad-output/project-context.md`
- `docs/project-architecture-skeleton.md`
- `docs/r3-mvp-conventions.md`
- `docs/uiwindows-mvp-pooling-lifecycle.md`

## Core Ownership Model

- UI.Windows owns window creation, loading, unloading, layouts, focus, depth, pooling, resource cleanup, and show/hide lifecycle.
- Project MVP code adapts to UI.Windows lifecycle; it must not replace or bypass it.
- `WindowSystem.Show` and `WindowSystem.ShowSync` are the valid opening boundary. Closing should use the UI.Windows-provided window/handler hide path or a narrow project-owned wrapper around it, not direct GameObject activation.
- A view is a UI.Windows window/component object with serialized Unity UI references and local presentation helpers.
- A presenter owns UI behavior for one view/window instance: field binding, button handling, animations, view-specific reactive subscriptions, and calls into explicit model/service ports.
- A model boundary may be a save object, runtime service, controller, ECS adapter, settings provider, localization provider, or a combination of those. Presenters should see this through explicit ports, not through a global resolver.
- CompositionRoot owns scene-level service/model construction and disposal; it must remain independent from UI.Windows, MVP, OpenUI, Zenject, UniRx, and R3 unless a future explicit architecture decision changes that boundary.

## MVP Boundary Rules

- Prefer concrete presenter-to-view typing, for example a presenter bound to a specific UI.Windows window/view type.
- Let presenters know view fields when that is pragmatic for Unity UI, including buttons, labels, sliders, animation roots, and layout components.
- Keep domain/model services independent from presenter implementations.
- Do not inject `IServiceResolver`, Zenject `DiContainer`, or concrete ECS world/store/root objects into presenters, models, or domain services.
- Prefer narrow ports such as `IPlayerReadModel`, `IPlayerCommands`, `IWindowNavigator`, `IUiEffectRequests`, `ILocalizationReadModel`, or equivalent names.
- Expose read-only R3 state from model/read-model ports, normally `ReadOnlyReactiveProperty<T>` for current state and `Observable<T>` for events/requests.
- Keep mutable R3 primitives such as `ReactiveProperty<T>` and `Subject<T>` private to the owning object.
- Route mutations through command methods such as `SetName`, `AddCoins`, `SelectItem`, `RequestClose`, or equivalent operations.

## Lifecycle Rules

- Bind presenters only after UI.Windows has created or loaded the window instance.
- Map presenter lifecycle onto UI.Windows hooks such as `OnInit`, `OnShowBegin`, `OnShowEnd`, `OnHideBegin`, `OnHideEnd`, and `OnDeInit`.
- Create show-scoped UI subscriptions on show or bind-show, depending on the verified adapter API.
- Dispose show-scoped subscriptions on hide completion or pool return. Do not wait for final deinit for pooled windows.
- Use final disposal for long-lived presenter resources and make it idempotent.
- Use `IDisposable` as the common subscription ownership boundary.
- Use R3 `AddTo(Component)` only for subscriptions whose lifetime should truly match Unity object destruction, not as a replacement for show/hide cleanup.

## Pooling Rules

- Pooled UI.Windows window instances keep one presenter binding per pooled window instance. Do not call `WindowPresenterBinder.Bind` again on reopen when `WindowPresenterBinder.TryGetBinding` says the active binding already exists.
- Each `OnShowBegin` must create a fresh `IUiShowScope`. `OnHideEnd` must dispose it before pool return, so R3 model subscriptions, button handlers, timers, frame streams, and other show-scoped state cannot survive hidden state.
- Final cleanup belongs to `OnDeInitialized` and explicit UI.Windows cleanup paths such as `WindowSystem.Clean`; it remains idempotent and is not a per-show reset substitute.
- Do not solve lifecycle bugs with direct `GameObject.SetActive`, presenter recreation on every pooled reopen, or custom window ownership outside UI.Windows.
- `LayoutWindowType` pooling can clear cached `componentInstance` while the layout instance remains reused. Wrapper code should tolerate that, for example by trying `GetLayoutComponent` first and then a narrow `FindComponent<T>` fallback, instead of patching UI.Windows internals without a verified compatibility issue.
- For the full rule/checklist, read `docs/uiwindows-mvp-pooling-lifecycle.md`.

## OpenUI Porting Rules

- Use OpenUI as a behavior and test reference, not as infrastructure to copy wholesale.
- Do not copy OpenUI's mandatory Zenject or UniRx setup.
- Do not copy `UiView.Show/Hide` semantics based on `gameObject.SetActive` as the project lifecycle mechanism.
- Do not copy Zenject prefab binding patterns such as `DiContainerUiExtensions.BindViewPresenter` directly.
- Preserve useful behavior patterns: presenters bind view fields, react to model changes, handle button events, coordinate view-specific animations, and request UI effects through ports.
- Replace OpenUI presenter-to-presenter or service-to-presenter coupling with explicit navigation/effect/read-model/command ports where practical.

## Design Checklist

When designing or reviewing a UI MVP change, verify:

- Opening and closing still goes through UI.Windows or a narrow wrapper around it.
- Presenter dependencies are explicit and narrow.
- Model/read-model mutable state does not leak across boundaries.
- Show-scoped subscriptions cannot duplicate across hide/show or pool reuse.
- If pooling is expected, reopen reuses the same window instance and presenter binding, while using a new show scope.
- PlayMode coverage exercises `WindowSystem.Show -> Hide -> reopen` cycles and detects duplicate model subscriptions, duplicate button handlers, stale show-scoped state, and idempotent final cleanup.
- Final cleanup is safe to call once or repeatedly.
- `CompositionRoot.Runtime` remains independent from UI, UI.Windows, MVP, R3, OpenUI, Zenject, and UniRx.
- The change is small enough for the current Linear issue and does not import broad OpenUI infrastructure.

## Tooling Checklist

When changing presenter, view, model-port, or R3 UI binding C# files:

- Use Rider `get_file_problems` on changed `.cs` files when Rider MCP is available.
- Use Rider `build_solution` after C# changes when practical, or report why Unity compile was used instead.
- Use Rider `rename_refactoring` for programmatic symbol renames.
- Use Unity MCP for Unity editor refresh/compile, PlayMode verification, and live UI.Windows API reflection.

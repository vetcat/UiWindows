# UI.Windows MVP Pooling Lifecycle

This document records the reusable lifecycle rule proven by `UIW-8` for future UI.Windows MVP windows.

## Source Of Truth

UI.Windows owns window creation, loading, layout, show/hide transitions, pooling, and resource cleanup.

Open windows through `WindowSystem.Show` / `WindowSystem.ShowSync` or a narrow project wrapper that delegates to those APIs. Close windows through the UI.Windows window or handler `Hide` path. Do not solve pooling by calling `GameObject.SetActive`, manually destroying window instances, or bypassing UI.Windows lifecycle events.

## Presenter Binding Rule

Bind a presenter after UI.Windows has produced a window instance.

For non-pooled windows, this normally means one binding for one loaded window lifetime.

For pooled windows, keep one presenter binding for the pooled window instance. Reopening the same pooled instance must not call `WindowPresenterBinder.Bind` again while an active binding is already anchored on that instance. Rebind only when UI.Windows has produced a different window instance without an active binding.

The binding lifetime and show lifetime are different:

- Binding lifetime: from first bind until `OnDeInitialized`, `WindowSystem.Clean`, or explicit binding disposal.
- Show lifetime: from `OnShowBegin` until `OnHideEnd`.
- `OnShowBegin` creates a fresh `IUiShowScope`.
- `OnHideEnd` disposes the active `IUiShowScope` before the window returns to the pool.
- Final cleanup remains idempotent and belongs to `OnDeInitialized` / `WindowSystem.Clean`.

## Show-Scoped State

Put these in `IUiShowScope`:

- R3 subscriptions from model/read-model ports to view fields.
- Unity UI button listeners and value-change handlers.
- R3 timers, frame streams, throttles, or other streams that should only run while the window is visible.
- Temporary animation or view-state subscriptions tied to one visible show.

Do not use final `OnDeInitialized` cleanup as the first cleanup point for show-scoped state. A pooled window may hide and reopen many times without being deinitialized.

Use `AddTo(Component)` only when the intended lifetime is Unity object destruction. It is not a replacement for hide/pool cleanup.

## LayoutWindowType Pooling Caveat

UI.Windows `LayoutWindowType` can keep a layout instance while clearing cached layout component references during pool return. In `UIW-8`, this showed up around `LayoutItem.PushToPool()` clearing `componentInstance`.

Project window wrappers should tolerate pool reuse. The reference pattern is `UiTopLeftWindow.TryGetView`: first use `GetLayoutComponent`, then fall back to `FindComponent<TView>()` on the window hierarchy. Prefer a narrow wrapper fix over modifying UI.Windows internals unless a verified package compatibility issue blocks progress.

## Verification Pattern

For each future pooled MVP window, add focused PlayMode coverage that follows the real UI.Windows lifecycle.

Reference test: `UiTopLeftWindowLifecycleTests.ReopenCyclesThroughWindowSystem_ReusePooledWindowWithoutDuplicateSubscriptions`.

Minimum verification sequence:

1. Load `Assets/Scenes/SampleScene.unity` or the canonical runtime scene for the slice.
2. Open the window through `WindowSystem.Show` or the project wrapper that delegates to it.
3. Assert the window source is pooled when pooling is part of the slice.
4. Capture the window instance id, presenter binding, and view reference.
5. Trigger one UI interaction and verify exactly one command/model effect.
6. Hide through UI.Windows `Hide`.
7. While hidden, mutate the model and verify stale visible UI text does not update.
8. While hidden, invoke the previous button reference and verify no command effect occurs.
9. Reopen through UI.Windows.
10. Assert pooled reopen uses the same window instance and the same presenter binding.
11. Repeat the cycle more than once.
12. Finish with `WindowSystem.Clean` or equivalent UI.Windows cleanup and assert binding disposal remains idempotent.

Behavioral checks are preferred over making R3 diagnostics a runtime requirement. `ObservableTracker` may be useful while debugging, but the project should prove correctness through lifecycle counters and user-observable effects.

## Do Not Copy

- Do not copy OpenUI `gameObject.SetActive` show/hide semantics as the lifecycle model.
- Do not rebind presenters on every pooled reopen.
- Do not keep button listeners outside the show scope unless their lifetime is intentionally the whole window instance.
- Do not expose mutable R3 primitives from model/read-model ports to presenters.
- Do not modify UI.Windows package internals for pooling behavior until a verified compatibility issue leaves no narrower project-owned fix.

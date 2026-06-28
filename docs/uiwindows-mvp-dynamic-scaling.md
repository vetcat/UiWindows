# UI.Windows MVP Dynamic Collection And Indicator Scaling

Date: 2026-06-28
Issue: `UIW-31` - `04 - Scale dynamic collections and object indicator update paths`
Parent: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`

## Purpose

This document records the first production-hardening decision for the migrated shop
collection and object indicator update paths.

The goal is to reduce avoidable work in the current implementation while preserving
the accepted `UIW-1` architecture:

- UI.Windows owns window lifecycle, loading, pooling, and resource cleanup.
- Presenters own UI behavior and show-scoped subscriptions.
- `ProjectContext` stays independent from Unity UI, UI.Windows, presenters,
  DOTween, OpenUI, Zenject, and UniRx.
- R3 remains the reactive foundation for UI-layer streams and frame updates.

No Android or iOS build was produced for this issue. All evidence is Editor/test
evidence or static inspection, not target-device proof.

## Shop Collection Decision

The current shop remains a full materialized, pool-backed `ScrollRect` collection.
That is sufficient for the migrated SampleScene catalog and moderate shop groups
because rows are simple, selection changes are event-driven, and row objects are
pooled.

`UIW-31` does not add identity diffing or virtualization. Those would add more
state and edge cases than the current demo data justifies. The next production step
for very large catalogs should be a proper visible-row virtualization layer, not a
larger presenter rebuild method.

The implemented change makes the existing pool count-aware:

- same-size rebuilds keep the same active row objects by index;
- shrinking a list releases only surplus rows to the pool;
- growing a list keeps existing rows and rents only missing rows from the pool;
- a null data source still releases all active rows.

This reduces avoidable parent changes, canvas group changes, and pool stack churn
when the selected group refreshes with the same item count. Presenter button
bindings remain show-scoped and are still cleared before each row rebind.

`UiShopView.ScrollToTop()` no longer calls global `Canvas.ForceUpdateCanvases()`.
It now forces layout only for the shop scroll content before resetting the vertical
scroll position.

## Object Indicator Decision

Object indicators now use a UI-layer update source:

- `IUiObjectIndicatorUpdateSource` is the presenter-facing port.
- `R3UiObjectIndicatorUpdateSource` owns the single R3 `Observable.EveryUpdate()`
  subscription.
- `UiObjectIndicatorPresenter` registers `UpdateIndicatorPosition` only in show
  scope and unregisters on hide/pool cleanup through the existing show-scope
  disposal path.
- `UiTopLeftDemoInstaller` creates and registers one update source for the scene and
  injects it through `UiObjectIndicatorPresenterFactory`.

The decision is to share one frame stream across visible indicators instead of
creating one `EveryUpdate` subscription per shown indicator presenter.

This issue does not add culling, throttling, or a full multi-target indicator
manager. Those should be added only when a production scene has multiple real
targets and measured Editor or target-device evidence shows that every-frame
screen-position updates are too expensive. If that happens, keep the scheduler in
`UiWindowsMvp`; do not move per-frame UI scheduling into `CompositionRoot.Runtime`
or `ProjectContext`.

## Verification Evidence

Focused tests added or extended for this issue:

- `UiShopPresenterTests.View_RebuildShopItems_PreservesActiveRowsAndPoolsOnlySurplusForLargeLists`
  simulates 32 shop rows and verifies same-size rebuilds preserve active row
  instances while shrink/grow cycles use the pool.
- `UiObjectIndicatorPresenterTests.Presenters_ShareUpdateSourceAndUnregisterHiddenIndicators`
  simulates two visible indicator presenters sharing one update source, then hides
  one and verifies only the still-visible presenter receives update ticks.
- Existing presenter and lifecycle tests continue to cover hidden pooled windows,
  button handler cleanup, reward requests, target registration cleanup, and pooled
  reopen behavior.

## Mobile Interpretation

The shop change should reduce avoidable warm-path churn for moderate full-list
rebuilds. The object-indicator change should reduce frame-stream subscription count
from one per visible indicator to one per scene update source.

This is mobile inference only. It is not evidence that a low-end Android device can
handle a large shop catalog or many world indicators at 60 FPS. Deferred evidence
still includes Android cold first-show latency, object-indicator frame cost,
dynamic collection layout cost, managed allocations, and frame pacing under a real
target-device build.

## Remaining Risks

- The shop still materializes every row in the selected group. Large production
  catalogs need virtualization once real content size or profiling justifies it.
- The shop still uses legacy `Text`, layout groups, and a `ContentSizeFitter` in the
  scroll content.
- Object indicators still update visible positions every frame. Culling/throttling
  remains deferred until multiple real targets and profiling evidence exist.
- `UiFxTarget.CharacterReward` remains a single semantic FX source target. A future
  multi-indicator reward design may need source-specific FX routing if several
  reward buttons can be clicked simultaneously.
- Target-device Android/iOS profiling remains intentionally deferred under `UIW-26`.

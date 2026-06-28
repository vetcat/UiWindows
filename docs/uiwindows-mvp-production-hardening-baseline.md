# UI.Windows MVP Production Hardening Baseline

Date: 2026-06-28
Issue: `UIW-27` - `00 - Establish production hardening baseline and mobile readiness targets`
Parent: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`

## Purpose

This document records the starting evidence for the `UIW-26` production hardening and
mobile readiness iteration. It is a baseline, not an optimization pass.

`UIW-1` is already accepted. The goal here is to define what must be measured or
cleaned up before broad production changes are made, while preserving the accepted
architecture:

- UI.Windows owns lifecycle, loading, pooling, layouts, and resource cleanup.
- Project-owned presenters own view behavior and show-scoped subscriptions.
- `ProjectContext` ports stay independent from UI.Windows, Unity UI, presenters,
  DOTween, OpenUI, Zenject, and UniRx.
- R3 remains the reactive foundation.

## Mobile Readiness Inputs

These inputs came from the `UIW-26` planning handoff and are treated as constraints:

- Primary target class: low-end Android devices.
- Runtime target: 60 FPS, using 16.67 ms as the total frame budget context.
- Initial baseline mode: Editor and Unity Profiler evidence only.
- Target-device builds and target-device profiling are intentionally deferred.
- Baseline evidence must separate measured Editor facts from mobile inference.
- Highest mobile risks to track: cold startup and cold first-show speed on low-end
  Android devices.

## Evidence Classes

| Evidence class | Meaning |
| --- | --- |
| Measured Editor evidence | Captured in the local Unity Editor session on 2026-06-28. Useful for trend and regression checks, not proof of low-end Android behavior. |
| Static inspection | Repository, prefab, scene, and code scans. Useful for inventory and risk prioritization. |
| Mobile inference | A reasoned risk based on known Unity mobile constraints. Must be verified on device later. |
| Deferred device evidence | Evidence intentionally unavailable for this task. Requires later Android device build/profiling authorization. |

## Measurement Environment

Measured Editor evidence came from Unity MCP and Unity Profiler tools in the local
Editor session:

- Unity instance: `UiWindows@29793614097f6f61`.
- Unity version: `6000.4.4f1`.
- Editor platform: `OSXEditor`.
- Active build target during measurement: `StandaloneOSX`.
- Active scene: `Assets/Scenes/SampleScene.unity`.
- Editor state before measurement: idle, not playing, not compiling, no domain reload
  pending.
- Profiler recording: started and stopped around the focused integrated PlayMode test
  with CPU, Rendering, Memory, UI, and UI Details areas enabled. The raw recording was
  written outside the repository at `/tmp/uiw-27-samplescene-profile.raw` and is not a
  committed artifact.

No Android or iOS build was produced. No target-device profiler capture exists yet.

## Measured Editor Evidence

### Prefab UI Inventory

Unity Editor prefab inspection over the 11 migrated view prefabs under
`Assets/Prefabs/UiWindowsMvp/SampleSceneWindows` returned:

| Prefab | GameObjects | Graphics | Raycast targets | Legacy Text | BestFit Text | LayoutGroups | ContentSizeFitters | Buttons | Toggles | Sliders | ScrollRects | CanvasGroups |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `UiTopLeftView` | 35 | 22 | 5 | 9 | 7 | 5 | 1 | 4 | 0 | 2 | 0 | 0 |
| `UiTopRightView` | 5 | 3 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| `UiTopCenterView` | 6 | 4 | 1 | 2 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| `UiDownLeftView` | 5 | 2 | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0 |
| `UiDownRightView` | 5 | 3 | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0 |
| `UiSettingsView` | 35 | 20 | 12 | 8 | 0 | 9 | 0 | 1 | 2 | 2 | 0 | 2 |
| `UiShopView` | 22 | 15 | 8 | 7 | 0 | 8 | 1 | 1 | 0 | 0 | 1 | 2 |
| `UiModalView` | 13 | 10 | 5 | 5 | 0 | 3 | 0 | 4 | 0 | 0 | 0 | 3 |
| `UiHintsView` | 4 | 3 | 2 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 |
| `UiFxView` | 7 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 |
| `UiObjectIndicatorView` | 11 | 8 | 5 | 3 | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 1 |
| **Total** | **148** | **90** | **41** | **38** | **9** | **25** | **2** | **13** | **2** | **5** | **1** | **10** |

Baseline interpretation:

- The largest static UI surfaces are `UiTopLeftView`, `UiSettingsView`, and
  `UiShopView`.
- The highest raycast-target counts are `UiSettingsView` with 12, `UiShopView` with 8,
  and `UiTopLeftView`, `UiModalView`, and `UiObjectIndicatorView` with 5 each.
- Legacy `Text` remains the current text system. BestFit appears in `UiTopLeftView`,
  `UiTopRightView`, and `UiTopCenterView`.
- `UiFxView` has no static `Graphic` components in the prefab because FX items are
  request-created and pooled at runtime.

### SampleScene Workflow

Focused PlayMode verification:

- Test assembly: `UiWindowsMvp.Tests.PlayMode`.
- Test:
  `UiWindowsMvp.Tests.PlayMode.SampleSceneIntegratedAcceptanceTests.SampleScene_RunIntegratedMigratedWorkflowAndRepresentativeShowScopeCleanup`.
- Result: 1 passed, 0 failed, 0 skipped.
- Unity-reported test duration: 1.6582813 seconds.
- Job wall time: about 7.6 seconds from job start to finish.
- `Assets/Scenes/SampleScene.unity` validation: clean, 0 issues, 0 missing scripts,
  0 broken prefabs.
- Unity Console error query after the run returned 0 error entries. The Console still
  contained Test Runner tooling noise: a result-save log entry typed as `Exception`
  and one `Unity.PerformanceTesting.Editor.TestRunBuilder` cleanup warning.

Profiler counter snapshot after the focused run:

| Counter | Value | Confidence |
| --- | ---: | --- |
| Memory `Game Object Count` | 28 | Low for runtime UI count because this was sampled after the PlayMode test completed. |
| Memory `Scene Object Count` | 550 | Low for runtime UI count because this includes Editor/test state and profiler overhead. |
| Memory `GC Allocated In Frame` | 1,664 bytes | Low for workflow allocation because this is a post-run frame sample, not a marker-bounded first-show capture. |
| Memory `GC Allocation In Frame Count` | 23 | Low for workflow allocation for the same reason. |
| Memory `Total Used Memory` | 3,718,023,962 bytes | Editor-process only, not target player memory. |
| Memory `Profiler Used Memory` | 800,051,944 bytes | Profiler overhead, useful only to explain why total Editor memory is not a mobile baseline. |
| UI `TextRendering.Render` | 0 ns | Post-run frame sample, not a steady gameplay UI frame. |
| UI `TextCoreRendering.Render` | 0 ns | Post-run frame sample, not a steady gameplay UI frame. |

Frame timing was available, but the captured `cpu_frame_time_ms` sample was
199.111333 ms after PlayMode/test transition. That value is recorded as an Editor
transition artifact and must not be used as a steady-frame or mobile target.

## Static Inspection Evidence

### Runtime Window First-Show Paths

`Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows/UiRuntimeWindowSource.cs`
creates runtime UI.Windows source objects on first launcher show:

- source `GameObject` plus `RectTransform`;
- `WindowLayout` source;
- `Canvas`, `CanvasScaler`, and `GraphicRaycaster`;
- disabled source camera;
- direct component `Resource`;
- `createPool = true`;
- `preferences.singleInstance = true`;
- `preferences.forceSyncLoad = true`;
- `InitialParameters.showSync = true` from launchers.

The pre-consolidation static scan found 11 `*DemoLauncher.cs` files using the same
broad `runtimeSource ??=`, `WindowSystem.Show`, `WindowPresenterBinder.TryGetBinding`,
and `WindowPresenterBinder.Bind` pattern. The current code routes that shared path
through `UiRuntimeWindowHandle.cs`.

Mobile inference:

- Lazy first-show creation is appropriate for small demos, but it is a cold first-show
  risk on low-end Android when many windows are first opened in one session.
- `forceSyncLoad` and `showSync` are predictable but should be paired with an explicit
  prewarm/first-show policy before production-scale window growth.

### Launcher And Runtime Source Duplication

The pre-consolidation scan found repeated runtime-source wrappers plus direct generic
source use in modal, hints, and FX launchers. The durable runtime source pattern is now:

- `UiRuntimeWindowSource.cs` creates pooled UI.Windows runtime source objects.
- `UiRuntimeWindowHandle.cs` owns the repeated lazy source creation, synchronous
  `WindowSystem.Show`, immediate hide transition, and guarded presenter binding path.
- Concrete launchers keep special behavior explicit, such as modal request state,
  down-left shop visibility, object-indicator targeting, and feedback overlays.

Future performance measurement should still verify that source consolidation preserves
pooling and first-show behavior.

### Shop Collection Rebuild

`UiShopPresenter` rebuilds group and item bindings on show and selected-group changes.
`UiShopView` uses `PooledViewCollection<TView>` for group and item views, but still:

- releases all active pooled items before each rebuild;
- rents or creates items for every row in the current group;
- clears and re-adds Unity UI listeners for rebuilt item/group views;
- calls `Canvas.ForceUpdateCanvases()` before scrolling to top.

Current demo data is small. Scaling behavior for larger item counts remains
unmeasured and belongs to `UIW-31`.

### Settings Language Refresh

`UiSettingsPresenter` builds language items during initialization and uses show-scoped
listeners for language toggles. On current-language changes it refreshes localized
labels and selection over the existing language item list.

Current risk is limited by the small language set, but the path should remain in the
mobile text/layout audit because it uses legacy `Text` and several layout groups.

### Object Indicator Update Path

`UiObjectIndicatorPresenter` subscribes to `Observable.EveryUpdate()` in show scope and
updates screen position through `IUiWorldToScreenAdapter` every frame while visible.
It registers `UiFxTarget.CharacterReward` only while shown and guards reward actions
after hide begins.

Current implementation covers one indicator. Scaling to multiple anchors is
measurement-driven and belongs to `UIW-31`.

### Hints And FX Tweens

`UiHintsView` creates one DOTween `Sequence` per hint and kills it in `ClearHint()`.
`UiFxView` tracks active sequences and active FX items, kills all active sequences in
`StopAllFx()`, and returns active items to a local pool. `UiHintsPresenter` and
`UiFxPresenter` call cleanup on hide.

Current lifecycle tests cover hidden overlays and show-scope cleanup. Production
targets should keep active tweens at zero after hide/pool return.

### Raycast, Text, And Layout

Static and Unity prefab inspection show:

- 41 active prefab raycast targets;
- 38 legacy `UnityEngine.UI.Text` components;
- 9 BestFit legacy text components;
- 25 layout groups and 2 content size fitters across migrated prefabs;
- `UIDevelopScene` keeps the accepted preview CanvasScaler baseline:
  `Scale With Screen Size`, `1280 x 720`, match `0.5`, reference pixels per unit
  `100`.

This is measurement-driven work for `UIW-30`.

### Validation Surface

Static validation scans on 2026-06-28:

- Runtime/prefab/scene scan for `UniRx`, `Zenject`, `Libs.OpenUI`, `OpenUI`, and
  `SetActive(` returned no matches in runtime code, migrated prefabs, `SampleScene`,
  or `UIDevelopScene`.
- `ProjectContext` scan for DOTween references returned no matches.
- `ProjectContext` scan for Unity UI, UI.Windows, presenter, WindowSystem, or DOTween
  references returned no matches.
- A broader all-script scan found existing `SetActive` use only in tests:
  `SceneCompositionRootLifecycleTests.cs` and `WindowPresenterBindingTests.cs`.
- Current automated PlayMode surface contains 12 `[UnityTest]` tests and 41 `[Test]`
  tests across `CompositionRoot`, `ProjectContext`, and `UiWindowsMvp` test folders.

This is the starting point for `UIW-33`, which should turn the most important scans
into durable local validation gates.

## Initial Targets And Decision Rules

These are starting rules for `UIW-28` through `UIW-34`. They are not final device
acceptance criteria.

| Area | Initial rule |
| --- | --- |
| 60 FPS frame budget | Treat 16.67 ms as the total mobile frame budget. UI work must preserve headroom for gameplay and rendering; a UI path that visibly consumes a whole frame on target device requires redesign, prewarm, or deferral. |
| Cold startup and cold first-show | Highest priority risk. Until device data exists, Editor first-show measurements should be captured per window category. If a cold first show has obvious Editor hitches or high allocations, `UIW-29` should define prewarm or async policy before production growth. |
| Warm reopen/pool reuse | Reopening pooled windows should avoid repeated source creation and should not accumulate duplicate presenters, button listeners, subscriptions, or hidden request consumers. Existing lifecycle tests remain the behavioral guard. |
| Allocations | Warm show/hide/reopen paths should trend toward zero managed allocations. Any persistent per-frame allocation in object indicators, FX, hints, shop lists, or settings refresh should be treated as a mobile blocker until justified. |
| UI object count | Current migrated prefab baseline is 148 GameObjects across 11 prefabs. Future tasks should document any material growth, especially for auto-start surfaces and dynamic collections. |
| Layout rebuild cost | Layout rebuilds should be event-driven, not per-frame. `Canvas.ForceUpdateCanvases()` and `ContentSizeFitter` usage should be measured when dynamic collections scale. |
| Raycast targets | Current prefab baseline is 41 active raycast targets. `UIW-30` should disable non-interactive raycast targets where practical and justify remaining blockers and controls. |
| Legacy Text / BestFit | Current baseline is 38 legacy `Text` components and 9 BestFit texts. BestFit and scaled legacy text should be audited for mobile readability and layout cost before adding more. |
| Active tweens | Hints and FX must have 0 active sequences after hide/pool return and must not consume request streams while hidden. Existing tests cover behavior; future validation can add direct counters if useful. |
| Integrated acceptance runtime | Focused integrated SampleScene test baseline is 1.6582813 seconds of Unity-reported test duration. Treat growth above roughly 3 seconds or above 20 percent without explanation as a review trigger. |

## Child Issue Classification

| Child issue | Classification | Baseline-driven reason |
| --- | --- | --- |
| `UIW-28` | Structural cleanup, measurement-informed | Shared launcher/runtime-source mechanics live in `UiRuntimeWindowHandle.cs` and `UiRuntimeWindowSource.cs`. Verify behavior and first-show impact after consolidation. |
| `UIW-29` | Measurement-driven policy | Cold startup and first-show are the highest mobile risks and need explicit prewarm/loading rules. |
| `UIW-30` | Measurement-driven optimization | Raycast targets, legacy Text, BestFit, layout groups, CanvasScaler behavior, and mobile readability need audit and selective changes. |
| `UIW-31` | Measurement-driven scaling | Shop collection rebuild and object indicator per-frame update paths need larger-data and multi-indicator evidence. |
| `UIW-32` | Structural cleanup, measurement-informed | Runtime-built fallback UI should move toward prefab-first production assets where it reduces runtime allocation and improves inspectability. |
| `UIW-33` | Structural validation | Existing scans are manual. Durable local commands should make dependency, lifecycle, text hygiene, and doc-link checks repeatable. |
| `UIW-34` | Reconciliation | Parent closure requires target-to-artifact-to-evidence traceability, not just closed child issues. |

## Prioritized Risks

1. Cold startup and cold first-show on low-end Android remain unmeasured. This is the
   highest parent risk and should drive `UIW-29`.
2. Lazy runtime source creation plus `forceSyncLoad` and `showSync` can create visible
   first-show hitches if production scenes grow beyond the current demo size.
3. `UiSettingsView`, `UiShopView`, and `UiTopLeftView` carry the highest current
   raycast/text/layout counts and should lead `UIW-30` inspection.
4. `UiObjectIndicatorPresenter` has a per-frame visible update path. It is correct for
   one indicator, but multi-indicator scale is unknown.
5. `UiShopPresenter` and `UiShopView` use pooling, but larger item sets may still
   expose layout rebuild, listener churn, and `Canvas.ForceUpdateCanvases()` cost.
6. Runtime-built fallback layouts in views remain useful as development safety nets but
   are not ideal production assets. `UIW-32` should decide what becomes prefab-first.
7. Validation is currently based on manual scans and selected tests. `UIW-33` should
   convert the stable parts into commands future agents can run before closure.

## Deferred Device Evidence

The following evidence is intentionally missing from `UIW-27` and should not be
represented as complete:

- Low-end Android cold startup time.
- Low-end Android cold first-show latency per window category.
- Android frame pacing under the integrated SampleScene workflow.
- Android managed allocation timeline for cold show, warm reopen, shop rebuild, object
  indicator updates, hints, and FX.
- Android memory footprint after startup, after all auto-start overlays, and after
  opening settings/shop/modal surfaces.
- iOS evidence of any kind.

`UIW-29` and later performance issues should continue to label Editor evidence and
mobile inference separately until Vitaly authorizes target-device builds/profiling.

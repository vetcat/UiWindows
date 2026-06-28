# UI.Windows MVP Loading, Prewarm, And First-Show Policy

Date: 2026-06-28
Issue: `UIW-29` - `02 - Define mobile loading, prewarm, and first-show policy`
Parent: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`

## Purpose

This document turns the current lazy UI.Windows MVP loading behavior into an
explicit policy for mobile production readiness. It covers the migrated
`SampleScene` window families and separates measured Editor evidence from mobile
inference and deferred target-device evidence.

This policy preserves the accepted architecture:

- UI.Windows owns window loading, showing, hiding, pooling, layouts, and resource
  cleanup.
- Project launchers may wrap `WindowSystem.Show` / `Hide`, but must not replace
  UI.Windows lifecycle with direct prefab instantiation, manual destruction, or
  `GameObject.SetActive` show/hide.
- Presenters bind after UI.Windows has produced a window instance.
- Pooled windows keep one presenter binding per pooled window instance; each show
  gets a fresh show scope.
- `ProjectContext` ports remain independent from UI.Windows, Unity UI,
  presenters, DOTween, OpenUI, Zenject, and UniRx.

## Current Mechanics

All migrated `SampleScene` runtime launchers use
`UiRuntimeWindowHandle<TWindow, TView>` and
`UiRuntimeWindowSource<TWindow, TView>`.

The current shared path is:

1. The concrete launcher lazily creates one `UiRuntimeWindowSource` on first
   `Show`.
2. The source creates a UI.Windows window source, layout source, disabled source
   camera, and direct component `Resource` for the view prefab.
3. The source sets `createPool = true`,
   `preferences.singleInstance = true`, and `preferences.forceSyncLoad = true`.
4. The handle calls `WindowSystem.Show` with `InitialParameters.showSync = true`
   and an immediate transition.
5. The show callback binds a presenter only when
   `WindowPresenterBinder.TryGetBinding` finds no active binding on the pooled
   window instance.
6. `Hide` delegates to the UI.Windows window hide path with an immediate
   transition.

`forceSyncLoad` and `showSync` are acceptable in the current reference project
because view resources are direct prefab references and tests need deterministic
lifecycle behavior. For production mobile scenes, synchronous first show must be
limited to windows that are either shown at scene start, prewarmed during an
idle-safe frame, or small enough that measured device first-show cost is below
the scene's UI budget.

If a future project moves windows to Addressables, remote content, larger view
hierarchies, or heavier dynamic content, do not keep synchronous cold show as
the default without new device evidence. Add an explicit loading/prewarm path
through UI.Windows rather than bypassing UI.Windows ownership.

## Strategy Definitions

| Strategy | Meaning |
| --- | --- |
| Show at scene start | The window is part of the initial scene UI and should be opened by the scene installer through its launcher. It stays resident while the scene is active unless hidden by gameplay state. |
| Early idle prewarm | After the first rendered/interactive frame, call the launcher through UI.Windows, then hide through the UI.Windows hide path if the window should not remain visible. Use this for likely-to-open or request-critical windows whose cold show could interrupt gameplay. |
| Load on demand | Defer first `Show` until the user or model state requests the window. Use this for optional or lower-probability windows when measured cold show is acceptable. |
| Keep resident while scene active | Once shown or prewarmed, rely on UI.Windows pooling/single-instance behavior and keep the pooled instance available until scene teardown. |
| Clean/unload after use | Hide through UI.Windows and clean/unload only for rare, heavy, or scene-specific windows where memory pressure outweighs reopen speed. This is not the default for current migrated windows. |

Prewarm means using the same launcher and UI.Windows lifecycle as a normal show,
then hiding immediately or after the first valid lifecycle tick. It does not mean
instantiating prefabs manually, keeping inactive GameObjects outside UI.Windows,
or creating a second pooling layer.

## Window-Family Policy

| Window family | Current first-show path | Policy strategy | Residency and cleanup | Evidence status | Follow-up |
| --- | --- | --- | --- | --- | --- |
| Top-left player HUD | `UiTopLeftDemoInstaller.Start` calls `UiTopLeftDemoLauncher.Show` when `showOnStart` is true. | Show at scene start. | Keep resident while scene active; hide only for workflow tests or scene state. | Included in Editor auto-start batch; warm reopen verified. | None for loading policy. |
| Top-right coins HUD | Installer calls `UiTopRightDemoLauncher.Show` when `showTopRightOnStart` is true. | Show at scene start. | Keep resident while scene active so the coin FX target is available while visible. | Included in Editor auto-start batch; warm reopen verified. | None for loading policy. |
| Top-center time/hint HUD | Installer calls `UiTopCenterDemoLauncher.Show` when `showTopCenterOnStart` is true. | Show at scene start. | Keep resident while scene active; hold timers remain show-scoped. | Included in Editor auto-start batch; warm reopen verified. | None for loading policy. |
| Down-right settings launcher | Installer calls `UiDownRightDemoLauncher.Show` when `showDownRightOnStart` is true. | Show at scene start. | Keep resident while scene active; it is a lightweight navigation control. | Included in Editor auto-start batch; warm reopen verified. | None for loading policy. |
| Down-left shop launcher | Installer calls `UiDownLeftDemoLauncher.Start`, then `Show` when `showDownLeftOnStart` is true. | Show at scene start; hide while shop is visible. | Keep resident while scene active; visibility follows `UiShopVisibilityState`. | Included in Editor auto-start batch; warm reopen verified. | None for loading policy. |
| Hints overlay | Installer opens `UiHintsDemoLauncher.Show` as an empty overlay. | Show at scene start. | Keep resident while scene active so transient hint requests are not lost while the overlay is intended to listen. Hidden overlays must not consume requests. | Included in Editor auto-start batch; warm reopen verified. | Future validation may add direct active-request counters under `UIW-33`. |
| FX overlay | Installer opens `UiFxDemoLauncher.Show` as an empty overlay. | Show at scene start. | Keep resident while scene active so transient FX requests can render. Hidden overlays must not consume requests and must have zero active tweens after hide. | Included in Editor auto-start batch; warm reopen verified. | Future validation may add active-tween counters under `UIW-33`. |
| Object indicator | Installer calls `UiObjectIndicatorDemoLauncher.Show` for one representative demo target when `showObjectIndicatorOnStart` is true. | In the reference scene, show one representative indicator at scene start. In production, prewarm the indicator layer early idle and show only indicators needed for active anchors. | Keep the layer resident while scene active; scale dynamic indicator count separately. | Included in Editor auto-start batch; warm reopen verified for one indicator. | Multi-anchor scaling belongs to `UIW-31`. |
| Settings window | Opened by the down-right launcher or directly by `UiSettingsDemoLauncher.Show`. | Load on demand by default; early idle prewarm if settings is likely in the first session or if target-device first-show exceeds the scene budget. | Keep resident after first show/prewarm while scene active. Clean on scene teardown. | Editor cold first show and warm reopen measured. | Mobile layout/text audit belongs to `UIW-30`. |
| Modal window | `UiModalDemoLauncher.Start` subscribes to modal state; first non-null modal request opens the window. | Load on demand for the current reference scene. Early idle prewarm in production scenes where modal requests can occur during gameplay-critical frames or before UI latency is acceptable. | Keep resident after first request/prewarm while scene active. The state subscription must be active before requests are emitted. | Editor cold first show and warm reopen measured. | None for loading policy. |
| Shop window | Opened by down-left launcher or directly by `UiShopDemoLauncher.Show`. | Early idle prewarm is recommended for production if shop is likely to be opened, because it is the heaviest measured cold first show. Load on demand remains acceptable for the reference scene and low-probability shop flows. | Keep resident after first show/prewarm while scene active; do not rebuild a custom pool outside UI.Windows. | Editor cold first show and warm reopen measured. | Larger collection/layout scaling belongs to `UIW-31`; text/layout/raycast audit belongs to `UIW-30`. |

## Measured Editor Evidence

Measurement came from the focused PlayMode test:

`UiWindowsMvp.Tests.PlayMode.UiWindowLoadingPolicyTests.SampleScene_RecordsEditorFirstShowAndWarmReopenPolicyEvidence`

Environment:

- Unity instance: `UiWindows@29793614097f6f61`.
- Unity version: `6000.4.4f1`.
- Editor platform: `OSXEditor`.
- Active scene: `Assets/Scenes/SampleScene.unity`.
- Evidence method: PlayMode stopwatch around scene auto-start, cold first show,
  and warm pooled reopen; managed memory delta from `GC.GetTotalMemory(false)`.

These numbers are Editor-only trend evidence. They are not low-end Android
acceptance data. The managed memory deltas are heap-size deltas, not precise
allocation counts; zero means no observed managed heap growth in that measurement,
not proof of zero allocations.

| Family | Phase | Editor elapsed ms | Managed memory delta bytes | Lifecycle evidence |
| --- | --- | ---: | ---: | --- |
| SampleScene auto-start batch | Scene load to all default windows shown | 297.936 | Not captured | Includes top-left, top-right, top-center, down-right, down-left, hints, FX, object indicator. |
| Top-left HUD | Warm reopen | 5.701 | 0 | Same pooled instance and presenter binding. |
| Top-right HUD | Warm reopen | 4.246 | 0 | Same pooled instance and presenter binding. |
| Top-center HUD | Warm reopen | 4.708 | 0 | Same pooled instance and presenter binding. |
| Down-right settings launcher | Warm reopen | 3.977 | 0 | Same pooled instance and presenter binding. |
| Down-left shop launcher | Warm reopen | 4.186 | 0 | Same pooled instance and presenter binding. |
| Hints overlay | Warm reopen | 3.451 | 0 | Same pooled instance and presenter binding. |
| FX overlay | Warm reopen | 3.845 | 0 | Same pooled instance and presenter binding. |
| Object indicator | Warm reopen | 4.110 | 0 | Same pooled instance and presenter binding. |
| Settings | Cold first show | 14.581 | 241,664 | Pooled, single presenter binding. |
| Settings | Warm reopen | 4.823 | 0 | Same pooled instance and presenter binding. |
| Modal | Cold first show | 6.052 | 114,688 | Pooled, single presenter binding. |
| Modal | Warm reopen | 4.274 | 0 | Same pooled instance and presenter binding. |
| Shop | Cold first show | 22.359 | 675,840 | Pooled, single presenter binding. |
| Shop | Warm reopen | 6.319 | 4,096 | Same pooled instance and presenter binding. |

Interpretation:

- The current reference scene's startup batch is acceptable as Editor evidence for
  a small demo, but it is not a target-device startup budget.
- Warm reopen is consistently lower than cold first show and preserves pooled
  instance/binding identity.
- `UiShopWindow` is the highest measured on-demand cold first-show path in the
  current Editor environment, so production scenes should prewarm it during early
  idle when shop opening is likely.
- `UiSettingsWindow` is the next highest on-demand cold path. Keep it on demand
  in the reference scene; prewarm it in product scenes where settings is commonly
  opened early.
- `UiModalWindow` cold show is smaller in this snapshot, but modal request timing
  can be user-critical. Use early idle prewarm for scenes where modal latency is
  not acceptable on first request.

## Mobile Inference

Low-end Android remains the primary target class and 60 FPS remains the runtime
target. Treat 16.67 ms as the full frame budget, not as a UI-only allowance.

Mobile production decisions should use these rules until target-device evidence
exists:

- Do not add more scene-start windows without measuring startup impact.
- Prefer scene-start residency for persistent HUD and empty request overlays that
  are expected to listen during normal gameplay.
- Prefer early idle prewarm for likely-to-open heavy windows, especially shop and
  potentially settings.
- Keep rare optional windows on demand only when their measured target-device cold
  first show fits within the scene's UI budget or can be hidden behind an accepted
  loading affordance.
- Keep pooled instances resident while the scene is active unless target-device
  memory pressure proves cleanup-after-use is necessary.
- Re-evaluate `forceSyncLoad` and `showSync` if future windows use Addressables,
  remote content, larger hierarchy counts, or heavier dynamic data.

## Deferred Device Evidence

No Android or iOS build was produced for `UIW-29`. No target-device profiler
capture exists yet.

The following evidence remains intentionally deferred:

- Low-end Android cold startup time.
- Low-end Android cold first-show latency per window family.
- Android frame pacing during the integrated SampleScene workflow.
- Android managed allocation timeline for cold show and warm reopen.
- Android memory footprint after startup, after scene-start overlays, after
  settings/shop/modal first show, and after scene teardown.
- iOS evidence of any kind.

Future device profiling should use this policy as the expected strategy map and
record whether each family remains `Done`, needs prewarm, needs async loading, or
needs cleanup-after-use under real mobile constraints.

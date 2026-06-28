# UI.Windows MVP Prefab-First Fallback Audit

Date: 2026-06-28
Issue: `UIW-32` - `05 - Move fallback-built UI toward prefab-first production assets`
Parent: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`

## Purpose

This audit records the production decision for runtime-built UI fallback paths in the
migrated `SampleScene` UI.Windows MVP views.

The production rule is now:

- Static view shells should come from prefabs under
  `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows`.
- View components may validate required serialized references, but should not silently
  rebuild complete replacement hierarchies at runtime.
- Data-driven transient UI may still be created at runtime when it is the current
  design and is pooled or bounded by model data.

## Decision Table

| View or path | Previous runtime construction | Action | Production justification |
| --- | --- | --- | --- |
| `UiTopLeftView` | None. The view already exposed serialized references only. | Kept prefab-first behavior unchanged. | The prefab is the source of truth for the player HUD shell. |
| `UiTopRightView` | `BuildDefaultLayout` created the coin HUD shell and text/icon refs. | Removed broad fallback builder; `EnsureLayout()` now requires prefab-assigned refs. | `UiTopRightView.prefab` already serializes the coin HUD references and preview coverage exists. |
| `UiTopCenterView` | `BuildDefaultLayout` created the clock/hold shell and `UiTopCenterHoldInput`. | Removed broad fallback builder; `EnsureLayout()` now requires prefab-assigned refs. | The top-center prefab owns the pointer input surface, time text, hold text, and static layout. |
| `UiDownLeftView` | `BuildDefaultLayout` created the item-shop launcher button shell. | Removed broad fallback builder; `EnsureLayout()` now requires prefab-assigned refs. | `UiDownLeftView.prefab` owns the launcher button hierarchy and item-shop icon/text refs. |
| `UiDownRightView` | `BuildDefaultLayout` created the settings launcher shell. | Removed broad fallback builder; `EnsureLayout()` now requires prefab-assigned refs. | `UiDownRightView.prefab` owns the launcher button hierarchy and settings icon/text refs. |
| `UiSettingsView` | Runtime-created language item shells for available languages. | Retained and documented as dynamic UI. | Language rows are model-driven by `ILocalizationReadModel.AvailableLanguages`; the static settings shell remains prefab-owned. A future template prefab is optional if the language list grows. |
| `UiShopView` | `BuildDefaultLayout` created the full shop shell; group/item rows were also runtime-created. | Removed full shell builder; retained pooled group/item row factories. | The shop prefab owns the modal shell, group/item roots, scroll rect, pool roots, and details refs. Group/item rows remain dynamic collection content and are covered by `PooledViewCollection<TView>`. |
| `UiModalView` | `BuildDefaultLayout` created the consolidated modal shell; `EnsureRuntimeGroups()` added missing `CanvasGroup`/`LayoutElement` components. | Removed broad fallback builder and runtime component addition; required prefab refs/components now fail fast. | `UiModalView.prefab` owns the consolidated modal structure. Runtime behavior should switch modes, not repair missing modal hierarchy. |
| `UiHintsView` | `BuildDefaultLayout` created the hint body, text, pointer, and canvas group. | Removed broad fallback builder; `EnsureLayout()` now requires prefab-assigned refs. | The hint overlay shell is static and already covered by `UiHintsView.prefab` and `UIDevelopScene`. |
| `UiFxView` | `BuildDefaultLayout` created the overlay roots and fallback source/target anchors. | Removed static overlay fallback builder; retained pooled transient FX item creation. | The overlay roots and anchors are prefab-owned. FX item instances are request-driven transient visuals and are pooled by the view. |
| `UiObjectIndicatorView` | `BuildDefaultLayout` created the representative indicator item shell. | Removed broad fallback builder; `EnsureLayout()` now requires prefab-assigned refs. | The representative indicator item is part of `UiObjectIndicatorView.prefab`; runtime updates should move and update it, not rebuild it. |
| `UiRuntimeWindowSource` | Creates UI.Windows runtime source objects and layout slots. | Retained as out of scope. | This is UI.Windows runtime source infrastructure, not view fallback UI. It remains governed by the loading/prewarm policy. |

## Test Fixture Boundary

Presenter unit-style PlayMode tests no longer rely on production view code to build
complete fallback hierarchies. Tests use `UiViewTestFixtures` under
`Assets/Scripts/UiWindowsMvp/Tests/PlayMode` for minimal test-only view objects.

This keeps production view code prefab-first while preserving focused presenter tests
that do not need to load full Unity prefab assets.

## Remaining Runtime Construction

The remaining production `new GameObject` paths under
`Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows` are intentionally limited to:

- `UiRuntimeWindowSource`: UI.Windows runtime source and slot objects.
- `UiSettingsView`: language rows derived from the localization model.
- `UiShopView`: pooled shop group and item rows derived from shop model data.
- `UiFxView`: pooled transient FX item views derived from request streams.

These retained paths should be revisited only with evidence that prefab templates would
reduce runtime cost or improve maintainability without broadening the current sample
architecture.

## Verification Evidence

Verification on 2026-06-28 used Unity `6000.4.4f1` through the local Unity MCP
instance `UiWindows@29793614097f6f61`:

- Serialized prefab YAML checks confirmed the required view references are assigned
  for the prefab-backed views that no longer rebuild static shells.
- `Assets/Scenes/Develop/UIDevelopScene.unity` validation reported 0 issues, 0
  missing scripts, and 0 broken prefabs.
- `Assets/Scenes/SampleScene.unity` validation reported 0 issues, 0 missing scripts,
  and 0 broken prefabs.
- Rider diagnostics on changed C# files reported no problems, and Rider solution build
  passed.
- Unity script refresh/compile completed with zero Console errors or warnings after
  clearing known Test Runner result-save/performance cleanup noise.
- `UiWindowsMvp.Tests.PlayMode` passed 39/39.
- Static scans found no new UniRx, Zenject, OpenUI runtime, `ProjectContext` DOTween
  leakage, or runtime `SetActive(` lifecycle use. Existing `SetActive(` matches remain
  limited to historical tests.
- Remaining production `new GameObject` matches are the intentionally retained
  `UiRuntimeWindowSource`, `UiSettingsView`, `UiShopView`, and `UiFxView` dynamic paths
  listed above.

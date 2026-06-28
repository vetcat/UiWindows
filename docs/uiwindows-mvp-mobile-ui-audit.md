# UI.Windows MVP Mobile UI Layout, Raycast, And Text Audit

Date: 2026-06-28
Issue: `UIW-30` - `03 - Audit and optimize UI layout, raycast, and text settings for mobile`
Parent: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`

## Purpose

This audit records the first targeted mobile UI cleanup pass for the 11 migrated
`SampleScene` UI.Windows MVP prefabs under
`Assets/Prefabs/UiWindowsMvp/SampleSceneWindows`.

The goal is to reduce avoidable mobile UI overhead without changing visible
behavior or reopening the accepted `UIW-1` migration scope.

Evidence is limited to Unity Editor prefab/runtime inspection and static scans.
No Android or iOS build was produced, and no target-device profiling is claimed.

## Starting Point

`UIW-27` established this prefab baseline:

| Metric | Before |
| --- | ---: |
| GameObjects | 148 |
| Graphics | 90 |
| Active raycast targets | 41 |
| Legacy `Text` | 38 |
| BestFit legacy `Text` | 9 |
| Layout groups | 25 |
| Content size fitters | 2 |

The highest-risk static surfaces were:

- `UiSettingsView`: 12 raycast targets, 9 layout groups.
- `UiShopView`: 8 raycast targets, 8 layout groups, 1 content size fitter.
- `UiTopLeftView`: 5 raycast targets, 7 BestFit texts, 5 layout groups, 1 content size fitter.

## Targeted Changes

The cleanup only changed elements with clear ownership:

- Disabled raycast targets on static panel/icon/decorator graphics that are not
  controls and do not need to block lower UI.
- Kept raycast targets on buttons, toggles, slider tracks/handles that still act
  as input surfaces, scrollable areas, modal close background, and panel/body
  blockers that prevent click-through in modal-like windows.
- Disabled BestFit only on fixed `+` / `-` `UiTopLeftView` button labels. Dynamic
  or localized labels kept BestFit where clipping risk is higher.
- Updated runtime fallback builders for settings language items, shop fallback
  panels/icons, hints, and object indicators so generated safety-net UI follows
  the same raycast intent as the prefab assets.

No TextMeshPro migration was performed. No layout groups or content size fitters
were removed in this pass.

## Before And After Inventory

| Prefab | Raycast before | Raycast after | BestFit before | BestFit after | Layout groups | Fitters | Rationale |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `UiTopLeftView` | 5 | 4 | 7 | 3 | 5 | 1 | Disabled the non-interactive body background raycast and fixed plus/minus button BestFit. Kept button raycasts and dynamic player/stat BestFit. |
| `UiTopRightView` | 1 | 0 | 1 | 1 | 0 | 0 | Disabled the coin icon raycast; it remains an FX target anchor, not an input surface. |
| `UiTopCenterView` | 1 | 1 | 1 | 1 | 0 | 0 | Kept the body raycast because `UiTopCenterHoldInput` receives pointer down/up/exit events there. |
| `UiDownLeftView` | 1 | 1 | 0 | 0 | 0 | 0 | Kept the item shop button target. |
| `UiDownRightView` | 1 | 1 | 0 | 0 | 0 | 0 | Kept the settings button target. |
| `UiSettingsView` | 12 | 8 | 0 | 0 | 9 | 0 | Disabled toggle checkmark and slider fill raycasts. Kept close button, toggles, slider background/handles, and body/modal panel blocker behavior. |
| `UiShopView` | 8 | 4 | 0 | 0 | 8 | 1 | Disabled static group/items/details panel and details icon raycasts. Kept body panel blocker, close button, scroll area, and viewport. |
| `UiModalView` | 5 | 5 | 0 | 0 | 3 | 0 | Kept modal close background, body blocker, and action buttons. |
| `UiHintsView` | 2 | 0 | 0 | 0 | 0 | 0 | Hints are transient visuals; their CanvasGroup remains non-interactive and non-blocking. |
| `UiFxView` | 0 | 0 | 0 | 0 | 0 | 0 | No static graphics. Runtime FX items already use non-raycast icon/text visuals. |
| `UiObjectIndicatorView` | 5 | 1 | 0 | 0 | 0 | 0 | Disabled background/icon/read-only slider graphic raycasts. Kept the reward button. |
| **Total** | **41** | **25** | **9** | **5** | **25** | **2** | Reduced 16 active raycast targets and 4 BestFit texts. |

## Runtime Path Audit

Static scans under `Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows` found:

- `Canvas.ForceUpdateCanvases()` only in `UiShopView.ScrollToTop()`.
- Runtime-built fallback UI in several view classes, intentionally retained as
  development safety nets until `UIW-32` decides prefab-first production assets.
- Dynamic shop group/item creation and object-indicator per-frame update paths,
  intentionally left for `UIW-31` scaling evidence.

`UiShopView.ScrollToTop()` still forces a canvas update before setting
`ScrollRect.verticalNormalizedPosition`. Removing or replacing that call belongs
with larger shop collection/layout scaling evidence in `UIW-31`, because the
current demo data is small and the behavior is coupled to dynamic content rebuild.

## Verification Evidence

This pass was verified in the local Unity Editor session:

- Unity instance: `UiWindows@29793614097f6f61`.
- Unity version: `6000.4.4f1`.
- Rider file diagnostics for changed C# files: no problems.
- Rider solution build: passed.
- Unity refresh and compile: passed.
- Unity Console after clearing known Test Runner result-save/performance cleanup
  noise: 0 errors, 0 warnings.
- Changed prefab info and hierarchy retrieval passed for `UiTopLeftView`,
  `UiTopRightView`, `UiSettingsView`, `UiShopView`, `UiHintsView`, and
  `UiObjectIndicatorView`.
- `UIDevelopScene` additive inspection found all 11 migrated prefab preview
  instances and 0 missing scripts.
- `UiWindowsMvp.Tests.PlayMode`: 37 passed, 0 failed, 0 skipped.
- `git diff --check main...HEAD`: passed.
- Final-newline check for changed `.cs`, `.md`, and Unity YAML files: passed.
- Forbidden dependency/lifecycle scans for UniRx, Zenject, OpenUI, direct
  `SetActive(` lifecycle usage, and `ProjectContext` DOTween references: no
  matches.

## Mobile Interpretation

The active prefab raycast target count dropped from 41 to 25 without changing the
number of objects, graphics, text components, layout groups, or content size
fitters.

This is useful mobile inference, not target-device proof. It should reduce the
number of candidate graphics considered by `GraphicRaycaster`, but Android
frame-time, allocation, and input-system impact still require later device
profiling.

## Remaining Risks

- Legacy `Text` remains the current text system for all migrated prefabs.
- Five BestFit texts remain intentionally for dynamic or localized content:
  player name, health value, XP value, coin amount, and press-and-hold hint.
- `UiSettingsView`, `UiShopView`, and `UiTopLeftView` still carry the highest
  layout-group counts.
- `UiShopView.ScrollToTop()` still uses `Canvas.ForceUpdateCanvases()`.
- Dynamic shop collection and multi-indicator scaling remain outside this issue
  and belong to `UIW-31`.
- Runtime-built fallback UI remains outside broad cleanup scope and belongs to
  `UIW-32`.
- Target-device Android/iOS evidence remains deferred.

# UI.Windows MVP Visual Layout Parity Audit

Date: 2026-06-27
Issue: `UIW-24`

## Purpose

This audit records the static visual/layout parity state for migrated `UiWindowsMvp` view prefabs against their OpenUI source references. It also records the `UIDevelopScene` static preview coverage baseline after the follow-up OpenUI view ports from `UIW-19` through `UIW-23`.

This is not a runtime lifecycle test. `Assets/Scenes/Develop/UIDevelopScene.unity` remains a static prefab inspection scene. UI.Windows lifecycle, pooling, presenter binding, R3 subscriptions, and integrated SampleScene acceptance remain under `Assets/Scenes/SampleScene.unity`, PlayMode tests, and the final `UIW-25` workflow.

## Preview Coverage Baseline

`Assets/Scenes/Develop/UIDevelopScene.unity` now contains static preview instances for all migrated view prefabs under `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows`.

The preview scene keeps the established CanvasScaler baseline:

- UI Scale Mode: Scale With Screen Size
- Reference Resolution: `1280 x 720`
- Match Width Or Height: `0.5`
- Reference Pixels Per Unit: `100`

The scene instances use preview-only RectTransform overrides so all migrated prefabs can be inspected together in one static scene. The prefab assets remain the source of truth for each view's runtime root layout.

Request-driven views may also have scene-only preview state overrides where the idle prefab would otherwise be invisible or empty. `UiModalView Preview` uses sample modal text, and `UiHintsView Preview` uses sample hint text plus visible CanvasGroup alpha. These overrides are for static inspection only.

Current preview instances:

- `UiTopLeftView Preview`
- `UiTopCenterView Preview`
- `UiTopRightView Preview`
- `UiDownLeftView Preview`
- `UiDownRightView Preview`
- `UiSettingsView Preview`
- `UiShopView Preview`
- `UiModalView Preview`
- `UiObjectIndicatorView Preview`
- `UiHintsView Preview`
- `UiFxView Preview`

## Checklist

| Project prefab | OpenUI reference | Layout / role | Preview coverage | Status | Notes |
| --- | --- | --- | --- | --- | --- |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiTopLeftView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/UiTopLeftView.prefab` | Top-left player state HUD with name, level, health, XP, sliders, and mutation buttons. | `UiTopLeftView Preview` | Done | Existing migrated prefab already covered the role. The preview instance was renamed for consistent coverage naming and moved into the static preview board. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiTopRightView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/UiTopRightView.prefab` | Top-right coin HUD and coin FX target. | `UiTopRightView Preview` | Done | Preview coverage added. Layout keeps the OpenUI coin icon plus amount shape, with project-owned UI.Windows view component references. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiTopCenterView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/UiTopCenterView.prefab` | Top-center clock and press-hold hint affordance. | `UiTopCenterView Preview` | Done | Preview coverage added. Project view keeps the clock/hold layout intent while using project-owned hold input and presenter binding. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiDownRightView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/UiDownRightView.prefab` | Bottom-right settings launcher. | `UiDownRightView Preview` | Done | Preview coverage added. Settings icon and label are represented without importing OpenUI runtime infrastructure. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiDownLeftView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/UiDownLeftView.prefab` | Bottom-left item shop launcher. | `UiDownLeftView Preview` | Done | Preview coverage added. Item shop icon and label are represented without OpenUI lifecycle semantics. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiSettingsView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/SettingsView/UiSettingsView.prefab` | Settings modal with settings/language tabs and volume controls. | `UiSettingsView Preview` | Done | Existing preview coverage retained and placed in the static preview board. Language item population remains runtime/model-driven and outside static scene scope. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiShopView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/ShopView/UiShopView.prefab` plus item/group source prefabs | Item shop view with group selector, item list, and details panel. | `UiShopView Preview` | Done | Existing preview coverage retained and placed in the static preview board. Dynamic shop group/item population remains presenter/model-driven and outside static scene scope. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiModalView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/ModaleWindows/ModalInfoOkView.prefab`, `ModalInfoOkCancelView.prefab`, and `ModalWaitView.prefab` | Project-owned consolidated modal view for info OK, info OK/Cancel, and wait modes. | `UiModalView Preview` | Accepted deviation | OpenUI uses separate modal prefabs. The project intentionally consolidates modes into one UI.Windows view to keep lifecycle/pooling simple while preserving behavior through presenter state. The preview instance uses scene-only sample text. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiHintsView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/UiHintsView.prefab` | Transient hint overlay. | `UiHintsView Preview` | Done | Preview coverage added. Runtime text/animation is request-driven; the preview instance uses scene-only sample text and visible alpha for inspection. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiFxView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/UIFxView.prefab` | Collect/spend FX overlay with source/target anchors and pooled FX items. | `UiFxView Preview` | Accepted deviation | The project prefab maps to OpenUI `UIFxView` despite the casing/name difference. Idle preview coverage is mostly hierarchy and anchor validation because animated FX items are request-driven at runtime. |
| `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiObjectIndicatorView.prefab` | `/Users/vitaly/Projects/OpenUI/Assets/Prefabs/UiPrefabs/SampleSceneWindows/ObjectIndicatorView/UiObjectIndicatorView.prefab` and `UiIndicatorItemView.prefab` | Dynamic object indicator with health, icon, text, and reward button. | `UiObjectIndicatorView Preview` | Accepted deviation | OpenUI separates the indicator item prefab. The project-owned view keeps the representative item structure inside the UI.Windows-compatible view prefab to simplify the current vertical slice. |

## Accepted Deviations

- `UiModalView.prefab` intentionally consolidates OpenUI's three modal prefabs into one UI.Windows view with presenter-controlled modes.
- `UiFxView.prefab` intentionally keeps idle FX content minimal; collect/spend visuals are request-driven by `UiFxPresenter` and DOTween in runtime.
- `UiObjectIndicatorView.prefab` intentionally includes the representative indicator item structure inside the migrated view instead of preserving OpenUI's separate `UiIndicatorItemView.prefab` asset split.
- `UiSettingsView.prefab` and `UiShopView.prefab` do not statically populate all runtime-generated language/group/item rows in `UIDevelopScene`; those are model/presenter-driven and covered by runtime slices.
- `UIDevelopScene` uses preview-only placement/scaling and sample-state overrides so every migrated view is covered in one inspection scene. Runtime anchoring and request-driven state remain on the prefab assets, presenters, and runtime SampleScene wiring.

## Follow-ups

No additional visual/layout defect requiring a new Linear issue was identified during this audit. The missing static preview coverage was applied directly in `UIDevelopScene`.

`UIW-25` later completed the integrated SampleScene acceptance workflow and final `UIW-1` reconciliation.

## Verification Evidence

- Linear `UIW-24` issue and latest `Executor Handoff` were read before implementation.
- Unity MCP loaded `Assets/Scenes/Develop/UIDevelopScene.unity` successfully.
- Unity MCP prefab info was read for all 11 migrated prefabs.
- Unity Console reported zero errors and zero warnings after scene load and scene edit.
- `UIDevelopScene` hierarchy inspection reported `Canvas` with 11 static preview children after the update.
- A 1280 x 720 camera render was generated in ignored `Temp/UIW-24/uidevelopscene-preview-coverage-1280x720.png` for manual visual review.

# UI.Windows MVP Final Reconciliation

Date: 2026-06-27
Issue: `UIW-25`
Parent: `UIW-1` - `[Plan] UI.Windows MVP migration with R3, without Zenject/UniRx`

## Purpose

This document is the final reconciliation gate for `UIW-1` after the follow-up gaps identified by `UIW-18` were implemented or audited in `UIW-19` through `UIW-24`.

It maps the original parent goal and OpenUI migration scope to current repository artifacts, verification evidence, and final status. It should be read together with:

- `docs/uiwindows-mvp-openui-migration-guide.md`
- `docs/uiwindows-mvp-pooling-lifecycle.md`
- `docs/r3-mvp-conventions.md`
- `docs/dotween-ui-fx-dependency.md`
- `docs/uiwindows-mvp-visual-layout-parity-audit.md`

## Final Closure

Closure result: `UIW-1` was closed as complete on 2026-06-27 after Vitaly explicitly authorized parent-level closure. `UIW-25` was accepted, merged, pushed, and closed through the normal Orchestrator/Closure Executor flow on 2026-06-27 before the parent was closed.

The repository now satisfies the parent plan's intended scope: a UI.Windows-owned lifecycle and resource/pooling layer, a project-owned MVP presenter adapter, R3-backed model/read-model ports, CompositionRoot wiring without Zenject, no UniRx dependency, and migrated representative OpenUI SampleScene UI examples and layouts. The implementation is not a literal OpenUI clone and does not import OpenUI infrastructure; that is an explicit project constraint, not a remaining gap.

No new migration child issue is required for `UIW-1` closure. The remaining differences are accepted architectural or visual deviations already documented in the migration and visual parity docs. The successor plan `UIW-26` tracks production hardening and mobile readiness improvements; it is not a remaining `UIW-1` migration gap.

## Integrated SampleScene Evidence

`Assets/Scenes/SampleScene.unity` is the canonical runtime/integration scene. `UIW-25` adds a focused PlayMode acceptance workflow:

- `Assets/Scripts/UiWindowsMvp/Tests/PlayMode/SampleSceneIntegratedAcceptanceTests.cs`
- Test: `SampleScene_RunIntegratedMigratedWorkflowAndRepresentativeShowScopeCleanup`

The workflow loads `SampleScene`, resolves services and launchers through `SceneCompositionRoot`, and drives the migrated surface through UI.Windows lifecycle paths:

- Auto-starts `UiTopLeft`, `UiTopRight`, `UiTopCenter`, `UiDownRight`, `UiDownLeft`, hints, FX, and object indicator windows.
- Verifies pooled UI.Windows windows have presenter bindings and one presenter instance per pooled window.
- Exercises top HUD health/coins binding.
- Opens settings from the down-right launcher and updates settings state.
- Opens shop from the down-left launcher, hides/restores the launcher, clicks a shop item, and completes the modal.
- Emits a top-center press-hold hint through `IUiFeedbackCommands`.
- Routes object-indicator reward FX from `UiFxTarget.CharacterReward` to the top-right coin target.
- Hides/reopens representative pooled windows and overlays, proving stale hidden UI does not consume requests or handlers and presenter bindings are reused.
- Cleans windows through `WindowSystem.Clean` and asserts presenter bindings are disposed.

## Traceability Matrix

| Parent target / migration scope | Implemented artifact(s) | Verification evidence | Status |
| --- | --- | --- | --- |
| Pinned UI.Windows dependency workflow | `Packages/manifest.json`, `Packages/packages-lock.json`, `docs/ui-windows-fork-workflow.md` | `UIW-12` / `UIW-2` closure notes; Unity compile on pinned `com.me.ui.windows` commit | Done |
| UI.Windows package integration on Unity 6000.4.4f1 | `com.me.ui.windows` UPM dependency, package lock, project Unity settings | `UIW-2` closure notes; Unity refresh/compile with zero Console errors/warnings | Done |
| CompositionRoot without Zenject/OpenUI/UI.Windows/R3 coupling | `Assets/Scripts/CompositionRoot/Runtime`, `docs/project-architecture-skeleton.md` | CompositionRoot PlayMode tests; assembly reference boundaries; project context rules | Done |
| R3 reactive foundation replacing UniRx/custom Rx baseline | `Packages/manifest.json`, `Packages/nuget-packages`, `docs/r3-mvp-conventions.md`, R3-backed ports | `UIW-13` closure; R3 compile smoke; model/read-model tests | Done |
| DOTween UI/effects dependency boundary | `Assets/Plugins/Demigiant/DOTween`, `Assets/Resources/DOTweenSettings.asset`, `docs/dotween-ui-fx-dependency.md` | `UIW-17` closure; DOTween smoke tests; ProjectContext DOTween scans | Done |
| MVP presenter lifecycle adapter over UI.Windows | `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` | `WindowPresenterBindingTests`; `UIW-5` and `UIW-8` verification | Done |
| UI.Windows pooling/show-scope proof | Pooled runtime sources, `IUiShowScope`, lifecycle tests for each major window family | `docs/uiwindows-mvp-pooling-lifecycle.md`; full `UiWindowsMvp.Tests.PlayMode`; integrated SampleScene acceptance test | Done |
| Player model/service from OpenUI | `Assets/Scripts/ProjectContext/Runtime/Player` | `PlayerServiceTests`; no UI.Windows/Unity UI/presenter dependencies in ProjectContext player layer | Done |
| `UiTopLeft` player HUD | `UiTopLeftView.prefab`, `UiTopLeftWindow`, `UiTopLeftPresenter`, `SampleScene` wiring | `UiTopLeftPresenterTests`; `UiTopLeftWindowLifecycleTests`; integrated acceptance test | Done |
| `UiTopRight` coins HUD and coin FX target | `UiTopRightView.prefab`, `UiTopRightPresenter`, `UiFxTargetRegistry` coin target registration | `UIW-19`; `UiTopRightWindowLifecycleTests`; integrated acceptance test | Done |
| `UiTopCenter` time and press-hold hint | `UiTopCenterView.prefab`, time provider, hold input/timer, localized hint trigger | `UIW-23`; `UiTopCenterPresenterTests`; `UiTopCenterWindowLifecycleTests`; integrated acceptance test | Done |
| `UiDownRight` settings launcher plus settings/localization | `UiDownRightView.prefab`, `UiDownRightPresenter`, `UiSettingsView`, settings/localization services | `UIW-9`; `UIW-20`; `UiDownRightWindowLifecycleTests`; `UiSettingsWindowLifecycleTests`; integrated acceptance test | Done |
| `UiDownLeft` shop launcher plus shop/items/modal behavior | `UiDownLeftView.prefab`, `UiDownLeftDemoLauncher`, `UiShopVisibilityState`, `UiShopPresenter` modal request | `UIW-10`; `UIW-21`; `UiDownLeftWindowLifecycleTests`; `UiShopWindowLifecycleTests`; integrated acceptance test | Done |
| Modal info / OK-cancel / wait behavior | `ProjectContext.UiRequests`, `UiModalView`, `UiModalPresenter`, `UiModalDemoLauncher` | `UIW-11`; `UiModalPresenterTests`; `UiFeedbackWindowLifecycleTests`; integrated acceptance test | Done |
| Hints behavior | `UiFeedbackService`, `UiHintsView`, `UiHintsPresenter`, top-center hint trigger | `UIW-11`; `UIW-23`; `UiFeedbackWindowLifecycleTests`; integrated acceptance test | Done |
| FX collect/spend/source-target behavior | `UiFeedbackService`, `UiFxView`, `UiFxPresenter`, `UiFxTargetRegistry`, source-aware FX requests | `UIW-11`; `UIW-19`; `UIW-22`; `UiFeedbackPresenterTests`; `UiFeedbackWindowLifecycleTests`; integrated acceptance test | Done |
| Object indicators / dynamic UI layer / reward source | `UiObjectIndicatorView.prefab`, object indicator presenter/window/source, world-to-screen adapter, character reward target | `UIW-22`; `UiObjectIndicatorPresenterTests`; `UiObjectIndicatorWindowLifecycleTests`; integrated acceptance test | Done |
| OpenUI top/down scheme composition represented in SampleScene | Auto-started top-left/top-right/top-center/down-right/down-left windows plus overlay windows and navigation launchers | `Assets/Scenes/SampleScene.unity`; `UiTopLeftDemoInstaller`; integrated acceptance test | Done |
| Visual/layout parity and static prefab preview coverage | `docs/uiwindows-mvp-visual-layout-parity-audit.md`, `Assets/Scenes/Develop/UIDevelopScene.unity` | `UIW-24`; Unity scene validation; prefab inspection for all 11 migrated prefabs | Done |
| Integrated SampleScene acceptance workflow | `SampleSceneIntegratedAcceptanceTests` | `UIW-25` focused PlayMode acceptance workflow; full `UiWindowsMvp.Tests.PlayMode` verification | Done |
| Avoid OpenUI infrastructure, Zenject, UniRx, and forbidden lifecycle shortcuts | No OpenUI runtime import; no mandatory Zenject/UniRx; UI opens through `WindowSystem.Show/Hide`; `SetActive` not used for runtime window lifecycle | Static scans over project runtime/scripts/scenes/prefabs; issue closure notes; `UIW-25` verification | Done |
| Test and verification coverage for implemented slices | Presenter tests, lifecycle tests, ProjectContext service tests, final integrated acceptance workflow | `UiWindowsMvp.Tests.PlayMode`, `ProjectContext.Player.Tests.PlayMode`, Unity scene validation, static scans | Done |

## Accepted Deviations

- `UiModalView.prefab` intentionally consolidates OpenUI's separate info OK, OK/Cancel, and wait modal prefabs into one presenter-controlled UI.Windows view.
- `UiObjectIndicatorView.prefab` includes the representative indicator item structure inside the UI.Windows-compatible view instead of preserving OpenUI's separate indicator item prefab split.
- `UiFxView.prefab` has limited idle static preview content because collect/spend items are request-driven at runtime.
- `UiSettingsView.prefab` and `UiShopView.prefab` do not statically populate every runtime-generated language/group/item row in `UIDevelopScene`; those are covered by presenter/model runtime tests.
- OpenUI's Zenject installers, UniRx subscriptions, OpenUI schemes, `UiView.Show/Hide` `SetActive` lifecycle, and presenter-to-presenter/domain-to-presenter coupling are intentionally not ported.

## Closure Position

`UIW-25` is accepted, merged, pushed, and closed. `UIW-1` is closed as complete, with this document and the `UIW-25` Linear note serving as the final evidence package. Further work continues under `UIW-26` as a new production-hardening iteration.

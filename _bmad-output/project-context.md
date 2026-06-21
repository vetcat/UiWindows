---
project_name: UiWindows
user_name: Vitaly
date: 2026-06-06
sections_completed:
  - technology_stack
  - project_goal
  - repository_research
  - implementation_plan
  - linear_task_plan
  - architecture_follow_up
  - reactive_dependency_follow_up
  - risks
existing_patterns_found: 8
status: uiw-18-complete-follow-up-migration-tasks-created
---

# Project Context for AI Agents

This file exists so a new AI chat can quickly recover the project goal, current findings, and constraints before making implementation decisions.

## Current Project State

- Local project path: `/Users/vitaly/Projects/UiWindows`.
- Unity version: `6000.4.4f1` from `ProjectSettings/ProjectVersion.txt`.
- Current repository is a Unity project with base project settings plus project-owned CompositionRoot, R3 integration, UI.Windows MVP adapter code, R3-backed player/settings/localization/shop model services, and modal/hint/FX request ports under `Assets/Scripts`.
- `UI.Windows-submodule` is integrated as a fork-pinned UPM Git dependency.
- R3 is integrated as the explicit reactive foundation for the MVP layer through NuGetForUnity plus the R3.Unity UPM package.
- Free DOTween `1.2.825` is installed from the official Demigiant ZIP source under `Assets/Plugins/Demigiant/DOTween` for UI/effects rendering work.
- The minimal UI.Windows MVP presenter lifecycle adapter is implemented under `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter`.
- OpenUI behavior ports are implemented incrementally without importing OpenUI infrastructure; player lives under `Assets/Scripts/ProjectContext/Runtime/Player`, settings/localization under `Assets/Scripts/ProjectContext/Runtime/Settings` and `Assets/Scripts/ProjectContext/Runtime/Localization`, the shop model under `Assets/Scripts/ProjectContext/Runtime/Shop`, and modal/hint/FX request ports under `Assets/Scripts/ProjectContext/Runtime/UiRequests`.
- `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiTopLeftView.prefab` is the project-owned UI.Windows-compatible UiTopLeft view asset.
- `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiModalView.prefab`, `UiHintsView.prefab`, and `UiFxView.prefab` are project-owned UI.Windows-compatible assets for the `UIW-11` modal, hints, and FX slice.
- `Assets/Scenes/SampleScene.unity` is the canonical runtime/integration scene for UI.Windows MVP vertical slices, including the UiTopLeft runtime wiring.
- `docs/uiwindows-mvp-pooling-lifecycle.md` records the reusable pooling/show-scope verification pattern for future UI.Windows MVP windows.
- `docs/dotween-ui-fx-dependency.md` records the DOTween source/version, setup workflow, generated files, and usage boundary.
- `docs/uiwindows-mvp-openui-migration-guide.md` records the final OpenUI migration rules for UI.Windows lifecycle, CompositionRoot boundaries, R3 request ports, presenter binding, pooling cleanup, DOTween usage, and prefab/scene roles.
- `UIW-17` and `UIW-11` are complete; modal, hints, representative collect/spend FX ports, and final OpenUI migration docs are implemented.
- `UIW-18` completed the parent reconciliation audit and confirmed the original `UIW-1` goal is not yet fully satisfied; follow-up child issues `UIW-19` through `UIW-25` were created from the audit gaps.
- Next ordered child issue: `UIW-19` - `12 - Port UiTopRight coin HUD and coin FX target integration`.
- `Assets/Scenes/Develop/UIDevelopScene.unity` is a static prefab layout-check scene for visually inspecting adapted UI prefabs under a Canvas.
- Do not create one runtime demo scene per UI prefab or slice by default; additional `Assets/Scenes/Develop/*Runtime*` scenes should be exceptional and explicitly requested or justified.
- No OpenUI infrastructure has been imported into this project.
- Process correction from the `UIW-1` readiness review on 2026-06-21: closed child issues do not prove that a parent/umbrella issue is complete. Broad parent goals such as "most OpenUI examples/layouts" require a parent reconciliation review and traceability matrix before parent closure. The `issue-task-flow` skill, `AGENTS.md`, and this context now require that gate for future parent plans.
- `.ai/mcp/mcp.json` is currently empty.
- Working tree was clean after the repository investigation.

## Project Skills

- Use `.agents/skills/uiwindows-mvp-architecture/SKILL.md` before designing, implementing, or reviewing UI.Windows MVP presenters, views, model/read-model ports, R3 UI bindings, OpenUI example ports, show/hide subscription lifetimes, or decisions about where UI logic belongs.
- The skill captures the project-wide MVP interpretation: UI.Windows owns window lifecycle; presenters own view binding and UI behavior; models may be saves, services, controllers, ECS adapters, or combinations exposed through explicit ports; R3 show-scoped subscriptions must be cleaned on hide or pool return.
- The skill now records the initial `UIW-5` adapter API names and verified lifecycle mapping; update it when later tasks materially change presenter/window lifecycle rules.
- Use `.agents/skills/uiwindows-view-prefab-porting/SKILL.md` before porting, creating, reviewing, or fixing UI.Windows view prefab assets, OpenUI visual prefab ports, serialized Unity UI refs, RectTransform layout, CanvasScaler/font readability, or `UIDevelopScene` layout-preview behavior.
- The prefab-porting skill records the `UIW-15` asset workflow: keep prefabs and visual assets outside `Assets/Scripts`, use `Assets/Scenes/Develop/UIDevelopScene.unity` as a static layout-check scene, use `Assets/Scenes/SampleScene.unity` for runtime integration slices, apply correct root RectTransform values to prefab assets rather than scene-only overrides, and separate UI.Windows editor-generated noise from task changes.

## IDE And Unity MCP Verification

- Local `.ai/mcp/mcp.json` can be empty while session-provided MCP tools are still available. Future chats should verify actual Linear, Rider, and Unity MCP availability through the active tool list/resource discovery before implementation or review work.
- If an expected MCP tool is unavailable, times out, or does not see this project, report the exact limitation in the Executor or Orchestrator result instead of assuming the tool is globally unavailable.
- When Rider MCP is available, prefer it for IDE-indexed project navigation and C#-aware operations: use Rider search tools to locate files/usages when indexed search is sufficient; use `rename_refactoring` for C# symbol renames; use `reformat_file`, `get_file_problems`, and `build_solution` for C# formatting and validation when practical.
- Do not force Rider MCP for every file operation. Use shell, `rg`, `apply_patch`, and git tools for raw file reads, diffs, git state, broad scripted inspection, Unity serialized assets, docs, package files, and edits that are clearer as patches.
- If Rider MCP is unavailable, stale, slow, or does not see this project, fall back to normal filesystem tools and report that limitation explicitly.
- When changing project-owned C# code, use Rider MCP when it is available.
- Run Rider `get_file_problems` on changed `.cs` files after edits.
- Run Rider `build_solution` after C# changes when practical, or explicitly report why Unity compile was used instead.
- Use Rider `rename_refactoring` for programmatic symbol renames instead of manual text replacement.
- Use Rider `reformat_file` for edited C# files when formatting changed and the file belongs to the opened solution.
- Unity MCP remains the source of truth for Unity editor refresh/compile, PlayMode verification, Unity Console state, and reflection against live Unity/UI.Windows APIs.
- Before using Unity MCP tools, read `mcpforunity://custom-tools`, `mcpforunity://instances`, and `mcpforunity://editor/state` when available.
- If Rider MCP or Unity MCP is unavailable, times out, or does not see the opened project, report that limitation in the Executor result.

UIW-14 local workflow check on 2026-06-13:

- `UIW-14` is a process/tooling issue, not part of the ordered `UIW-1` migration child sequence.
- The dedicated branch is `feature/uiw-14-flow-harden-executor-mcp-and-closure-verification-workflow`.
- Linear MCP, Rider MCP, and Unity MCP were available in this session even though `.ai/mcp/mcp.json` was empty.
- Unity MCP saw one active `UiWindows@29793614097f6f61` editor instance on Unity `6000.4.4f1`; editor state was idle and ready for tools.
- Rider MCP saw the Unity solution projects and `get_file_problems` plus targeted `build_solution` passed for `Assets/Scripts/CompositionRoot/Samples/SampleCompositionInstaller.cs`.
- Cached `quick_validate.py` scripts in Rider/Codex system skill caches were not executable, so direct execution failed with permission denied. Running the same validator through `python3` worked here because `PyYAML 6.0.3` was installed.
- `tools/quick-validate-skill` is the project-owned wrapper for skill validation. It calls the system `skill-creator` `quick_validate.py` through `python3`, preferring `$CODEX_HOME` or `~/.codex` over Rider versioned cache paths.
- Do not change permissions or contents in system cached skill directories as a task fix. Prefer `tools/quick-validate-skill <skill-directory>` or `python3 <quick_validate.py> <skill-directory>`; if `PyYAML` or the validator is unavailable, document a manual frontmatter fallback validation.
- Pre-existing Unity-generated changes were present in `Packages/nuget-packages/NuGet.config.meta` and `Packages/nuget-packages/packages.config.meta`; treat generated/importer changes as separate from task edits unless they are intentionally accepted.

## Technology Stack

Current local Unity packages from `Packages/manifest.json`:

- `com.cysharp.r3`: Git dependency `https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity#1.3.1`
- `com.github-glitchenzo.nugetforunity`: Git dependency `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity#v4.5.0`
- `com.unity.inputsystem`: `1.19.0`
- `com.unity.render-pipelines.universal`: `17.4.0`
- `com.unity.ugui`: `2.0.0`
- `com.unity.test-framework`: `1.6.0`
- `com.unity.ai.navigation`: `2.0.13`
- `com.coplaydev.unity-mcp`: Git dependency `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main`
- `com.me.ui.windows`: Git dependency `https://github.com/vetcat/UI.Windows-submodule.git#60a4bf6e47c85ad57935f633a53fc3ca8b707167`

Unity-resolved transitive packages for `com.me.ui.windows` include:

- `com.unity.addressables`: resolved to `2.9.1`
- `com.unity.localization`: resolved to `1.5.8`
- `com.unity.ui`: resolved to built-in `2.0.0`

NuGetForUnity-restored packages for R3 include:

- `R3`: `1.3.1`
- `Microsoft.Bcl.AsyncInterfaces`: `6.0.0`
- `Microsoft.Bcl.TimeProvider`: `8.0.0`
- `System.ComponentModel.Annotations`: `5.0.0`
- `System.Runtime.CompilerServices.Unsafe`: `6.0.0`
- `System.Threading.Channels`: `8.0.0`
- `System.Threading.Tasks.Extensions`: `4.5.4`

Imported third-party Unity assets include:

- Free DOTween `1.2.825` from official Demigiant download `https://dotween.demigiant.com/downloads/DOTween_1_2_825.zip`, SHA-256 `689d42944f7076038eb6b87ed4c85ae6951c4aec8eaae4e068524ce55b422c8c`.
- DOTween lives under `Assets/Plugins/Demigiant/DOTween`; generated settings live at `Assets/Resources/DOTweenSettings.asset`.
- `DOTween.Modules.asmdef` is enabled so project-owned asmdef assemblies can reference Unity UI shortcuts through `DOTween.Modules`.

External repositories studied on 2026-06-06:

- `https://github.com/chromealex/UI.Windows-submodule`, commit `60a4bf6e47c85ad57935f633a53fc3ca8b707167`, dated 2026-05-18.
- `https://github.com/vetcat/OpenUI`, commit `f29fca04343c38b79a6dd8aed66e538cfbe8b232`, dated 2026-02-06.

Architecture references reviewed on 2026-06-07:

- `https://github.com/sebas77/Svelto.ECS`, commit `15c336fd1d01be086d0bccde64395e3eab1d834d`, dated 2025-05-01.
- `https://github.com/vetcat/OpenUI`, commit `f29fca04343c38b79a6dd8aed66e538cfbe8b232`, dated 2026-02-06.
- `https://github.com/friflo/Friflo.Engine.ECS`, commit `25dc91b4a6981df2e5500e4bca7fa764430d0b30`, dated 2026-05-01.
- `https://github.com/Leopotam/ecs`, commit `b256e570bdb15a0bea9e664af32953068a2ac1e5`, dated 2025-11-29.

Reactive library references checked on 2026-06-07:

- `https://github.com/neuecc/UniRx`, HEAD `6baeccf6c544c155497164327cca72f28163a578`; GitHub marks the repository as archived, and its README points users to `Cysharp/R3` instead of UniRx.
- `https://github.com/Cysharp/R3`, HEAD `3fed50ae5c7e123073f6e50218b2a0e6310d50b4`; GitHub marks the repository as not archived, and the project supports Unity.

R3 integration decision checked on 2026-06-07:

- `NuGetForUnity` tag `v4.5.0` exists at commit `a7c6b49a0141a5bff9b1983e38137522ef61977d`.
- `R3` tag `1.3.1` exists at commit `f6eed2dd4208dc4ae171c601e799e85f82aca25e`.
- NuGet package `R3` version `1.3.1` exists on nuget.org.
- Official R3 Unity installation requires the NuGet `R3` package plus the `R3.Unity` Unity package.
- `R3.Unity.asmdef` on tag `1.3.1` references precompiled `R3.dll`, `Microsoft.Bcl.TimeProvider.dll`, and `Microsoft.Bcl.AsyncInterfaces.dll`, so Git/OpenUPM Unity package content alone is not enough without the NuGet side.
- `Microsoft.Bcl.AsyncInterfaces` is pinned to NuGet package `6.0.0` because `Microsoft.Bcl.TimeProvider 8.0.0` references assembly version `6.0.0.0`.
- Project restore config and restored artifacts live under `Packages/nuget-packages`, using NuGetForUnity `InPackagesFolder` placement and a dummy embedded `package.json`.

UIW-13 R3 integration snapshot on 2026-06-07:

- `Packages/manifest.json` pins `com.github-glitchenzo.nugetforunity` to Git tag `v4.5.0`.
- `Packages/manifest.json` pins `com.cysharp.r3` to the R3.Unity Git UPM path at tag `1.3.1`.
- `Packages/packages-lock.json` resolves `com.github-glitchenzo.nugetforunity` to commit `a7c6b49a0141a5bff9b1983e38137522ef61977d`.
- `Packages/packages-lock.json` resolves `com.cysharp.r3` to commit `f6eed2dd4208dc4ae171c601e799e85f82aca25e`.
- `Packages/packages-lock.json` includes embedded package `nuget-packages`.
- `Packages/nuget-packages/InstalledPackages` contains committed restored NuGet artifacts and Unity `.meta` import settings.
- Unity `6000.4.4f1` refresh/compile completed with zero Console errors and zero warnings after R3/NuGet integration.
- Compile smoke `UiWindowsMvp.Reactive.R3MvpSmokeCheck.CanCreateReadOnlySurface()` returned `true`.
- No UniRx package dependency was added.

UIW-11 modal/hints/FX migration snapshot on 2026-06-21:

- `ProjectContext.UiRequests` lives under `Assets/Scripts/ProjectContext/Runtime/UiRequests` and exposes R3-backed modal, hint, and UI FX request ports without UI.Windows, Unity UI, presenter, DOTween, OpenUI, Zenject, or UniRx dependencies.
- `UiModalService` exposes `CurrentModal` plus command methods for info OK, info OK/Cancel, wait, close, and completion.
- `UiFeedbackService` exposes transient hint and collect/spend FX request streams.
- `PlayerService` can publish representative coin collect/spend FX through an optional `IUiFeedbackCommands` port while remaining independent from UI presenters and DOTween.
- `UiModalWindow`, `UiHintsWindow`, and `UiFxWindow` are UI.Windows `LayoutWindowType` wrappers opened through `WindowSystem.Show` by narrow project launchers.
- `UiModalPresenter`, `UiHintsPresenter`, and `UiFxPresenter` subscribe through show-scoped `IUiShowScope`; hidden pooled hint/FX overlays do not consume request streams.
- `UiHintsView` and `UiFxView` use DOTween only in `Assets/Scripts/UiWindowsMvp` and kill active sequences on hide/final cleanup.
- `Assets/Scenes/SampleScene.unity` wires the modal/hints/FX prefabs into `UiTopLeftDemoInstaller`; hints and FX open as empty overlay windows so transient request streams are not lost.
- Focused PlayMode coverage includes presenter tests and `UiFeedbackWindowLifecycleTests.ModalHintsAndFx_RunThroughWindowSystemAndCleanShowScopedRequests`.
- `UIW-11` closure verification passed on 2026-06-21: Rider solution build succeeded with no problems, full Unity PlayMode suite passed `38/38`, Unity Console had 0 errors and 0 warnings after clearing TestRunner tooling logs, `git diff --check main...HEAD` passed, no UniRx/Zenject/OpenUI runtime dependency was introduced, DOTween references stayed out of `Assets/Scripts/ProjectContext`, and new runtime code did not use `SetActive(` for lifecycle.

UI.Windows fork workflow established on 2026-06-07:

- Project fork: `https://github.com/vetcat/UI.Windows-submodule`
- Upstream remote: `https://github.com/chromealex/UI.Windows-submodule`
- Compatibility branch: `unity6000-compat`
- Initial pinned commit: `60a4bf6e47c85ad57935f633a53fc3ca8b707167`
- `package.json` is at the repository root, so `?path=` is not required.
- Committed Unity Package Manager target format for `UIW-2`: `"com.me.ui.windows": "https://github.com/vetcat/UI.Windows-submodule.git#60a4bf6e47c85ad57935f633a53fc3ca8b707167"`
- Full workflow documentation: `docs/ui-windows-fork-workflow.md`

UIW-2 integration snapshot on 2026-06-07:

- `Packages/manifest.json` pins `com.me.ui.windows` to `60a4bf6e47c85ad57935f633a53fc3ca8b707167`.
- Unity Package Manager resolves `com.me.ui.windows` as `UI.Windows` version `1.2.7` from Git.
- Unity refresh/compile completed in Unity `6000.4.4f1` with zero console errors and zero warnings.
- No fork compatibility patch was required.
- `UI.Windows.asmdef` still references `FMODUnity`, but no FMOD package or assembly is installed and this did not block compilation in the verified editor state.
- Key public runtime types are available from assembly `UI.Windows`:
  - `UnityEngine.UI.Windows.WindowSystem`
  - `UnityEngine.UI.Windows.WindowBase`
  - `UnityEngine.UI.Windows.WindowTypes.LayoutWindowType`
  - `UnityEngine.UI.Windows.WindowComponent`

UIW-3 architecture skeleton snapshot on 2026-06-07:

- All project-owned C# code lives under `Assets/Scripts`.
- `Assets/Scripts` is the explicit search/edit scope for project logic; view assets, prefabs, art, textures, scenes, settings, and other non-code Unity assets should stay outside it.
- Reusable CompositionRoot runtime code lives under `Assets/Scripts/CompositionRoot/Runtime` in assembly `CompositionRoot.Runtime`.
- `SceneCompositionRoot` is the scene bootstrap component for explicit service/model registration without Zenject, UniRx, UI.Windows, MVP, OpenUI, or a generic DI framework dependency.
- `SceneCompositionRoot` is intended to be reusable for any scene; scene-specific setup belongs in `ICompositionInstaller` components.
- `ServiceRegistry` initializes `IInitializable` services in registration order and disposes `IDisposable` services once in reverse registration order.
- Failed bootstrap disposes the temporary `ServiceRegistry`, leaves `SceneCompositionRoot` not bootstrapped, and rethrows the original exception.
- Sample bootstrap code lives under `Assets/Scripts/CompositionRoot/Samples` and does not depend on OpenUI or UI.Windows.
- PlayMode lifecycle verification lives under `Assets/Scripts/CompositionRoot/Tests/PlayMode` in assembly `CompositionRoot.Tests.PlayMode`.
- UI.Windows presenter adapter code lives under `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` in assembly `UiWindowsMvp.UIAdapter`; this assembly may depend on `CompositionRoot.Runtime` and `UI.Windows`, while `CompositionRoot.Runtime` must not depend on `UiWindowsMvp` or `UI.Windows`.
- R3 compile/convention plumbing lives under `Assets/Scripts/UiWindowsMvp/Runtime/R3Integration` in assembly `UiWindowsMvp.Reactive`; this assembly may depend on `R3` and `R3.Unity`.
- Code organization, CompositionRoot mechanics, bootstrap path, failure behavior, and ownership rules are documented in `docs/project-architecture-skeleton.md`.
- R3 MVP usage boundaries, dependency pins, and restore workflow are documented in `docs/r3-mvp-conventions.md`.

UIW-5 MVP presenter adapter snapshot on 2026-06-08:

- Minimal project-owned UI.Windows MVP adapter lives under `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` in assembly `UiWindowsMvp.UIAdapter`.
- `IUiPresenter` defines presenter initialization, show/hide hooks, and final `IDisposable` cleanup.
- `IWindowPresenter<TWindow>` binds a presenter to a concrete `WindowBase` subtype.
- `IWindowPresenterFactory<TWindow>` creates presenters through explicit dependencies or narrow ports.
- `WindowPresenterBinder.Bind(window, factory)` attaches one presenter binding to a UI.Windows window instance after UI.Windows has produced that instance, normally from a `WindowSystem.Show` or `WindowSystem.ShowSync` callback.
- `WindowPresenterBinding<TWindow>` owns presenter lifecycle forwarding and idempotent final cleanup.
- `IUiShowScope` and `WindowPresenterShowScope` own show-scoped `IDisposable` subscriptions.
- UI.Windows lifecycle mapping is documented in `.agents/skills/uiwindows-mvp-architecture/SKILL.md` and `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md`.
- The adapter intentionally uses `IDisposable` as the subscription boundary instead of depending directly on R3; R3 subscriptions can be added to `IUiShowScope`.
- `WindowPresenterEventSubscription<TWindow>` avoids UI.Windows generic `UnRegister` because that API leaves null delegates that can crash later event dispatch; disposed subscriptions become no-op and release strong references until UI.Windows clears the event registry.
- PlayMode tests live under `Assets/Scripts/UiWindowsMvp/Tests/PlayMode` in assembly `UiWindowsMvp.Tests.PlayMode`.
- Verified during review: Unity compile had zero errors/warnings, full PlayMode suite passed `8/8`, R3 smoke returned `True`, no UniRx/Zenject/OpenUI dependencies were added, and `CompositionRoot.Runtime` remained independent.
- No real UI.Windows prefab/window was opened through `WindowSystem.Show` yet; first real visible slice is deferred to later issues.

UIW-6 player model/service snapshot on 2026-06-13:

- R3-backed player domain/model code lives under `Assets/Scripts/ProjectContext/Runtime/Player` in assembly `ProjectContext.Player`.
- Public ports are split into `IPlayerReadModel`, `IPlayerCommands`, `IPlayerService`, and `IPlayerSettings`.
- `PlayerService` exposes read-only R3 state for health, XP, coins, level, name, and XP progress; mutable `ReactiveProperty<T>` and `Subject<T>` instances remain private.
- Player mutation crosses the boundary through command methods such as `SetHealth`, `AddXp`, `SetCoins`, `RemoveCoins`, and `SetName`.
- Health is clamped to `IPlayerSettings.MaxHealth`.
- XP progression uses OpenUI-style level bounds `[0, 100, 200, 300, 400]` by default, publishes `XpUpdates`, and publishes `LevelUps` when XP raises the player level.
- OpenUI's `PlayerService -> IUiFxViewPresenter` dependency was not preserved. UI-neutral effect requests were not added in this slice because no UIW-7 player binding requires coin FX yet; future FX ports should be explicit request streams rather than presenter dependencies.
- PlayMode/unit tests live under `Assets/Scripts/ProjectContext/Tests/PlayMode` in assembly `ProjectContext.Player.Tests.PlayMode`.
- The player runtime does not reference UI.Windows windows, Unity UI views, presenter interfaces, OpenUI, Zenject, or UniRx.

UIW-15 UiTopLeft view asset/layout preview snapshot on 2026-06-14:

- The adapted UiTopLeft view prefab lives at `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiTopLeftView.prefab`.
- The project-owned layout preview scene lives at `Assets/Scenes/Develop/UIDevelopScene.unity`.
- `UIDevelopScene` is only a static prefab layout-check scene: it contains a Main Camera, Directional Light, Canvas with `CanvasScaler` and `GraphicRaycaster`, EventSystem, and one `UiTopLeftView` prefab instance under the Canvas.
- `UIDevelopScene` uses `CanvasScaler` `Scale With Screen Size`, reference resolution `1280x720`, match `0.5`, and reference pixels per unit `100`. This matches the OpenUI develop scene baseline and avoids shrinking legacy `UnityEngine.UI.Text` too aggressively in small editor Game Views.
- `UiTopLeftView.prefab` root `RectTransform` should carry the top-left HUD placement itself: anchors `(0,1)`, pivot `(0,1)`, anchored position `(32,-32)`, and size delta `(420,520)`. Do not leave those values only as scene instance overrides.
- The scene exists so agents and humans can inspect the top-left HUD layout without running OpenUI or the runtime UI.Windows vertical slice.
- `UIDevelopScene` is not the UI.Windows lifecycle vertical slice. It does not add `UiTopLeftPresenter`, `IPlayerReadModel`/`IPlayerCommands` binding, R3 subscriptions, scene launcher/bootstrap behavior, `WindowSystem.Show` wiring, OpenUI runtime, Zenject, UniRx, schemes, installers, localization, or effects infrastructure.
- Presenter/model/R3 binding and real `WindowSystem.Show` lifecycle verification belong in `Assets/Scenes/SampleScene.unity`, not in `UIDevelopScene`.

UIW-16 UiTopLeft runtime integration snapshot on 2026-06-14:

- `Assets/Scenes/SampleScene.unity` is the canonical runtime/integration scene for UI.Windows MVP vertical slices.
- `SampleScene` keeps its existing Main Camera, Directional Light, Global Volume, and `SceneCompositionRoot` shape, and adds the UiTopLeft installer, `EventSystem`, and `WindowSystem` wiring needed to show the slice through UI.Windows lifecycle.
- `UiTopLeftPresenter`, `UiTopLeftPresenterFactory`, `UiTopLeftWindow`, and the runtime launcher/source code live under `Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows`.
- The presenter uses narrow player ports (`IPlayerReadModel`, `IPlayerCommands`, `IPlayerSettings`), subscribes to R3 read-model surfaces through `IUiShowScope`, and routes health/XP buttons through command methods.
- The former `Assets/Scenes/Develop/UiTopLeftRuntimeDemoScene.unity` was removed. Avoid per-prefab runtime demo scenes unless Vitaly explicitly requests or a strong technical blocker is documented.

UIW-8 pooling lifecycle verification snapshot on 2026-06-14:

- The reusable pooling lifecycle rule is documented in `docs/uiwindows-mvp-pooling-lifecycle.md`, `docs/r3-mvp-conventions.md`, and `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md`.
- `UiTopLeftRuntimeWindowSource` marks the runtime `UiTopLeftWindow` source as pooled with UI.Windows `createPool`, so repeated opens use UI.Windows pool mechanics rather than direct GameObject activation or custom destruction.
- `UiTopLeftDemoLauncher` keeps opening through `WindowSystem.Show` and hiding through UI.Windows `Hide`; on pooled reopen it reuses the existing `WindowPresenterBinding` on the same window instance instead of rebinding a second presenter.
- For pooled UI.Windows MVP windows, there is one presenter binding per pooled window instance. `OnShowBegin` creates a fresh `IUiShowScope` every show, and `OnHideEnd` disposes that scope before the instance returns to the pool.
- Show-scoped R3 model subscriptions, UI button listeners, timers, frame streams, and visible-state handlers belong in `IUiShowScope`. They must not wait for final `OnDeInitialized` cleanup because pooled windows may hide and reopen many times without deinit.
- Final cleanup remains `OnDeInitialized` / `WindowSystem.Clean` / binding `Dispose`, and it must be idempotent.
- UI.Windows `LayoutWindowType` pooling has an important caveat: `LayoutItem.PushToPool()` can clear cached `componentInstance` references while the layout instance remains. Project window wrappers should be resilient after pool reuse; `UiTopLeftWindow.TryGetView` first checks `GetLayoutComponent` and then falls back to `FindComponent<UiTopLeftView>()`.
- `UiTopLeftWindowLifecycleTests.ReopenCyclesThroughWindowSystem_ReusePooledWindowWithoutDuplicateSubscriptions` is the reference PlayMode pattern for future windows: show through `SampleScene`, capture pooled instance/binding/view, repeat show-hide-reopen cycles, verify one button effect per click, verify hidden model updates and hidden clicks do not affect stale UI state, verify reopen reuses the same window instance and binding, then verify final `WindowSystem.Clean` disposal is safe.
- R3 `ObservableTracker` was not made a runtime dependency. The verification relies on observable behavior and lifecycle counters rather than editor diagnostics.
- `UIW-8` verification passed in Unity `6000.4.4f1`: `UiWindowsMvp.Tests.PlayMode` 9/9, full PlayMode suite 19/19, Rider diagnostics/build passed, Unity Console had 0 errors and 0 warnings.

UIW-9 settings/localization slice snapshot on 2026-06-15:

- Settings domain/model code lives under `Assets/Scripts/ProjectContext/Runtime/Settings` in assembly `ProjectContext.Settings`.
- Settings ports are split into `IGameSettingsReadModel`, `IGameSettingsCommands`, and `IGameSettingsService`.
- `GameSettingsService` exposes read-only R3 state for music and sound volume, keeps mutable `ReactiveProperty<float>` instances private, clamps values to `[0, 1]`, and persists values through `PlayerPrefs`.
- Localization domain/model code lives under `Assets/Scripts/ProjectContext/Runtime/Localization` in assembly `ProjectContext.Localization`.
- Localization ports are split into `ILocalizationReadModel`, `ILocalizationCommands`, and `ILocalizationService`.
- `LocalizationService` exposes read-only current-language state and a `LanguageChanged` event stream through R3, keeps mutable R3 primitives private, supports the sample English/French/German/Russian keys needed by the settings slice, and persists the selected language through `PlayerPrefs`.
- The adapted settings view prefab lives at `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiSettingsView.prefab`.
- `Assets/Scenes/Develop/UIDevelopScene.unity` includes a static `UiSettingsView` preview instance alongside existing view previews.
- Runtime settings slice code lives under `Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows`, including `UiSettingsWindow`, `UiSettingsView`, `UiSettingsPresenter`, `UiSettingsPresenterFactory`, `UiSettingsDemoLauncher`, and `UiSettingsRuntimeWindowSource`.
- `Assets/Scenes/SampleScene.unity` wires the settings prefab through the existing `UiTopLeftDemoInstaller` CompositionRoot pattern; `showSettingsOnStart` defaults to false while the launcher remains resolvable for tests or manual opens.
- `UiSettingsPresenter` subscribes to settings/localization read-model state through `IUiShowScope` and removes slider/toggle/button handlers on hide, matching the UIW-8 pooling lifecycle rule.
- `WindowPresenterEventSubscription<TWindow>` now unregisters concrete UI.Windows callbacks on dispose while retaining idempotent binding cleanup; regression coverage still verifies disposed bindings do not receive later UI.Windows events.
- Focused verification for `UIW-9` passed in Unity `6000.4.4f1`: `ProjectContext.Player.Tests.PlayMode` plus `UiWindowsMvp.Tests.PlayMode` 23/23, Rider diagnostics/build passed, Unity Console had 0 errors and 0 warnings, and `git diff --check main...HEAD` passed after YAML whitespace normalization.

UIW-10 shop and collection pooling slice snapshot on 2026-06-16:

- Shop domain/model code lives under `Assets/Scripts/ProjectContext/Runtime/Shop` in assembly `ProjectContext.Shop`.
- Shop ports are split into `IShopReadModel`, `IShopCommands`, and `IShopService`.
- `ShopService` exposes read-only R3 state for selected group, items in selected group, and selected item; mutable `ReactiveProperty<T>` instances remain private and mutations go through `SelectGroup` / `SelectItem`.
- The default shop catalog mirrors the OpenUI sample data: two groups and ten item entries with amounts `10` through `100`, without carrying OpenUI sprite/settings infrastructure into the model layer.
- `LocalizationService` includes the shop keys needed by this slice for English, French, German, and Russian.
- The adapted shop view prefab lives at `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows/UiShopView.prefab`.
- `Assets/Scenes/Develop/UIDevelopScene.unity` includes a static `UiShopView` preview instance alongside existing view previews.
- Runtime shop slice code lives under `Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows/Shop`, including `UiShopWindow`, `UiShopView`, `UiShopPresenter`, `UiShopPresenterFactory`, `UiShopDemoLauncher`, and `UiShopRuntimeWindowSource`.
- `Assets/Scenes/SampleScene.unity` wires the shop prefab through the existing `UiTopLeftDemoInstaller` CompositionRoot pattern; `showShopOnStart` defaults to false while the launcher remains resolvable for tests or manual opens.
- The view-side `PooledViewCollection<TView>` is a narrow Unity UI entry pooling helper only; it is not a reactive framework and it does not use `SetActive` for window lifecycle or item reuse.
- `UiShopPresenter` subscribes to shop/localization read-model state through `IUiShowScope`, rebuilds group/item entries from read-model state, and disposes item/group button handlers both on list rebuild and on hide/pool return.
- Focused verification for `UIW-10` passed in Unity `6000.4.4f1`: targeted shop PlayMode tests 6/6, full PlayMode suite 31/31, scene validation for `SampleScene` and `UIDevelopScene`, Unity Console with 0 errors and 0 warnings, `git diff --check main...HEAD`, and static forbidden-dependency scans. Rider `get_file_problems` passed on changed C# files; Rider `build_solution` returned `isSuccess=false` with an empty problem list, so Unity compile/tests were used as the build authority.

UIW-17 DOTween dependency setup snapshot on 2026-06-21:

- Free DOTween `1.2.825` was downloaded from the official Demigiant ZIP source, not DOTween Pro.
- The imported archive URL is `https://dotween.demigiant.com/downloads/DOTween_1_2_825.zip`; the imported archive SHA-256 is `689d42944f7076038eb6b87ed4c85ae6951c4aec8eaae4e068524ce55b422c8c`.
- Official DOTween assets live under `Assets/Plugins/Demigiant/DOTween`; Unity setup generated `Assets/Resources/DOTweenSettings.asset` and `Assets/Plugins/Demigiant/DOTween/Modules/DOTween.Modules.asmdef`.
- DOTween setup added the `DOTWEEN` scripting define in `ProjectSettings/ProjectSettings.asset`.
- DOTween settings keep Unity UI shortcuts enabled and asmdef compatibility enabled; UI Toolkit, TextMeshPro, and external EPOOutline modules remain disabled until needed.
- `UiWindowsMvp.SampleSceneWindows` references `DOTween` and `DOTween.Modules`, and `DOTweenMvpFxSmokeCheck` plus `DOTweenMvpFxSmokeTests` provide a minimal project-owned compile/runtime smoke for Unity UI tween access from asmdef code.
- DOTween is allowed only in UI/effects rendering code, primarily under `Assets/Scripts/UiWindowsMvp`.
- DOTween types must not appear in `Assets/Scripts/ProjectContext` public domain/model ports.
- DOTween must not replace UI.Windows show/hide/pooling lifecycle. Future presenters/views that create tweens must kill or complete active tweens on hide, pool return, and final disposal as appropriate.
- `UIW-17` review verification passed: `UiWindowsMvp.Tests.PlayMode` 16/16, Rider diagnostics/build passed, Unity Console had 0 errors and 0 warnings, `git diff --check main...HEAD` passed, no DOTween Pro files were imported, and no `DG.Tweening` / `DOTween` references leaked into `Assets/Scripts/ProjectContext`.
- `UIW-11` can use DOTween for representative modal/hints/FX UI rendering and should not repeat DOTween dependency setup.

## Project Goal

Build a Unity UI approach based primarily on `UI.Windows-submodule`, adding a Model-View-View-Presenter / MVP-style architecture similar to `OpenUI`, but without mandatory Zenject and UniRx dependencies.

Target direction:

- Use `UI.Windows-submodule` for window lifecycle, loading, unloading, layout, pooling, and resource management.
- Port most `OpenUI` examples, including prefab/layout content, to the new approach.
- Replace Zenject with a simple scene `CompositionRoot`.
- Replace UniRx with R3 (`Cysharp/R3`) as the deliberate reactive foundation for MVP state, event streams, operators, timers, frame streams, and subscription ownership.
- Do not implement a broad project-owned custom Rx-like framework in parallel with R3.
- Keep DOTween usage acceptable for animation examples unless later explicitly removed.

## Repository Research Summary

### OpenUI Findings

Confirmed from cloned repository `f29fca0`:

- `OpenUI` is a full Unity project, not just a package.
- `OpenUI` uses Zenject and UniRx extensively.
- Core library is in `Assets/Libs/OpenUI`.
- Sample logic is in `Assets/Scripts/SampleScene` and `Assets/Scripts/ProjectContext`.
- Presenter pattern is centered on `UiView`, `UiPresenter<TView>`, `IUiPresenter`, `UiSchemeBuilder`, and `WindowsController`.
- `SampleSceneUiInstaller` instantiates view prefabs under scene canvases named `CanvasUi`, `CanvasFx`, and `CanvasDynamic`.
- Views are ordinary prefab hierarchies under `Assets/Prefabs/UiPrefabs/SampleSceneWindows`.
- OpenUI examples include player data UI, settings, language selection, shop/items, hints, FX, modal windows, localization, collections, and tests.
- OpenUI's UI optimization is limited because most UI views are instantiated through the installer and then shown/hidden, rather than being loaded/unloaded through a resource-aware window system.
- OpenUI is a useful behavior donor for presenter contracts, schemes, window coordination, player HUD, settings, localization, shop collections, hints, FX, modal windows, and selective tests.
- OpenUI is not a good infrastructure donor for this project because its view creation, presenter discovery, signals, initialization, disposal, and tests are tightly coupled to Zenject and UniRx.
- OpenUI `PlayerService` directly depends on UI presenters for FX behavior. Do not preserve that direction. Domain/model services should publish state or effect requests, and the UI layer should decide how to render them.

Important OpenUI constraints:

- Zenject is used not only for dependency injection but also for prefab instantiation, factories, initializable/disposable lifecycle, and resolving all `IUiPresenter` instances.
- UniRx is used for reactive model properties, button observables, presenter show/hide streams, signal streams, timers, frame updates, and subscription disposal.
- Direct copy-paste into the target project is not viable if Zenject and UniRx are not required dependencies.
- Do not copy `UiView.Show()` / `Hide()` semantics based on `gameObject.SetActive` as the window lifecycle mechanism. In this project, UI.Windows `WindowSystem.Show/Hide` remains the lifecycle source of truth.
- Do not copy `DiContainerUiExtensions.BindViewPresenter` as-is. UI.Windows should create/load window instances, and the project-owned MVP adapter should bind presenters after the window instance is available.

### UI.Windows-submodule Findings

Confirmed from cloned repository `60a4bf6`:

- `UI.Windows-submodule` is a UPM package named `com.me.ui.windows`.
- It provides a Screen/Layout/Component model, not an OpenUI-style presenter architecture.
- Core runtime is under `Runtime/Core`, `Runtime/Modules`, `Runtime/Types`, and `Runtime/Components`.
- `WindowSystem.Show` and `WindowSystem.ShowSync` are the correct entry points for opening windows.
- `WindowBase`, `WindowObject`, and `LayoutWindowType` provide lifecycle hooks: `OnInit`, `OnDeInit`, `OnShowBegin`, `OnShowEnd`, `OnHideBegin`, `OnHideEnd`.
- `WindowSystemResources` provides Addressables/custom-loader resource loading, concurrent load deduplication, handler-based reference tracking, `DeleteAll(handler)`, and unload behavior.
- `WindowSystemPools` provides prefab pooling and despawn/respawn behavior.
- `LayoutWindowType` loads layouts and tagged `WindowComponent` resources into layout slots.
- No explicit Zenject, UniRx, MVP, MVVM, or presenter layer was found in `UI.Windows-submodule`.

Important UI.Windows constraints:

- Do not bypass `WindowSystem.Show/Hide`, because that would skip focus, depth/layer handling, full-coverage optimization, lifecycle events, loading, pooling, and cleanup.
- Presenter binding must account for pooling. A pooled window may be hidden and despawned without full `DoDeInit` on every hide.
- `OnDeInit` is suitable for final resource cleanup, but show-scoped subscriptions should be cleaned on `OnHideEnd` or pool add, not only on `OnDeInit`.
- `WindowSystemResources.DeleteAll(windowInstance)` is already called by the UI.Windows lifecycle where applicable, so the MVP layer should cooperate with that handler model rather than replace it.

### CompositionRoot And ECS Architecture Follow-up

Confirmed from Svelto.ECS, OpenUI, Friflo.Engine.ECS, and LeoECS review on 2026-06-07:

- Svelto.ECS treats the composition root as the place that manually creates the runtime root, scheduler, factories/functions, and engines. The core root stays owned by the composition root; narrower capability objects are passed outward.
- Friflo.Engine.ECS uses explicit `EntityStore` and optional `SystemRoot`/`SystemGroup` composition. Structural changes inside query iteration are expected to go through `CommandBuffer`.
- LeoECS uses explicit `EcsWorld` and `EcsSystems` with `Init`, `Run`, and `Destroy`; it supports reflection field injection inside ECS systems, but the repository states that development has stopped and recommends EcsProto or EcsLite instead.
- All three ECS references support the same architectural direction for this project: a future ECS runtime should be owned by a scene or application composition layer, while UI and domain-facing code should depend on narrow ports, not on the concrete ECS world/store/root.

Future ECS integration rules:

- Do not inject or expose a concrete `EntityStore`, `SystemRoot`, `EcsWorld`, `EcsSystems`, or Svelto `EnginesRoot` directly to UI presenters.
- Do not inject the project `IServiceResolver` into presenters, models, or domain services. Use explicit constructor dependencies and narrow ports/capabilities.
- Prefer ports such as `IPlayerReadModel`, `IPlayerCommands`, `IUiEffectRequests`, `IWindowNavigator`, or equivalent names over generic service location.
- Domain/model services must not depend on UI presenter interfaces. If domain logic needs an animation or UI effect, publish an event/request and let the UI adapter handle rendering.
- A future ECS adapter may implement these ports using Friflo, EcsLite/EcsProto, LeoECS, Svelto, or another ECS without changing UI presenter contracts.
- If a scene update/tick loop is needed, add a dedicated update driver service or scene component. Do not overload `SceneCompositionRoot` with per-frame update behavior by default.
- Keep ECS-specific code out of `CompositionRoot.Runtime` unless it is behind project-owned abstractions and the runtime assembly remains independent from the selected ECS library.

### Reactive Dependency Follow-up

Confirmed from UniRx and R3 review on 2026-06-07:

- Do not integrate UniRx by default. The `UniRx` repository is archived, and its README directs users to `Cysharp/R3` instead.
- R3 is now the selected reactive foundation for MVP work. Do not implement `ObservableProperty<T>`, custom event streams, or disposable collections as a parallel framework unless a narrow project-owned adapter is explicitly justified later.
- Prefer read-only reactive surfaces for presenter/model boundaries, normally `ReadOnlyReactiveProperty<T>` for state with current value and `Observable<T>` for event/request streams.
- Keep mutable R3 primitives such as `ReactiveProperty<T>` and `Subject<T>` inside their owning object.
- Use command methods for mutations instead of exposing mutable properties across boundaries.
- `IDisposable` remains the common subscription ownership boundary.
- Show-scoped subscriptions must be disposed on hide/pool cleanup, not only on final `OnDeInit`.

## Proposed Architecture Direction

Add a thin project-owned adapter layer on top of `UI.Windows`, not inside OpenUI and not by rewriting the UI.Windows resource system.

Suggested layer names are provisional:

- `SceneCompositionRoot`: creates project services, models, event bus, and UI registry for a scene.
- `IUiPresenter`: non-generic presenter contract with `Initialize`, `Dispose`, `OnShow`, `OnHide`, `Show`, `Hide`, and `Lock` semantics as needed.
- `UiPresenter<TWindow>` or `WindowPresenter<TWindow>`: base class for presenter logic bound to a `WindowBase` or `LayoutWindowType` instance.
- `WindowPresenterBinder`: attaches a presenter to a loaded/shown UI.Windows window and disposes show-scoped state on hide/pool.
- `SimpleSignalBus`: minimal replacement for Zenject `SignalBus`.
- R3 (`ReadOnlyReactiveProperty<T>`, `Observable<T>`, `ReactiveProperty<T>`, `Subject<T>`, `DisposableBag`, `CompositeDisposable` where appropriate): deliberate replacement for UniRx. Mutable R3 primitives remain owner-private.
- `IPlayerReadModel` and `IPlayerCommands` or equivalent ports: expose player state and mutations to UI without binding presenters to a future ECS world/store.
- `IUiEffectRequests` or equivalent: allows domain/application services to request visual feedback without depending on UI presenter implementations.

Expected lifecycle model:

- CompositionRoot owns application and scene services.
- A future ECS composition installer may own ECS world/store/system-root creation, update driver registration, and disposal.
- UI.Windows owns window instance creation, loading, layout, show/hide, pooling, and resource cleanup.
- Presenter owns UI behavior and model binding for a loaded window instance.
- Presenter should not instantiate windows directly unless it calls `WindowSystem.Show` or a wrapper around it.
- Presenter should not manually destroy pooled UI.Windows windows.
- Presenters should depend on explicit ports/capabilities, not on `IServiceResolver`, Zenject `DiContainer`, or concrete ECS runtime objects.

## Migration Plan

Preferred implementation sequence:

1. Import or reference `UI.Windows-submodule` and verify compilation in Unity `6000.4.4f1`.
2. Fix package compatibility issues before writing MVP code.
3. Create the minimal CompositionRoot and integrate R3 as the reactive foundation.
4. Create one vertical slice based on OpenUI's `UiTopLeftView` and player model.
5. Verify that a UI.Windows window can load, attach presenter, show, update from model, hide, return to pool, and show again without duplicate subscriptions.
6. Port `UiTopRightView`, `UiDownRightView`, `UiSettingsView`, and localization behavior.
7. Port `UiShopView` and collection pooling behavior.
8. Port modal windows, hints, object indicators, and FX after core lifecycle is stable.
9. Add editor/playmode tests equivalent to OpenUI's existing tests, but using CompositionRoot test setup instead of Zenject `TestBase`.

## Linear Task Plan

Linear MCP access was verified on 2026-06-06.

Use the Linear team `UiWindows` for implementation tracking:

- Team ID: `262ab550-90a9-4030-9dca-476abfa8caca`
- Available statuses: `Backlog`, `Todo`, `In Progress`, `Done`, `Canceled`, `Duplicate`
- Available labels: `Feature`, `Improvement`, `Bug`

Main parent issue:

- `UIW-1` - `[Plan] UI.Windows MVP migration with R3, without Zenject/UniRx`
- URL: https://linear.app/white-rabbits-rabbit-hole/issue/UIW-1/plan-uiwindows-mvp-migration-with-r3-without-zenjectunirx

Future AI chat workflow:

1. Read `AGENTS.md`.
2. Read this `project-context.md`.
3. Open Linear issue `UIW-1`.
4. Review child issues ordered by numeric prefix.
5. Pick the first child issue that is not `Done` or `Canceled`, unless Vitaly explicitly chooses another task.
6. Verify available Linear, Rider, Unity, and validation tooling through actual session tools/resources; report unavailable or timed-out tooling explicitly.
7. Start from the latest `main`, then create a dedicated branch for that Linear issue.
8. Use `feature/<issue-slug>` for Executor task branches. When Linear provides a generated branch name such as `owner/uiw-2-task-title`, preserve the generated issue slug but replace the leading owner namespace with `feature/`, for example `feature/uiw-2-task-title`. If no generated slug is available, use `feature/<issue-id>-<normalized-task-title>`.
9. Work only on that issue's scope, verify its acceptance criteria, rerun git status after editor/tool checks, then update Linear status and notes.
10. After the task is accepted/closed, merge the task branch back into `main`, push `main`, verify local `main` is not still ahead of `origin/main`, and update any relevant parent-plan next-task marker.

Current child issue sequence:

- `UIW-12` - `00 - Establish UI.Windows fork and pinned UPM dependency workflow`
- `UIW-2` - `01 - Integrate UI.Windows-submodule and resolve Unity compatibility`
- `UIW-3` - `02 - Create project architecture skeleton and scene CompositionRoot`
- `UIW-13` - `03 - Integrate R3 reactive foundation for MVP`
- `UIW-4` - `03x - Canceled: custom reactive primitives superseded by R3`
- `UIW-5` - `04 - Implement MVP presenter lifecycle adapter for UI.Windows with R3`
- `UIW-6` - `05 - Port Player model/service from OpenUI with R3` - Done
- `UIW-7` - `06 - Build first vertical slice: UiTopLeft on UI.Windows MVP with R3` - Done
- `UIW-8` - `07 - Verify pooling and R3 subscription lifecycle for MVP windows` - Done
- `UIW-9` - `08 - Port settings and localization slice with R3` - Done
- `UIW-10` - `09 - Port shop and collection pooling slice with R3` - Done
- `UIW-17` - `09a - Install DOTween dependency for UI.Windows MVP FX migration` - Done
- `UIW-11` - `10 - Port modal, hints, FX examples and finalize R3 migration docs` - Done
- `UIW-18` - `11 - Audit UIW-1 completion against original OpenUI migration scope` - Done
- `UIW-19` - `12 - Port UiTopRight coin HUD and coin FX target integration` - Todo
- `UIW-20` - `13 - Port UiDownRight settings launcher into SampleScene navigation` - Todo
- `UIW-21` - `14 - Port UiDownLeft shop launcher and shop/modal interaction` - Todo
- `UIW-22` - `15 - Port object indicator dynamic UI layer and character reward source integration` - Todo
- `UIW-23` - `16 - Port UiTopCenter time and press-hold hint example` - Todo
- `UIW-24` - `17 - Visual/layout parity and UIDevelopScene preview coverage audit` - Todo
- `UIW-25` - `18 - Integrated SampleScene acceptance workflow and final UIW-1 reconciliation` - Todo

Parent/umbrella issue closure rule:

- Do not treat all child issues being `Done` or `Canceled` as parent completion.
- Before recommending closure for `UIW-1` or any future parent/umbrella plan, build a traceability matrix: parent target -> implemented artifact(s) -> verification evidence -> status (`Done`, `Partial`, `Deferred`, `Missing`).
- Broad parent wording such as "most", "representative", "finalize", "complete", "migration", or "ready" must be converted into an explicit coverage list before closure.
- If completed child scopes are narrower than the original parent goal, call out the mismatch and either create follow-up tasks or ask Vitaly for explicit reduced-scope acceptance.

## AI Role Workflow

Future chats should use explicit role modes when possible.

Orchestrator mode:

- Use when Vitaly asks a chat to coordinate, delegate, prepare prompts, review another chat's work, or decide the next task.
- Do not implement the selected task directly unless Vitaly explicitly asks.
- Read `AGENTS.md`, this context file, `UIW-1`, and the relevant child issue.
- Pick the first child issue under `UIW-1` that is not `Done` or `Canceled`, unless Vitaly chooses another task.
- Map the selected child issue to the parent acceptance target it advances and note parent targets that remain open or intentionally deferred.
- Verify available MCP tools before preparing or reviewing task work.
- Verify blockers before creating implementation prompts.
- Create one focused Executor prompt for one Linear issue.
- After Executor completion, review diff, acceptance criteria, verification evidence, Linear notes, branch name, and git hygiene.
- If all child issues under a parent are closed, run the parent reconciliation review before saying the parent is complete.
- After acceptance, merge the task branch into `main`, push `main`, verify `ahead origin/main` is clear, and update Linear/parent-plan notes only when Vitaly asks to complete closure.

Executor mode:

- Use when Vitaly provides a specific task or an Executor prompt.
- Work on exactly one Linear issue.
- Start from latest `main`, create a dedicated `feature/<issue-slug>` branch, and derive the issue slug from the Linear-generated branch name when available by replacing its leading owner namespace with `feature/`.
- Stay within the issue scope.
- If the issue is a child of a parent plan, report which parent acceptance target was advanced and which known parent gaps remain outside this issue's scope.
- Run verification from the issue and project context, including Rider MCP for changed C# files when available and Unity MCP for Unity editor/Console/test state.
- Report unavailable, timed-out, or skipped MCP tooling explicitly.
- Update Linear with implementation notes and verification results.
- Return changed files, branch name, commits, verification results, and unresolved risks.
- Do not merge to `main` or close the task unless Vitaly explicitly asks.

## Acceptance Targets

The future implementation should preserve these OpenUI behaviors where practical:

- Player health, XP, level, coins, and name updates propagate to visible UI.
- Buttons update model state and visible UI.
- Settings UI changes sound/music values and localization language.
- Shop UI shows item groups and item details.
- Modal info/wait windows can be shown and closed.
- Hints and FX examples still work.
- Windows can be loaded/unloaded or pooled through UI.Windows rather than permanently instantiated in scene installers.
- Reopening a pooled window does not duplicate button subscriptions or model subscriptions.

## Known Risks And First Checks

- `UI.Windows-submodule` package metadata is old: `unity` is `2019.1`, Addressables is `1.19.4`, Localization is `0.11.1-preview`. Unity `6000.4.4f1` resolved newer compatible registry versions during `UIW-2`.
- `UI.Windows` runtime asmdef references `FMODUnity`; this did not block compilation during `UIW-2`, but if it blocks later, patch the fork rather than adding FMOD by default.
- `OpenUI/main` is Unity `2022.3.39f1`; local project is Unity `6000.4.4f1`.
- OpenUI view prefabs use MonoBehaviours derived from `UiView`; these must be adapted to UI.Windows roots/components before they can be used directly.
- Zenject replacement is straightforward but touches constructors, factories, installer assets, initialization order, and tests.
- UniRx replacement is broader because it touches model properties, button observables, signal streams, timers, frame updates, and subscription disposal.
- R3 package availability depends on committed `Packages/nuget-packages/InstalledPackages` artifacts or NuGetForUnity restore. If artifacts are absent on a fresh checkout, Unity may compile before NuGet restore; use NuGetForUnity CLI restore before first Unity launch when available, or let Unity/NuGetForUnity restore and recompile after ignoring the initial missing-assembly prompt.
- Future ECS selection is intentionally unresolved. Friflo.Engine.ECS, EcsLite/EcsProto, Svelto, or another ECS should be hidden behind project-owned ports so UI code does not need to be rewritten when the ECS choice is made.
- LeoECS classic (`https://github.com/Leopotam/ecs`) is marked by its author as discontinued; use it as a lifecycle reference only unless Vitaly explicitly chooses it despite that status.
- OpenUI contains useful behavior examples but also contains a domain-to-UI dependency in `PlayerService` for FX. Preserve the behavior through events/ports, not the dependency direction.

## UIW-2 Compatibility Policy

When starting `UIW-2`, add the fork-pinned Unity Package Manager dependency first and use Unity Package Manager resolution plus Unity Console/compiler output as the source of truth.

Preferred initial dependency target:

```json
"com.me.ui.windows": "https://github.com/vetcat/UI.Windows-submodule.git#60a4bf6e47c85ad57935f633a53fc3ca8b707167"
```

Do not preemptively rewrite package metadata or runtime code before observing actual Unity `6000.4.4f1` resolution/compile failures.

Dependency and patch rules:

- Let Unity resolve transitive package dependencies first.
- Add explicit project-level dependencies only when Unity resolution or compiler errors prove they are required.
- Keep `Packages/manifest.json` changes limited to package references and versions needed for compilation.
- If `FMODUnity` asmdef references block compilation, patch the fork rather than adding FMOD to this project by default.
- Prefer minimal compatibility patches in the fork: package metadata, asmdef references, optional integration guards, or narrow runtime compatibility fixes.
- Do not put MVP adapter, CompositionRoot, OpenUI ports, project models, presenters, or sample UI behavior into the fork.
- Document every fork compatibility patch with the upstream commit, reason, exact files changed, and Unity verification result.

`UIW-2` verification gate:

- Unity Package Manager resolves the fork-pinned dependency without package-resolution errors.
- Editor compilation completes with zero errors.
- Unity Console has no package or compile errors after import/refresh.
- `com.me.ui.windows` is present in resolved packages.
- Key UI.Windows runtime assemblies/types are available to project scripts, for example `WindowSystem`, `WindowBase`, and `LayoutWindowType` or their actual discovered equivalents.
- Any remaining warnings must be documented and classified as non-blocking.
- If availability cannot be reliably verified through Unity MCP/editor reflection, add only a minimal compile-only smoke test or editor check that references public UI.Windows API. Do not create MVP, scenes, windows, OpenUI ports, or vertical-slice behavior in `UIW-2`.
- If a fork compatibility patch is required, update the project dependency pin to the resulting fork commit or immutable tag and document before/after verification.

## Rules For Future AI Agents

- Do not start by importing all OpenUI code blindly.
- Do not reintroduce mandatory Zenject or UniRx unless Vitaly explicitly changes the requirement.
- Do not treat "without UniRx" as "must write a full custom reactive framework". R3 is the selected reactive foundation; avoid parallel custom Rx primitives.
- Keep R3 usage deliberate and localized; prefer read-only reactive surfaces, command methods for mutations, and `IDisposable` ownership boundaries over leaking mutable R3 types everywhere.
- Do not bypass `WindowSystem.Show/Hide` for window lifecycle.
- Keep the first implementation as a small vertical slice before porting all examples.
- Treat `UI.Windows` lifecycle and pooling as the source of truth.
- Prefer project-owned adapter code over modifying third-party package internals unless a package compatibility fix is unavoidable.
- Use OpenUI as a behavior and test reference, not as an infrastructure template.
- Do not pass `IServiceResolver`, Zenject `DiContainer`, or concrete ECS world/store/root objects into presenters or domain services.
- Introduce explicit ports/capabilities when a dependency boundary may later be backed by ECS.
- Keep domain/model services independent from UI presenter interfaces; UI effects should be requested through events or ports and rendered by the UI layer.
- Before editing files, check git status and avoid overwriting unrelated user changes.
- Implement Linear tasks in dedicated `feature/<issue-slug>` branches from latest `main`, then merge completed task work back to `main` after acceptance.

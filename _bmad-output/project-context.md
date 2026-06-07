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
status: compositionroot-ecs-reactive-followup-recorded
---

# Project Context for AI Agents

This file exists so a new AI chat can quickly recover the project goal, current findings, and constraints before making implementation decisions.

## Current Project State

- Local project path: `/Users/vitaly/Projects/UiWindows`.
- Unity version: `6000.4.4f1` from `ProjectSettings/ProjectVersion.txt`.
- Current repository is a mostly empty Unity project with base `Assets`, `Packages`, and `ProjectSettings` only.
- `UI.Windows-submodule` is integrated as a fork-pinned UPM Git dependency.
- R3 is integrated as the explicit reactive foundation for the MVP layer through NuGetForUnity plus the R3.Unity UPM package.
- No OpenUI code has been imported into this project yet.
- `.ai/mcp/mcp.json` is currently empty.
- Working tree was clean after the repository investigation.

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
- Future UI.Windows presenter adapter code is reserved under `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` in assembly `UiWindowsMvp.UIAdapter`; this assembly may depend on `CompositionRoot.Runtime` and `UI.Windows`, while `CompositionRoot.Runtime` must not depend on `UiWindowsMvp` or `UI.Windows`.
- R3 compile/convention plumbing lives under `Assets/Scripts/UiWindowsMvp/Runtime/R3Integration` in assembly `UiWindowsMvp.Reactive`; this assembly may depend on `R3` and `R3.Unity`.
- Presenter lifecycle adapter work is still deferred to `UIW-5`.
- Code organization, CompositionRoot mechanics, bootstrap path, failure behavior, and ownership rules are documented in `docs/project-architecture-skeleton.md`.
- R3 MVP usage boundaries, dependency pins, and restore workflow are documented in `docs/r3-mvp-conventions.md`.

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

- `UIW-1` - `[Plan] UI.Windows MVP migration without mandatory Zenject/UniRx`
- URL: https://linear.app/white-rabbits-rabbit-hole/issue/UIW-1/plan-uiwindows-mvp-migration-without-mandatory-zenjectunirx

Future AI chat workflow:

1. Read `AGENTS.md`.
2. Read this `project-context.md`.
3. Open Linear issue `UIW-1`.
4. Review child issues ordered by numeric prefix.
5. Pick the first child issue that is not `Done` or `Canceled`, unless Vitaly explicitly chooses another task.
6. Start from the latest `main`, then create a dedicated branch for that Linear issue.
7. Use `feature/<issue-slug>` for Executor task branches. When Linear provides a generated branch name such as `owner/uiw-2-task-title`, preserve the generated issue slug but replace the leading owner namespace with `feature/`, for example `feature/uiw-2-task-title`. If no generated slug is available, use `feature/<issue-id>-<normalized-task-title>`.
8. Work only on that issue's scope, verify its acceptance criteria, then update Linear status and notes.
9. After the task is accepted/closed, merge the task branch back into `main` and push `main` so the next task starts from the integrated state.

Current child issue sequence:

- `UIW-12` - `00 - Establish UI.Windows fork and pinned UPM dependency workflow`
- `UIW-2` - `01 - Integrate UI.Windows-submodule and resolve Unity compatibility`
- `UIW-3` - `02 - Create project architecture skeleton and scene CompositionRoot`
- `UIW-4` - `03 - Implement minimal event, observable, and disposable primitives`
- `UIW-5` - `04 - Implement MVP presenter lifecycle adapter for UI.Windows`
- `UIW-6` - `05 - Port Player model/service from OpenUI without UniRx`
- `UIW-7` - `06 - Build first vertical slice: UiTopLeft on UI.Windows MVP`
- `UIW-8` - `07 - Verify pooling and resource lifecycle for MVP windows`
- `UIW-9` - `08 - Port settings and localization slice`
- `UIW-10` - `09 - Port shop and collection pooling slice`
- `UIW-11` - `10 - Port modal, hints, FX examples and finalize migration docs`

## AI Role Workflow

Future chats should use explicit role modes when possible.

Orchestrator mode:

- Use when Vitaly asks a chat to coordinate, delegate, prepare prompts, review another chat's work, or decide the next task.
- Do not implement the selected task directly unless Vitaly explicitly asks.
- Read `AGENTS.md`, this context file, `UIW-1`, and the relevant child issue.
- Pick the first child issue under `UIW-1` that is not `Done` or `Canceled`, unless Vitaly chooses another task.
- Verify blockers before creating implementation prompts.
- Create one focused Executor prompt for one Linear issue.
- After Executor completion, review diff, acceptance criteria, verification evidence, Linear notes, branch name, and git hygiene.
- After acceptance, merge the task branch into `main`, push `main`, and update Linear only when Vitaly asks to complete closure.

Executor mode:

- Use when Vitaly provides a specific task or an Executor prompt.
- Work on exactly one Linear issue.
- Start from latest `main`, create a dedicated `feature/<issue-slug>` branch, and derive the issue slug from the Linear-generated branch name when available by replacing its leading owner namespace with `feature/`.
- Stay within the issue scope.
- Run verification from the issue and project context.
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

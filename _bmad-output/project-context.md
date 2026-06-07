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
  - risks
existing_patterns_found: 8
status: active-research-no-code-implementation-yet
---

# Project Context for AI Agents

This file exists so a new AI chat can quickly recover the project goal, current findings, and constraints before making implementation decisions.

## Current Project State

- Local project path: `/Users/vitaly/Projects/UiWindows`.
- Unity version: `6000.4.4f1` from `ProjectSettings/ProjectVersion.txt`.
- Current repository is a mostly empty Unity project with base `Assets`, `Packages`, and `ProjectSettings` only.
- No UI.Windows or OpenUI code has been imported into this project yet.
- `.ai/mcp/mcp.json` is currently empty.
- Working tree was clean after the repository investigation.

## Technology Stack

Current local Unity packages from `Packages/manifest.json`:

- `com.unity.inputsystem`: `1.19.0`
- `com.unity.render-pipelines.universal`: `17.4.0`
- `com.unity.ugui`: `2.0.0`
- `com.unity.test-framework`: `1.6.0`
- `com.unity.ai.navigation`: `2.0.13`
- `com.coplaydev.unity-mcp`: Git dependency `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main`

External repositories studied on 2026-06-06:

- `https://github.com/chromealex/UI.Windows-submodule`, commit `60a4bf6e47c85ad57935f633a53fc3ca8b707167`, dated 2026-05-18.
- `https://github.com/vetcat/OpenUI`, commit `f29fca04343c38b79a6dd8aed66e538cfbe8b232`, dated 2026-02-06.

UI.Windows fork workflow established on 2026-06-07:

- Project fork: `https://github.com/vetcat/UI.Windows-submodule`
- Upstream remote: `https://github.com/chromealex/UI.Windows-submodule`
- Compatibility branch: `unity6000-compat`
- Initial pinned commit: `60a4bf6e47c85ad57935f633a53fc3ca8b707167`
- `package.json` is at the repository root, so `?path=` is not required.
- Committed Unity Package Manager target format for `UIW-2`: `"com.me.ui.windows": "https://github.com/vetcat/UI.Windows-submodule.git#60a4bf6e47c85ad57935f633a53fc3ca8b707167"`
- Full workflow documentation: `docs/ui-windows-fork-workflow.md`

## Project Goal

Build a Unity UI approach based primarily on `UI.Windows-submodule`, adding a Model-View-View-Presenter / MVP-style architecture similar to `OpenUI`, but without mandatory Zenject and UniRx dependencies.

Target direction:

- Use `UI.Windows-submodule` for window lifecycle, loading, unloading, layout, pooling, and resource management.
- Port most `OpenUI` examples, including prefab/layout content, to the new approach.
- Replace Zenject with a simple scene `CompositionRoot`.
- Replace UniRx with minimal local primitives only where needed, such as simple observable properties, event streams, and disposable collections.
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

Important OpenUI constraints:

- Zenject is used not only for dependency injection but also for prefab instantiation, factories, initializable/disposable lifecycle, and resolving all `IUiPresenter` instances.
- UniRx is used for reactive model properties, button observables, presenter show/hide streams, signal streams, timers, frame updates, and subscription disposal.
- Direct copy-paste into the target project is not viable if Zenject and UniRx are not required dependencies.

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

## Proposed Architecture Direction

Add a thin project-owned adapter layer on top of `UI.Windows`, not inside OpenUI and not by rewriting the UI.Windows resource system.

Suggested layer names are provisional:

- `SceneCompositionRoot`: creates project services, models, event bus, and UI registry for a scene.
- `IUiPresenter`: non-generic presenter contract with `Initialize`, `Dispose`, `OnShow`, `OnHide`, `Show`, `Hide`, and `Lock` semantics as needed.
- `UiPresenter<TWindow>` or `WindowPresenter<TWindow>`: base class for presenter logic bound to a `WindowBase` or `LayoutWindowType` instance.
- `WindowPresenterBinder`: attaches a presenter to a loaded/shown UI.Windows window and disposes show-scoped state on hide/pool.
- `SimpleSignalBus`: minimal replacement for Zenject `SignalBus`.
- `ObservableProperty<T>` and `DisposableBag`: minimal replacement for the subset of UniRx used by the examples.

Expected lifecycle model:

- CompositionRoot owns application and scene services.
- UI.Windows owns window instance creation, loading, layout, show/hide, pooling, and resource cleanup.
- Presenter owns UI behavior and model binding for a loaded window instance.
- Presenter should not instantiate windows directly unless it calls `WindowSystem.Show` or a wrapper around it.
- Presenter should not manually destroy pooled UI.Windows windows.

## Migration Plan

Preferred implementation sequence:

1. Import or reference `UI.Windows-submodule` and verify compilation in Unity `6000.4.4f1`.
2. Fix package compatibility issues before writing MVP code.
3. Create the minimal CompositionRoot and local event/disposable primitives.
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
7. Use the Linear-generated branch name when available; if not available, use `<issue-id>-<normalized-task-title>` so the branch name matches the task identity without unsafe Git characters.
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

- `UI.Windows-submodule` package metadata is old: `unity` is `2019.1`, Addressables is `1.19.4`, Localization is `0.11.1-preview`.
- `UI.Windows` runtime asmdef references `FMODUnity`; if FMOD is not installed this may block compilation and may need an asmdef/reference adjustment.
- `OpenUI/main` is Unity `2022.3.39f1`; local project is Unity `6000.4.4f1`.
- OpenUI view prefabs use MonoBehaviours derived from `UiView`; these must be adapted to UI.Windows roots/components before they can be used directly.
- Zenject replacement is straightforward but touches constructors, factories, installer assets, initialization order, and tests.
- UniRx replacement is broader because it touches model properties, button observables, signal streams, timers, frame updates, and subscription disposal.

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
- Do not bypass `WindowSystem.Show/Hide` for window lifecycle.
- Keep the first implementation as a small vertical slice before porting all examples.
- Treat `UI.Windows` lifecycle and pooling as the source of truth.
- Prefer project-owned adapter code over modifying third-party package internals unless a package compatibility fix is unavoidable.
- Before editing files, check git status and avoid overwriting unrelated user changes.
- Implement Linear tasks in dedicated branches from latest `main`, then merge completed task work back to `main` after acceptance.

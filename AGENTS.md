# Agent Instructions

## Context First

Before planning or editing code, read the project context file:

- `_bmad-output/project-context.md`

That file contains the current project goal, repository research summary, proposed architecture direction, migration plan, acceptance targets, known risks, and implementation rules for future AI agents.

## Project Summary

This Unity project is an investigation and implementation workspace for building a UI architecture based on `UI.Windows-submodule`, with an MVP / Model-View-View-Presenter style layer inspired by `OpenUI`.

Core direction:

- Use `UI.Windows-submodule` for window lifecycle, loading, unloading, layouts, pooling, and resource management.
- Use and extend the project-owned MVP adapter layer without mandatory Zenject or UniRx.
- Use `Cysharp/R3` as the explicit reactive foundation for MVP state, event streams, operators, timers, frame streams, and subscription ownership.
- Use a simple scene `CompositionRoot` for dependency wiring.
- Port most `OpenUI` examples and layouts onto the new approach after a small vertical slice is proven.

Do not import all OpenUI code blindly. Do not bypass `WindowSystem.Show/Hide` for UI.Windows lifecycle.
Do not build a parallel custom Rx-like framework now that R3 is integrated.

## Code Organization And CompositionRoot

All project-owned C# code belongs under `Assets/Scripts`. This is the explicit code search scope for programmers and AI agents.

Keep view assets, prefabs, art, textures, scenes, settings, and other non-code Unity assets outside `Assets/Scripts`.

Reusable scene composition infrastructure lives in:

- `Assets/Scripts/CompositionRoot`

UI.Windows-specific MVP adapter code lives in:

- `Assets/Scripts/UiWindowsMvp`

`CompositionRoot` is a reusable scene bootstrap/lifecycle primitive, not a UI concept:

- `SceneCompositionRoot` is intended to be reusable for any scene.
- Each scene should provide scene-specific `ICompositionInstaller` components.
- `ServiceRegistry` initializes `IInitializable` services in registration order.
- `ServiceRegistry` disposes `IDisposable` services once in reverse registration order.
- Failed bootstrap must dispose the temporary registry, leave the root not bootstrapped, and rethrow the original exception.
- `CompositionRoot.Runtime` must not depend on `UiWindowsMvp`, `UI.Windows`, OpenUI, Zenject, UniRx, or R3.

Before changing `CompositionRoot`, scene bootstrap behavior, or code folder organization, read:

- `docs/project-architecture-skeleton.md`

## R3 Reactive Foundation

R3 is already integrated in this project through `UIW-13`.

Current R3 baseline:

- `Packages/manifest.json` pins `com.cysharp.r3` to `https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity#1.3.1`.
- `Packages/manifest.json` pins `com.github-glitchenzo.nugetforunity` to `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity#v4.5.0`.
- `Packages/nuget-packages/packages.config` pins NuGet `R3` at `1.3.1` and the required NuGet dependencies.
- `Packages/nuget-packages/InstalledPackages` contains committed restored NuGet artifacts so fresh checkouts can compile without waiting for a first-launch restore.
- `Assets/Scripts/UiWindowsMvp/Runtime/R3Integration` contains the minimal compile-smoke assembly `UiWindowsMvp.Reactive`.

Before changing R3 usage boundaries, dependency pins, restore workflow, or MVP reactive conventions, read:

- `docs/r3-mvp-conventions.md`

R3 usage rules:

- Public model/read-model ports should expose read-only reactive surfaces, normally `ReadOnlyReactiveProperty<T>` for state with a current value or `Observable<T>` for event/request streams.
- Keep mutable R3 primitives such as `ReactiveProperty<T>` and `Subject<T>` inside their owning object.
- Mutations should cross boundaries through explicit command methods/ports.
- `IDisposable` remains the subscription ownership boundary.
- Show-scoped UI subscriptions must be disposed on hide/pool cleanup, not only on final `OnDeInit`.
- Do not introduce UniRx or a project-owned custom Rx framework in parallel with R3.

## UI.Windows MVP Architecture Skill

Before designing, implementing, or reviewing UI.Windows MVP presenters, views, model/read-model ports, R3 UI bindings, OpenUI example ports, show/hide subscription lifetimes, or decisions about where UI logic belongs, use:

- `.agents/skills/uiwindows-mvp-architecture/SKILL.md`

The skill records the project-wide MVP interpretation and current `UIW-5` adapter API:

- UI.Windows owns window lifecycle, loading, unloading, layouts, pooling, and resource cleanup.
- Presenters own view binding and UI behavior.
- Models may be saves, services, controllers, ECS adapters, or combinations exposed through explicit ports.
- R3 show-scoped subscriptions must be cleaned on hide or pool return.
- The initial adapter lives under `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter`.

## Linear

Linear MCP access was verified on 2026-06-06.

Use Linear for project task tracking when the user asks to create or update tasks.

Known Linear workspace data:

- Team name: `UiWindows`
- Team ID: `262ab550-90a9-4030-9dca-476abfa8caca`
- `UiWindows` was found as a Linear team, not as a Linear project.
- No Linear project named `UiWindows` was found during the check.
- Existing issues in the `UiWindows` team: none at time of check.

Available issue statuses for team `UiWindows`:

- `Backlog`
- `Todo`
- `In Progress`
- `Done`
- `Canceled`
- `Duplicate`

Available issue labels for team `UiWindows`:

- `Feature`
- `Improvement`
- `Bug`

Main Linear task-plan:

- Parent issue: `UIW-1` - `[Plan] UI.Windows MVP migration with R3, without Zenject/UniRx`
- URL: https://linear.app/white-rabbits-rabbit-hole/issue/UIW-1/plan-uiwindows-mvp-migration-with-r3-without-zenjectunirx
- Future agents should open `UIW-1`, inspect child issues ordered by numeric prefix, and pick the first child issue that is not `Done` or `Canceled` unless the user says otherwise.

Current child issue sequence:

- `UIW-12` - `00 - Establish UI.Windows fork and pinned UPM dependency workflow`
- `UIW-2` - `01 - Integrate UI.Windows-submodule and resolve Unity compatibility`
- `UIW-3` - `02 - Create project architecture skeleton and scene CompositionRoot`
- `UIW-13` - `03 - Integrate R3 reactive foundation for MVP`
- `UIW-4` - `03x - Canceled: custom reactive primitives superseded by R3`
- `UIW-5` - `04 - Implement MVP presenter lifecycle adapter for UI.Windows with R3`
- `UIW-6` - `05 - Port Player model/service from OpenUI with R3` - next active task after `UIW-5` closure
- `UIW-7` - `06 - Build first vertical slice: UiTopLeft on UI.Windows MVP with R3`
- `UIW-8` - `07 - Verify pooling and R3 subscription lifecycle for MVP windows`
- `UIW-9` - `08 - Port settings and localization slice with R3`
- `UIW-10` - `09 - Port shop and collection pooling slice with R3`
- `UIW-11` - `10 - Port modal, hints, FX examples and finalize R3 migration docs`

Task creation should use the Linear `save_issue` tool with `team: "UiWindows"`. Do not create test issues just to verify write permission unless the user explicitly asks.

## AI Role Modes

Use explicit role modes when the user wants orchestration or delegated execution.

### Orchestrator

Use this mode when the user says the chat is an orchestrator, asks to delegate a task to another chat, asks for task setup, asks for review/control of an implementation, or asks what should be done next.

The Orchestrator should:

- Read `AGENTS.md`, `_bmad-output/project-context.md`, `UIW-1`, and the relevant child issue.
- Pick the first child issue under `UIW-1` that is not `Done` or `Canceled`, unless Vitaly chooses another task.
- Verify blocker status before preparing implementation work.
- Avoid implementing the task directly unless Vitaly explicitly asks it to.
- Write a focused Executor prompt for exactly one Linear issue.
- Require the Executor to create a dedicated branch from latest `main` using `feature/<issue-slug>`.
- After Executor completion, review git diff, acceptance criteria, verification evidence, Linear comments, and branch hygiene.
- Only recommend merging/closing when acceptance criteria and verification are satisfied.
- After acceptance, merge the task branch into `main`, push `main`, and ensure Linear status/notes are updated when Vitaly asks the Orchestrator to complete the closure.

### Executor

Use this mode when the user gives a specific Linear issue to implement or provides an Executor prompt.

The Executor should:

- Work on exactly one Linear issue.
- Start from latest `main` and create a dedicated task branch.
- Stay inside the selected issue scope.
- Make code/config/docs changes needed for that issue.
- Run the issue's verification steps.
- Update Linear with implementation notes and verification results.
- Return changed files, branch name, commits, verification results, and any unresolved risks.
- Not merge to `main` or close the task unless Vitaly explicitly asks.

## Local MCP Notes

The local `.ai/mcp/mcp.json` file is currently empty, but Linear tools were available in the session environment when checked. If a future agent does not see Linear tools, report that MCP availability differs from the prior session instead of assuming Linear is unavailable globally.

## Repository Hygiene

- Check `git status --short --branch` before editing.
- Implement each Linear task in its own branch created from the latest `main`.
- Use `feature/<issue-slug>` for Executor task branches. When Linear provides a generated branch name such as `owner/uiw-2-task-title`, preserve the generated issue slug but replace the leading owner namespace with `feature/`, for example `feature/uiw-2-task-title`. If no generated slug is available, use `feature/<issue-id>-<normalized-task-title>`.
- Do not implement task work directly on `main`, except for explicitly requested repository-maintenance changes.
- After a task is accepted/closed, merge its branch back into `main` and push `main` so the next task starts from the latest integrated state.
- Do not overwrite unrelated user changes.
- Keep third-party package internals unchanged unless a compatibility fix is unavoidable.
- Prefer a small vertical slice before broad migration work.

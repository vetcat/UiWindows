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

The original migration parent `UIW-1` was closed as complete on 2026-06-27. The active Linear parent for the next iteration is `UIW-26`, focused on production hardening, code readability, maintainability for AI/human agents, and mobile readiness.

Do not import all OpenUI code blindly. Do not bypass `WindowSystem.Show/Hide` for UI.Windows lifecycle.
Do not build a parallel custom Rx-like framework now that R3 is integrated.

## Scene Roles

- `Assets/Scenes/SampleScene.unity` is the canonical runtime/integration scene for UI.Windows MVP vertical slices.
- `Assets/Scenes/Develop/UIDevelopScene.unity` is the static layout/prefab inspection scene.
- Do not create one runtime demo scene per UI prefab or slice by default. Additional `Assets/Scenes/Develop/*Runtime*` scenes should be exceptional and explicitly requested or justified.

## Code Organization And CompositionRoot

All project-owned C# code belongs under `Assets/Scripts`. This is the explicit code search scope for programmers and AI agents.

Keep view assets, prefabs, art, textures, scenes, settings, and other non-code Unity assets outside `Assets/Scripts`.

Reusable scene composition infrastructure lives in:

- `Assets/Scripts/CompositionRoot`

UI.Windows-specific MVP adapter code lives in:

- `Assets/Scripts/UiWindowsMvp`

Project/application model and domain ports live in:

- `Assets/Scripts/ProjectContext`

Current player model/service code lives under `Assets/Scripts/ProjectContext/Runtime/Player`.
This layer may use R3 for read-model state and events, but must not depend on UI.Windows windows, Unity UI views, presenter interfaces, OpenUI, Zenject, or UniRx.

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

## DOTween UI/FX Dependency

DOTween is integrated through `UIW-17` as the explicit tween/animation dependency for UI/effects rendering.

Current DOTween baseline:

- Free DOTween `1.2.825` is imported from the official Demigiant ZIP source under `Assets/Plugins/Demigiant/DOTween`.
- DOTween settings live at `Assets/Resources/DOTweenSettings.asset`.
- `DOTween.Modules.asmdef` is enabled so project-owned asmdef assemblies can reference Unity UI shortcut modules.
- `UiWindowsMvp.SampleSceneWindows` references `DOTween` and `DOTween.Modules`.
- `docs/dotween-ui-fx-dependency.md` records the source URL, archive hash, setup workflow, generated files, and usage boundaries.

DOTween usage rules:

- DOTween is allowed only in UI/effects rendering code, normally under `Assets/Scripts/UiWindowsMvp`.
- Do not expose DOTween types from `Assets/Scripts/ProjectContext` domain/model ports.
- Domain/model services should publish state, commands, or effect requests through project-owned ports; the UI layer decides how to render DOTween animations.
- DOTween must not replace UI.Windows lifecycle ownership. Windows still open and close through `WindowSystem.Show/Hide` or narrow wrappers around those APIs.
- Presenters/views that create tweens must kill or complete active tweens on hide, pool return, and final disposal as appropriate.

## UI.Windows MVP Architecture Skill

Before designing, implementing, or reviewing UI.Windows MVP presenters, views, model/read-model ports, R3 UI bindings, OpenUI example ports, show/hide subscription lifetimes, or decisions about where UI logic belongs, use:

- `.agents/skills/uiwindows-mvp-architecture/SKILL.md`

The skill records the project-wide MVP interpretation and current `UIW-5` adapter API:

- UI.Windows owns window lifecycle, loading, unloading, layouts, pooling, and resource cleanup.
- Presenters own view binding and UI behavior.
- Models may be saves, services, controllers, ECS adapters, or combinations exposed through explicit ports.
- R3 show-scoped subscriptions must be cleaned on hide or pool return.
- The initial adapter lives under `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter`.

## UI.Windows View Prefab Porting Skill

Before porting, creating, reviewing, or fixing UI.Windows view prefab assets, OpenUI visual prefab ports, serialized Unity UI refs, RectTransform layout, CanvasScaler/font readability, or `UIDevelopScene` layout-preview behavior, use:

- `.agents/skills/uiwindows-view-prefab-porting/SKILL.md`

The skill records the project-specific asset workflow learned during `UIW-15`:

- View prefabs live outside `Assets/Scripts`, normally under `Assets/Prefabs/UiWindowsMvp`.
- Supporting visual assets live outside `Assets/Scripts`, normally under `Assets/Content/UiWindowsMvp`.
- `Assets/Scenes/SampleScene.unity` is the runtime integration scene for UI.Windows MVP slices.
- `Assets/Scenes/Develop/UIDevelopScene.unity` is a static layout-check scene, not a UI.Windows lifecycle/runtime slice.
- The preview CanvasScaler baseline is `Scale With Screen Size`, `1280x720`, match `0.5`, reference pixels per unit `100`.
- Correct prefab root RectTransform values should be applied to the prefab asset, not left as scene-only overrides.

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

Completed predecessor plan:

- Parent issue: `UIW-1` - `[Plan] UI.Windows MVP migration with R3, without Zenject/UniRx`
- URL: https://linear.app/white-rabbits-rabbit-hole/issue/UIW-1/plan-uiwindows-mvp-migration-with-r3-without-zenjectunirx
- Status: closed as `Done` on 2026-06-27 after the final reconciliation in `docs/uiwindows-mvp-final-reconciliation.md`.

Main Linear task-plan:

- Parent issue: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`
- URL: https://linear.app/white-rabbits-rabbit-hole/issue/UIW-26/plan-uiwindows-mvp-production-hardening-and-mobile-readiness
- Supporting plan doc: `docs/uiwindows-mvp-production-hardening-plan.md`
- Purpose: harden the completed MVP migration for production-scale Unity/mobile use without reopening the already accepted migration scope.
- Future agents should open `UIW-26`, inspect child issues ordered by numeric prefix, read current child issue statuses from Linear, and pick the first child issue that is not `Done` or `Canceled` unless the user says otherwise.
- Linear child issue status is the source of truth for task progress. Local files such as `AGENTS.md` and `_bmad-output/project-context.md` may record issue order, durable context, and recent snapshots, but must not be treated as the authoritative progress ledger when Linear is available.
- Closing child issues is not enough to close or accept the parent plan. For parent/umbrella issues, run a parent reconciliation review against the original parent description, comments, project context, and current repository state before recommending parent closure.
- Broad parent wording such as "production", "hardening", "mobile readiness", "complete", or "migration" requires an explicit coverage list or traceability matrix: parent target -> implemented artifact -> verification evidence -> status (`Done`, `Partial`, `Deferred`, `Missing`).

Child issue order reference; query Linear for current status:

- `UIW-27` - `00 - Establish production hardening baseline and mobile readiness targets`
- `UIW-28` - `01 - Consolidate UI.Windows MVP launcher and runtime window source infrastructure`
- `UIW-29` - `02 - Define mobile loading, prewarm, and first-show policy`
- `UIW-30` - `03 - Audit and optimize UI layout, raycast, and text settings for mobile`
- `UIW-31` - `04 - Scale dynamic collections and object indicator update paths`
- `UIW-32` - `05 - Move fallback-built UI toward prefab-first production assets`
- `UIW-33` - `06 - Add production hardening validation gates for future agents`
- `UIW-34` - `07 - Final production hardening reconciliation and next-stage readiness review`

Task creation should use the Linear `save_issue` tool with `team: "UiWindows"`. Do not create test issues just to verify write permission unless the user explicitly asks.

## AI Role Modes

Use explicit role modes when the user wants orchestration or delegated execution.

Default task flow after the successful `UIW-20` trial:

- Keep one human-facing chat in Orchestrator mode.
- The intended interaction model is: Vitaly talks to the Orchestrator; the Orchestrator starts and controls specialized agents; durable task communication lives in Linear handoff comments and final notes whenever Linear is available.
- The purpose of this flow is to preserve the Orchestrator's context window for continued delegation, review, repeat follow-up calls, and human-facing decisions. It is not a token-saving rule for any role.
- In this project, when Vitaly asks the Orchestrator to take the next issue, implement a Linear issue, review/control an implementation, or close/finish a task, that is explicit authorization to use the project sub-agent workflow. Vitaly does not need to repeat the words "sub-agent" or "delegate" on every task.
- The Orchestrator owns planning, handoff quality, review, acceptance decisions, retries/follow-ups, and final accountability. It should not spend its context window on low-level implementation, repository mechanics, or tracker closure when a suitable sub-agent path is available.
- When sub-agent tools are available, the Orchestrator must launch one Executor sub-agent for one Linear issue instead of implementing code/config/docs itself or asking Vitaly to open a separate Executor chat.
- After accepting an implementation result, the Orchestrator must launch a Closure Executor sub-agent for mechanical merge/push/Linear closure when sub-agent tools are available, instead of doing routine repository/tracker closure itself.
- A separate Executor or Closure Executor chat remains the fallback when sub-agent tools are unavailable, blocked, unsuitable for the required isolation, or explicitly requested.
- Direct Orchestrator execution of implementation or mechanical closure is a last-resort fallback only when no sub-agent or separate-chat Executor path is available, or when Vitaly explicitly asks the Orchestrator to do the work personally without sub-agents. Lack of the literal word "sub-agent" in a task/closure request is not a valid reason for direct fallback.
- Before launching the Executor, write the full task-specific prompt into the selected Linear issue as the latest comment titled `Executor Handoff`.
- Launch the sub-agent with a short bootstrap prompt containing the project path, issue ID/link, and instructions to read the latest `Executor Handoff` comment. The short prompt is only a pointer to the full task contract, not a reduced-context implementation brief.
- If Linear is unavailable or the handoff comment cannot be created/read, do not use a link-only handoff; either pass the full prompt directly or stop and report the blocker.
- Before launching a Closure Executor, write the authorized closure actions into the selected Linear issue as the latest comment titled `Closure Handoff`.
- The Orchestrator must not edit the shared task branch while a sub-agent Executor is actively implementing, because sub-agents may work in the same checkout rather than an isolated workspace.
- The Orchestrator should avoid full context forks unless the task truly needs the whole thread, but must still give the Executor enough context for safe implementation through Linear or a full direct prompt.
- If the Executor reports `BLOCKED_HUMAN_ACTION_REQUIRED`, for example Unity MCP is blocked by a Unity Editor domain reload or modal confirmation, the Orchestrator must surface the required action in this human-facing chat, wait for confirmation, then resume the same Executor when possible.
- The Orchestrator must independently review the Executor result and request follow-up from the same sub-agent when the fix is inside that implementation context.
- If an Executor `wait_agent` call times out on a Unity-heavy task, the Orchestrator should first use read-only status checks such as `git status --short --branch` and then wait again or request a status update; do not assume failure from one timeout.
- Executor handoffs must require final text hygiene before reporting: changed `.cs`, `.md`, and other text files should end with a final newline, and `git diff --check main...HEAD` should pass.
- Verification scope in Executor handoffs should be focused: require issue-specific tests plus directly relevant existing suites; broad unrelated suites should be run only when touched, cheap, or specifically justified.
- After a Closure Executor reports success, the Orchestrator must run a post-closure documentation drift check before the final human report. At minimum, scan `AGENTS.md`, `_bmad-output/project-context.md`, and relevant docs for the closed issue id/title and stale transient language such as `Todo`, `In Progress`, `review pending`, `implemented on branch`, or `requires closure`.
- If post-closure drift is found, the Orchestrator should make a small context-only commit on `main`, push it, and report that commit. Do not ask the Closure Executor to perform judgment-heavy documentation updates.
- The Orchestrator closes the sub-agent after accepting or abandoning its result.

### Orchestrator

Use this mode when the user says the chat is an orchestrator, asks to delegate a task to an Executor or Closure Executor sub-agent, asks for fallback chat setup, asks for review/control of an implementation, asks to complete closure, or asks what should be done next.

The Orchestrator should:

- Read `AGENTS.md`, `_bmad-output/project-context.md`, `UIW-26`, and the relevant child issue.
- Pick the first child issue under `UIW-26` that is not `Done` or `Canceled`, unless Vitaly chooses another task.
- Map the selected child issue to the parent acceptance target it advances and note parent targets that remain open or intentionally deferred.
- Verify blocker status before preparing implementation work.
- Avoid implementing the task directly unless Vitaly explicitly asks the Orchestrator to do the implementation personally without an Executor.
- Verify multi-agent, Linear, Rider, Unity, and validation tool availability before delegating or reviewing.
- Add a focused `Executor Handoff` comment to the Linear issue, then launch a short-prompt Executor sub-agent for exactly one Linear issue when sub-agent tools are available; otherwise write a focused separate-chat Executor prompt.
- Relay Executor `BLOCKED_HUMAN_ACTION_REQUIRED` reports to Vitaly immediately, including exact human action, branch/status, last successful step, and resume instruction; resume the same Executor after Vitaly confirms when possible.
- Require the Executor to create a dedicated branch from latest `main` using `feature/<issue-slug>`.
- Require the Executor to commit completed task changes before final report unless explicitly blocked.
- After Executor completion, review committed git diff, acceptance criteria, verification evidence, Linear comments, branch/upstream hygiene, and final repository status.
- Run an independent Orchestrator spot-check proportional to risk, for example `git diff --check`, final-newline checks for changed text files, targeted diagnostics, focused tests, or forbidden dependency scans.
- Only recommend merging/closing when acceptance criteria and verification are satisfied.
- If all child issues under a parent are `Done` or `Canceled`, run the parent reconciliation review before saying the parent is complete. If the implemented result is narrower than the original parent goal, either create/follow up missing tasks or ask Vitaly for explicit reduced-scope acceptance.
- After acceptance and when Vitaly asks to complete closure, add a focused `Closure Handoff` comment to Linear, then launch a Closure Executor sub-agent to merge the accepted task branch into `main`, push `main`, update Linear status/final notes, and report final status. The Orchestrator must not perform closure directly unless sub-agent/separate-chat closure is unavailable or Vitaly explicitly requests direct Orchestrator closure.
- After closure succeeds, perform the post-closure documentation drift check, reconcile stale local context with Linear/repository state when needed, commit/push that context-only update, and only then give the final "closed and merged" report.

### Executor

Use this mode when the user gives a specific Linear issue to implement or provides an Executor prompt.

The Executor should:

- Work on exactly one Linear issue.
- Read the latest Linear comment titled `Executor Handoff` when launched from a short bootstrap prompt; if it is missing or Linear is unavailable, stop and report instead of guessing.
- If Unity MCP or another required tool is blocked by a visible human action such as Unity domain reload or modal confirmation, report `BLOCKED_HUMAN_ACTION_REQUIRED` with the exact action needed, current branch/status, last successful step, and resume instruction.
- Start from latest `main` and create a dedicated task branch.
- Do not leave the feature branch misleadingly tracking `origin/main`; unset upstream or push/set upstream to the remote feature branch when appropriate.
- Stay inside the selected issue scope.
- If the issue is a child of a parent plan, report which parent acceptance target was advanced and which known parent gaps remain outside this issue's scope.
- Make code/config/docs changes needed for that issue.
- Run the issue's verification steps.
- Keep verification proportional to the issue: run focused tests and directly relevant existing suites, and call out any broader suite as intentional.
- Before final report, verify changed text files end with a final newline and `git diff --check main...HEAD` passes.
- Update Linear with implementation notes and verification results.
- Commit completed task changes before final report unless blocked or explicitly told not to commit.
- Return changed files, branch name, commits, verification results, final repository status, and any unresolved risks.
- Not merge to `main` or close the task unless Vitaly explicitly asks.

### Closure Executor

Use this mode only when the Orchestrator has accepted an implementation result and provided a `Closure Handoff`.

The Closure Executor should:

- Read the latest Linear comment titled `Closure Handoff`; if it is missing, ambiguous, or Linear is unavailable, stop and report instead of guessing.
- Perform only the authorized mechanical closure actions: merge accepted branch/commit into the named integration branch, push the integration branch, update Linear final note/status, and update parent/plan notes only when explicitly authorized.
- Do not perform post-closure documentation drift review or local context edits. That is Orchestrator responsibility after closure.
- Not review acceptance criteria, edit files manually, resolve merge conflicts, run broad implementation verification, or close parent/umbrella issues without explicit parent reconciliation authorization.
- Stop and report on unexpected dirty working tree, accepted branch/commit mismatch, divergent integration branch, merge conflict, failed push, missing tracker access, or tracker update failure.
- Return merge result, pushed head, Linear updates, final `git status --short --branch`, and any blockers.

## Local MCP Notes

The local `.ai/mcp/mcp.json` file is currently empty, but Linear, Rider, Unity MCP, and multi-agent tools may still be available from the session environment. Before Orchestrator, Executor, or Closure Executor work, verify actual tool availability through the active tool list/resource discovery instead of inferring availability from `.ai/mcp/mcp.json`.

If Linear, Rider, Unity, multi-agent, or another expected tool is unavailable, times out, or does not see this project, report the exact limitation in the prompt, review, Executor result, or Closure Executor result.

## IDE And Unity MCP Verification

When Rider MCP is available, prefer it for IDE-indexed project navigation and C#-aware operations:

- Use Rider search tools (`find_files_by_name_keyword`, `find_files_by_glob`, `search_file`, `search_in_files_by_text`, `search_in_files_by_regex`) to locate project files and usages when indexed search is sufficient.
- Use Rider `rename_refactoring` for programmatic C# symbol renames instead of manual text replacement.
- Use Rider `reformat_file`, `get_file_problems`, and `build_solution` for C# formatting and validation when practical.
- Do not force Rider MCP for every file operation. Use shell, `rg`, `apply_patch`, and git tools for raw file reads, diffs, git state, broad scripted inspection, Unity serialized assets, docs, package files, and edits that are clearer as patches.
- If Rider MCP is unavailable, stale, slow, or does not see this project, fall back to the normal filesystem tools and report that limitation explicitly.

When changing project-owned C# code, use Rider MCP when it is available:

- Run Rider `get_file_problems` on changed `.cs` files after edits.
- Run Rider `build_solution` after C# changes when practical, or explicitly report why Unity compile was used instead.
- Use Rider `rename_refactoring` for programmatic symbol renames instead of manual text replacement.
- Use Rider `reformat_file` for edited C# files when formatting changed and the file belongs to the opened solution.

Unity MCP remains the source of truth for Unity editor refresh/compile, PlayMode verification, Unity Console state, and reflection against live Unity/UI.Windows APIs. If Rider MCP or Unity MCP is unavailable, times out, or does not see the opened project, report that limitation in the Executor result.

Before using Unity MCP tools, read `mcpforunity://custom-tools`, `mcpforunity://instances`, and `mcpforunity://editor/state` when available so the active Unity instance and editor readiness are explicit.

## Skill Validation

When validating project-owned skills, prefer the repository wrapper:

```bash
tools/quick-validate-skill .agents/skills/<skill-name>
```

The wrapper runs the system `skill-creator` `quick_validate.py` through `python3`, using `$CODEX_HOME/skills/.system/skill-creator/scripts/quick_validate.py` when `CODEX_HOME` is set or `~/.codex/skills/.system/skill-creator/scripts/quick_validate.py` otherwise. Set `QUICK_VALIDATE_PY` for a custom validator path or `PYTHON_BIN` for a custom Python executable.

If a cached `quick_validate.py` is present but not executable, run it through the wrapper or with `python3` rather than changing permissions in system cache directories.

If validator execution fails because `PyYAML` or another local dependency is missing, do not modify cached system skills blindly. Use a documented fallback: manually verify `SKILL.md` frontmatter has a `---` block, valid YAML shape where practical, required `name` and `description`, and only allowed keys (`name`, `description`, `license`, `allowed-tools`, `metadata`), then report that fallback validation was used.

## Repository Hygiene

- Check `git status --short --branch` before editing.
- Implement each Linear task in its own branch created from the latest `main`.
- Use `feature/<issue-slug>` for Executor task branches. When Linear provides a generated branch name such as `owner/uiw-2-task-title`, preserve the generated issue slug but replace the leading owner namespace with `feature/`, for example `feature/uiw-2-task-title`. If no generated slug is available, use `feature/<issue-id>-<normalized-task-title>`.
- Do not leave a task branch tracking `origin/main` as its upstream after creation. If the branch is local-only, unset upstream; if it is pushed, track the matching remote feature branch.
- Do not implement task work directly on `main`, except for explicitly requested repository-maintenance changes.
- After Unity, Rider, or editor checks, rerun `git status --short --branch` and separate generated/importer noise from task changes before committing or reporting.
- After a task is accepted/closed, merge its branch back into `main`, push `main`, verify the local branch is not still ahead of `origin/main`, update Linear status/final notes, and update any parent-plan next-task marker when relevant.
- Do not overwrite unrelated user changes.
- Keep third-party package internals unchanged unless a compatibility fix is unavoidable.
- Prefer a small vertical slice before broad migration work.

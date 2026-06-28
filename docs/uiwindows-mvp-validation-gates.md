# UI.Windows MVP Validation Gates

Date: 2026-06-28
Issue: `UIW-33` - `06 - Add production hardening validation gates for future agents`
Parent: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`

## Purpose

This document records the local validation gate future agents should run before
reporting production-hardening work complete. The gate turns repeated manual scans
from earlier `UIW-26` issues into a durable repository command and keeps editor-only
checks explicit.

The gate preserves the accepted architecture:

- UI.Windows owns runtime window lifecycle, loading, pooling, and cleanup.
- Project-owned presenters own UI behavior and show-scoped subscriptions.
- `ProjectContext` stays independent from UI.Windows, Unity UI, presenters, DOTween,
  OpenUI, Zenject, and UniRx.
- `CompositionRoot.Runtime` stays reusable scene bootstrap infrastructure and does
  not depend on UI, MVP, UI.Windows, OpenUI, Zenject, UniRx, or R3.
- R3 remains the explicit reactive foundation; UniRx and a second custom reactive
  framework remain out of scope.

## Local Command

Run from the repository root:

```bash
tools/validate-uiwindows-hardening
```

The command defaults to `main` as the integration branch for diff checks. To validate
against another branch:

```bash
tools/validate-uiwindows-hardening --base origin/main
```

For script-maintenance changes, the lightweight matcher self-test is:

```bash
tools/validate-uiwindows-hardening --self-test
```

## Automated Checks

`tools/validate-uiwindows-hardening` runs these local checks:

| Check | Scope | Fails on |
| --- | --- | --- |
| Forbidden runtime dependencies and lifecycle bypasses | Production code under `Assets/Scripts` excluding test folders, `Assets/Prefabs/UiWindowsMvp`, `Assets/Scenes/SampleScene.unity`, and `Assets/Scenes/Develop/UIDevelopScene.unity` | `UniRx`, `Zenject`, `Libs.OpenUI`, `OpenUI`, `DiContainer`, `SignalBus`, or `SetActive(` |
| `ProjectContext` dependency boundary | `Assets/Scripts/ProjectContext/Runtime` | DOTween, Unity UI/EventSystems, UI.Windows, `UiWindowsMvp`, presenter/binder/window-system types, OpenUI, Zenject, or UniRx |
| `CompositionRoot.Runtime` dependency boundary | `Assets/Scripts/CompositionRoot/Runtime` | DOTween, Unity UI/EventSystems, UI.Windows, `UiWindowsMvp`, presenter/binder/window-system types, OpenUI, Zenject, UniRx, or R3 |
| Diff whitespace | `main...HEAD`, staged diff, and worktree diff | trailing whitespace, conflict markers, and other `git diff --check` findings |
| Final newline | Changed text files from `main...HEAD`, staged diff, worktree diff, and untracked files | non-empty changed text files that do not end with a newline |

The script intentionally does not fail on test-only `SetActive(` usage. Existing test
fixtures may use direct activation to exercise Unity behavior, but production
UI.Windows lifecycle code must continue to use `WindowSystem.Show`/`Hide` or narrow
wrappers around those APIs.

The script also intentionally does not fail on documented dynamic `new GameObject`
paths such as `UiRuntimeWindowSource`, dynamic language/shop rows, and pooled FX
items. Those paths are governed by the prefab-first fallback audit and scaling docs,
not by a broad string ban.

## Mandatory Executor Gates

Every implementation issue should run or account for:

1. `git status --short --branch` before edits and before the final report.
2. `tools/validate-uiwindows-hardening`.
3. `git diff --check main...HEAD` before final report. The validation command runs
   this, but the explicit command remains acceptable evidence in handoffs and review.
4. Final-newline hygiene for changed text files. The validation command checks this
   for changed `.cs`, `.md`, `.asmdef`, `.json`, `.yaml`, `.yml`, `.sh`, `.prefab`,
   `.unity`, and other known text files.
5. `tools/quick-validate-skill .agents/skills/<skill-name>` when a project-owned
   skill is changed.

## Conditional Editor Gates

Shell validation does not replace Rider or Unity evidence.

Run these checks when the touched files make them relevant:

| Change type | Required evidence |
| --- | --- |
| Project-owned C# files | Rider `get_file_problems` on changed `.cs` files and Rider `build_solution` when available. If Rider is unavailable, report the fallback. |
| Project-owned C# files | Unity refresh/compile and Unity Console check after compilation. |
| Unity scenes, prefabs, or serialized assets | Unity scene/prefab validation for directly touched assets plus Unity Console check. |
| UI.Windows lifecycle, presenter binding, R3 subscriptions, pooling, or request streams | Focused PlayMode tests for the affected presenter/window path, plus broader relevant suites when the touched surface is shared. |
| Skill files | `tools/quick-validate-skill` through the repository wrapper. Do not change cached system skill permissions as a task fix. |

Before using Unity MCP tools, read `mcpforunity://custom-tools`,
`mcpforunity://instances`, and `mcpforunity://editor/state`. If Unity reports
external changes dirty, refresh before compile or scene/prefab validation.

## Mobile Evidence Boundary

This local gate provides static and editor-workflow validation only. It is not Android
or iOS target-device evidence.

Future mobile performance work must continue to separate:

- measured Editor evidence;
- static inspection;
- mobile inference; and
- deferred target-device evidence.

Low-end Android startup, cold first-show, frame pacing, memory, and allocation
profiling remain explicit task scope and must not be claimed as complete because this
local validation command passed.

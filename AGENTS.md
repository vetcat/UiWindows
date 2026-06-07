# Agent Instructions

## Context First

Before planning or editing code, read the project context file:

- `_bmad-output/project-context.md`

That file contains the current project goal, repository research summary, proposed architecture direction, migration plan, acceptance targets, known risks, and implementation rules for future AI agents.

## Project Summary

This Unity project is an investigation and implementation workspace for building a UI architecture based on `UI.Windows-submodule`, with an MVP / Model-View-View-Presenter style layer inspired by `OpenUI`.

Core direction:

- Use `UI.Windows-submodule` for window lifecycle, loading, unloading, layouts, pooling, and resource management.
- Add a project-owned MVP adapter layer without mandatory Zenject or UniRx.
- Use a simple scene `CompositionRoot` for dependency wiring.
- Port most `OpenUI` examples and layouts onto the new approach after a small vertical slice is proven.

Do not import all OpenUI code blindly. Do not bypass `WindowSystem.Show/Hide` for UI.Windows lifecycle.

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

- Parent issue: `UIW-1` - `[Plan] UI.Windows MVP migration without mandatory Zenject/UniRx`
- URL: https://linear.app/white-rabbits-rabbit-hole/issue/UIW-1/plan-uiwindows-mvp-migration-without-mandatory-zenjectunirx
- Future agents should open `UIW-1`, inspect child issues ordered by numeric prefix, and pick the first child issue that is not `Done` or `Canceled` unless the user says otherwise.

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

Task creation should use the Linear `save_issue` tool with `team: "UiWindows"`. Do not create test issues just to verify write permission unless the user explicitly asks.

## Local MCP Notes

The local `.ai/mcp/mcp.json` file is currently empty, but Linear tools were available in the session environment when checked. If a future agent does not see Linear tools, report that MCP availability differs from the prior session instead of assuming Linear is unavailable globally.

## Repository Hygiene

- Check `git status --short --branch` before editing.
- Do not overwrite unrelated user changes.
- Keep third-party package internals unchanged unless a compatibility fix is unavoidable.
- Prefer a small vertical slice before broad migration work.

---
name: issue-task-flow
description: Orchestrate and execute issue-tracked software tasks using explicit Orchestrator and Executor modes, branch-per-task workflow, handoff prompts, implementation review, verification gates, and merge/closure control. Use when the user asks to act as an orchestrator, delegate work to another chat or agent, prepare an executor prompt, implement a specific issue, review an executor's result, choose the next issue from Linear/GitHub/Jira or a local task plan, or manage task branches and closure.
---

# Issue Task Flow

## Core Rule

Use one explicit mode per turn: `Orchestrator` or `Executor`.

If the user names a mode, use it. If the user asks to delegate, coordinate, prepare a prompt, review another chat, or choose the next task, use `Orchestrator`. If the user gives one concrete issue to implement or provides an executor prompt, use `Executor`.

If the mode is still ambiguous and the next action would differ materially, ask one short clarification before making changes.

## Always Load Local Context First

Before planning or editing, read the repository's local instructions when present:

- `AGENTS.md`
- project context files referenced by `AGENTS.md`
- issue/task plan files referenced by the project
- the selected issue and relevant comments from the available issue tracker
- blocker, predecessor, or setup issues and their relevant comments when they affect the selected issue

Treat local project instructions as authoritative over this generic skill.

When a project uses ordered issue titles or numeric prefixes, sort issues by that explicit project ordering. Do not rely on tracker API return order unless the project says API order is authoritative.

## Mode Routing

- For orchestration, read `references/orchestrator.md`.
- For execution, read `references/executor.md`.
- For post-implementation review, use Orchestrator mode and read `references/orchestrator.md`.

## Tracker And Branch Defaults

Prefer the project's configured issue tracker and branch naming conventions. If the project does not specify them:

- Issue tracker priority: Linear MCP, GitHub Issues/Projects, Jira, then user-provided issue text.
- Branch base: latest `main` unless the project names another integration branch.
- Branch name: tracker-generated branch name when available; otherwise `<issue-id>-<normalized-title>`.
- One task equals one branch.
- Executors do not merge or close tasks unless the user explicitly asks.
- Orchestrators merge/close only after acceptance and verification are satisfied, and only when the user asks to complete closure.

## Output Expectations

Keep outputs operational:

- Orchestrator outputs should include the chosen task, blockers, the executor prompt, and review/closure criteria.
- Executor outputs should include branch name, changed files, commits, verification results, issue updates, and unresolved risks.

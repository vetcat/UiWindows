---
name: issue-task-flow
description: Orchestrate and execute issue-tracked software tasks using explicit Orchestrator and Executor modes, branch-per-task workflow, handoff prompts, implementation review, verification gates, and merge/closure control. Use when the user asks to act as an orchestrator, delegate work to another chat or agent, prepare an executor prompt, implement a specific issue, review an executor's result, choose the next issue from Linear/GitHub/Jira or a local task plan, or manage task branches and closure.
---

# Issue Task Flow

## Core Rule

Use one explicit mode per turn: `Orchestrator` or `Executor`.

If the user names a mode, use it. If the user asks to delegate, coordinate, prepare a prompt, review another chat, or choose the next task, use `Orchestrator`. If the user gives one concrete issue to implement or provides an executor prompt, use `Executor`.

An Orchestrator may delegate implementation to an Executor sub-agent when sub-agent tools are available and the user has asked to try or use delegated execution. In that flow, the main chat remains the Orchestrator/control surface for the human; the sub-agent is the Executor implementation worker. Prefer storing the full task-specific Executor prompt as an `Executor Handoff` Linear comment, then launching the sub-agent with a short bootstrap prompt that points to the issue and handoff comment. If sub-agent tools or Linear handoff access are unavailable, blocked, or inappropriate for the task, fall back to a full separate-chat Executor handoff prompt.

The purpose of delegated execution is to preserve the Orchestrator's context window for long-lived coordination, spawning Executors, review, and human-facing decisions. It is not a token-saving rule for either role. Do not reduce the Executor's task context below what safe implementation requires; the Executor must receive the complete task contract through the Linear handoff or a full direct prompt.

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

## Parent And Epic Reconciliation

Closing child issues is not proof that a parent plan, epic, or umbrella issue is complete. When working from a parent issue, preserve the parent outcome separately from each child's local scope.

Before preparing or closing child work, identify which parent acceptance target the child advances and which parent targets remain open or intentionally deferred. When the parent uses broad wording such as "most", "representative", "finalize", "complete", or "migration", require an explicit coverage list or traceability matrix that maps requested outcomes to implemented artifacts, verification evidence, and deferred gaps.

When all known child issues are `Done`, `Canceled`, or otherwise inactive, do not mark or recommend the parent as complete by default. First run a parent reconciliation review against the original parent description, comments, local project context, and current repository state. If the implemented result is narrower than the original parent goal, report the mismatch and either create a follow-up/audit issue or ask for an explicit scope-change acceptance.

## Capability Discovery

Before preparing, implementing, reviewing, or closing issue work, verify which tracker, IDE, editor, validation, and multi-agent tools are actually available in the current session. Do not infer tool availability from local config files alone; hosted or IDE-provided tools may exist even when repository MCP config is empty.

When expected tooling is unavailable, times out, lacks dependencies, or cannot see the opened project, continue only with an explicit fallback and report the limitation in the prompt, review, or final result.

If an Executor is blocked by tooling that requires visible human action, such as a Unity Editor domain reload or modal confirmation that Unity MCP cannot accept, use the status `BLOCKED_HUMAN_ACTION_REQUIRED`. The Executor must report the required human action, branch/status, last successful step, and resume instruction. The Orchestrator must surface that blocker in the human-facing chat and resume the same Executor after confirmation when possible.

For project-owned skill validation, prefer a repository-provided validation wrapper when present, then an available system validator. If a cached validator script is not executable, invoke it through its interpreter instead of changing system cache permissions. If validator dependencies are missing, use and report a documented manual frontmatter check rather than silently skipping validation.

## Mode Routing

- For orchestration, read `references/orchestrator.md`.
- For execution, read `references/executor.md`.
- For post-implementation review, use Orchestrator mode and read `references/orchestrator.md`.

## Tracker And Branch Defaults

Prefer the project's configured issue tracker and branch naming conventions. If the project does not specify them:

- Issue tracker priority: Linear MCP, GitHub Issues/Projects, Jira, then user-provided issue text.
- Branch base: latest `main` unless the project names another integration branch.
- Executor branch namespace: `feature/`.
- Executor branch name: use the tracker-generated issue slug when available, but replace any leading personal/org namespace with `feature/`. For example, `owner/issue-slug` becomes `feature/issue-slug`.
- If no tracker-generated branch slug is available, use `feature/<issue-id>-<normalized-title>`.
- One task equals one branch.
- Executors commit task changes before final report unless blocked or explicitly told not to commit. Final Executor status should be clean; uncommitted/untracked changes require an explicit explanation.
- Executors do not merge or close tasks unless the user explicitly asks.
- Orchestrators merge/close only after acceptance and verification are satisfied, and only when the user asks to complete closure.

## Output Expectations

Keep outputs operational:

- Orchestrator outputs should include the chosen task, blocker status, relevant tool availability, delegation mode (sub-agent Executor or separate-chat fallback), the executor prompt or sub-agent result, and review/closure criteria.
- Executor outputs should include branch name, changed files, commits, tool availability/fallbacks, verification results, issue updates, and unresolved risks.

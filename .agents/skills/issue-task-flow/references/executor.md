# Executor Mode

Use Executor mode to implement exactly one selected issue.

An Executor may run as a separate chat or as a sub-agent spawned by an Orchestrator. The rules are the same in both cases. When running as a sub-agent, do not assume the workspace is isolated: you may be editing the same checkout the Orchestrator will inspect, so keep the branch clean and do not rely on parallel Orchestrator edits.

When launched with a short bootstrap prompt, first read the selected issue and the latest Linear comment titled `Executor Handoff`. Treat that comment as the task-specific contract. If Linear is unavailable, the issue cannot be opened, or the handoff comment is missing, stop and report the blocker instead of guessing from stale local context.

## Startup Checklist

1. Read local project instructions and context.
2. Open the selected issue and relevant comments, especially the latest `Executor Handoff` comment when present.
3. Check `git status --short --branch` before changes.
4. Verify tracker, IDE, editor, and validation tooling available in the current session.
5. Fetch the integration branch.
6. Create a dedicated task branch from the latest integration branch using the `feature/` namespace. If the tracker-generated branch is `owner/issue-slug`, replace the leading namespace and create `feature/issue-slug`; if no tracker slug exists, create `feature/<issue-id>-<normalized-title>`.
7. Check branch upstream hygiene. The feature branch should not misleadingly track the integration branch; unset upstream or push/set upstream to the remote feature branch when appropriate.
8. Restate the issue scope, non-goals, acceptance criteria, verification plan, available/unavailable tooling, and parent acceptance target advanced by this child before substantial edits.

If local changes already exist, do not overwrite them. Stop and ask how to proceed unless they are clearly your own current-turn changes.

## Implementation Rules

- Work only on the selected issue.
- Keep changes minimal and aligned with existing project patterns.
- Do not broaden scope to adjacent tasks.
- When implementation changes the repository's factual baseline, update the canonical local context or project docs referenced by local instructions before finalizing. Examples include new dependencies, verified environment state, architecture decisions, workflow changes, or completed setup that makes earlier context stale.
- Use IDE diagnostics/build/refactor/format tools for changed code when available, and report any skipped or unavailable IDE tooling.
- Use editor-specific tools for editor refresh/compile, tests, Console state, generated asset state, and live API checks when available.
- Do not close the issue unless explicitly asked.
- Do not merge into the integration branch unless explicitly asked.
- Update the issue tracker with meaningful notes after implementation and verification.
- If the issue is a child of a parent plan or epic, update the tracker with what parent acceptance target was advanced and what parent gaps remain outside this issue's scope. Do not imply the parent is complete unless a parent reconciliation review was explicitly performed.
- Commit the completed task changes before final report unless blocked or explicitly told not to commit. If an Orchestrator requests follow-up fixes, commit those fixes too or amend only when explicitly requested.
- Final repository status should be clean. If Unity/editor/generated noise remains, separate it from task changes and report exactly what is left.

## Verification Rules

Run the issue's requested checks. If a check cannot run, explain exactly why and what evidence remains.

After running IDE/editor tooling, rerun repository status and separate unrelated generated files from task changes before committing or reporting.

For project-owned skill changes, validate skill frontmatter with a repository-provided validation wrapper when present or with the available system validator. If a cached validator cannot execute directly, run it through its interpreter. If the validator or dependency is unavailable, use and report a documented manual frontmatter fallback.

Verification evidence should include:

- commands or tools run;
- pass/fail result;
- relevant errors or warnings;
- manual checks when automation is unavailable;
- any remaining risk.

## Final Report Template

```text
Branch: <branch-name>
Commits: <commit-shas-or-blocker-explanation>
Tool availability:
- <tracker/IDE/editor/validation tools available or fallback used>

Changed files:
- <path>: <summary>

Verification:
- <check>: <result>

Issue tracker:
- <comments/status updates made>

Parent traceability:
- Advanced: <parent acceptance target(s) or n/a>
- Still open/deferred: <known parent gaps or n/a>

Unresolved risks:
- <risk or none>

Not done:
- <explicitly out-of-scope or blocked items>

Repository status:
- <clean/ahead/behind/upstream notes/generated noise>
```

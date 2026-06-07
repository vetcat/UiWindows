# Executor Mode

Use Executor mode to implement exactly one selected issue.

## Startup Checklist

1. Read local project instructions and context.
2. Open the selected issue and relevant comments.
3. Check `git status --short --branch` before changes.
4. Fetch the integration branch.
5. Create a dedicated task branch from the latest integration branch.
6. Restate the issue scope, non-goals, acceptance criteria, and verification plan before substantial edits.

If local changes already exist, do not overwrite them. Stop and ask how to proceed unless they are clearly your own current-turn changes.

## Implementation Rules

- Work only on the selected issue.
- Keep changes minimal and aligned with existing project patterns.
- Do not broaden scope to adjacent tasks.
- Do not close the issue unless explicitly asked.
- Do not merge into the integration branch unless explicitly asked.
- Update the issue tracker with meaningful notes after implementation and verification.

## Verification Rules

Run the issue's requested checks. If a check cannot run, explain exactly why and what evidence remains.

Verification evidence should include:

- commands or tools run;
- pass/fail result;
- relevant errors or warnings;
- manual checks when automation is unavailable;
- any remaining risk.

## Final Report Template

```text
Branch: <branch-name>
Commits: <commit-shas-or-none>
Changed files:
- <path>: <summary>

Verification:
- <check>: <result>

Issue tracker:
- <comments/status updates made>

Unresolved risks:
- <risk or none>

Not done:
- <explicitly out-of-scope or blocked items>
```

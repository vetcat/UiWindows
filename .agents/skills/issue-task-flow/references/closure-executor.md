# Closure Executor Mode

Use Closure Executor mode only after the Orchestrator has accepted an implementation result and explicitly authorized closure.

The Closure Executor performs mechanical repository/tracker closure. It does not decide whether the work is good enough, does not review acceptance criteria, does not edit source files, and does not resolve conflicts.

## Startup Checklist

1. Read local project instructions and context.
2. Open the selected issue and latest Linear comment titled `Closure Handoff`.
3. Confirm the handoff includes accepted branch, accepted commit, integration branch, authorized closure actions, final tracker note/status, and stop conditions.
4. Check `git status --short --branch` before changes.
5. Verify tracker and git tooling are available.

If the `Closure Handoff` is missing, ambiguous, or not explicit about authorization, stop and report the blocker.

## Allowed Actions

- Fetch/update the integration branch named in the handoff.
- Merge the accepted task branch or accepted commit using the strategy specified by the handoff or project default.
- Push the integration branch.
- Update the selected issue status/final notes exactly as authorized.
- Update parent-plan notes or next-task markers only when explicitly authorized.
- Report final repository status and remote sync state.

## Forbidden Actions

- Do not edit source, asset, package, documentation, or generated files manually.
- Do not resolve merge conflicts.
- Do not run broad refactors, cleanup, formatting, tests, or verification beyond explicit closure checks.
- Do not reinterpret acceptance criteria or decide that the implementation is good enough.
- Do not close parent/umbrella issues unless the handoff explicitly says the Orchestrator completed parent reconciliation and authorized parent closure.
- Do not continue after unexpected dirty status, divergent integration branch, failed merge, failed push, missing tracker access, or tracker update failure.

## Stop Conditions

Stop and return a blocker report when any of these occur:

- Missing or stale `Closure Handoff`.
- Working tree has unexpected changes before closure.
- Accepted branch/commit does not exist or does not match the handoff.
- Integration branch is divergent or cannot be updated cleanly.
- Merge conflict or non-fast-forward/strategy failure.
- Push failure.
- Tracker update failure.
- Any file content change would be required to complete closure.

## Closure Handoff Contract

The handoff must include:

```text
## Closure Handoff

Project: <absolute-project-path>
Role: Closure Executor
Issue: <issue-id> - <title>
Issue URL: <url-if-available>

Accepted implementation:
- Branch: <feature-branch>
- Commit(s): <accepted-commit-shas>
- Orchestrator acceptance summary: <short summary>

Authorized closure actions:
- Merge <feature-branch> into <integration-branch>.
- Push <integration-branch>.
- Add final Linear note: <exact or summarized note>.
- Move Linear issue to <status>, if authorized.
- Update parent/plan note: <exact action or none>.

Stop conditions:
- Unexpected dirty working tree.
- Accepted branch/commit mismatch.
- Merge conflict.
- Failed push.
- Missing tracker access or failed tracker update.
- Any need to edit files manually.

Final report must include:
- Closure branch/integration branch status.
- Merge commit or fast-forward result.
- Pushed remote/head commit.
- Tracker updates made.
- Final `git status --short --branch`.
- Blockers or risks.
```

## Final Report Template

```text
Role: Closure Executor
Issue: <issue-id>

Accepted branch/commit:
- <branch and commit(s)>

Closure actions:
- <merge/push/tracker updates performed>

Tracker:
- <status/final notes updated, or blocker>

Repository status:
- <git status --short --branch result>
- <integration branch/head and remote sync>

Blockers:
- <none or exact blocker>
```

# Orchestrator Mode

Use Orchestrator mode to coordinate issue work without doing the implementation yourself.

## Startup Checklist

1. Read local project instructions and context.
2. Check repository status and current branch.
3. Verify tracker, IDE, editor, validation, and multi-agent tooling available in the current session.
4. Open the parent task plan or tracker query named by the project.
5. Identify the next issue by project ordering rules.
6. If the project uses ordered titles or numeric prefixes, sort by that explicit order instead of tracker API return order.
7. Read current issue status from the tracker when available. Treat local docs as ordering/context snapshots, not as the authoritative progress ledger.
8. Verify blockers, status, labels, comments, and acceptance criteria.
9. Read relevant comments on completed blockers, predecessor tasks, or setup tasks when they define workflow or constraints for the selected issue.
10. Map the selected child issue to the parent acceptance target it advances, and note parent targets that remain open or intentionally deferred.
11. Confirm the branch naming convention and integration branch. For Executor work, default to `feature/<issue-slug>` unless local project instructions say otherwise.

If the next task is unclear, report the ambiguity and ask for a choice instead of guessing.

## Parent Reconciliation Gate

Use this gate for broad parent plans, epics, umbrella issues, migrations, or any issue whose goal spans multiple child issues.

Before preparing the last child, closing a parent, or saying "all tasks are done":

1. Re-read the parent issue description, parent comments, local project context, and completed child issue summaries.
2. Build a concise traceability matrix: parent acceptance target -> implemented artifact(s) -> verification evidence -> status (`Done`, `Partial`, `Deferred`, `Missing`).
3. Treat broad wording such as "most", "representative", "finalize", "complete", "migration", and "ready" as ambiguous until it is backed by an explicit list.
4. Compare the repository's current artifacts with the original parent scope, not only with the final child issue's scope.
5. If child issue scopes narrowed the parent outcome, call out the mismatch and require either follow-up issues or explicit scope-change acceptance.
6. Do not mark or recommend the parent complete solely because all children are `Done` or `Canceled`.

For a parent/audit issue, the expected Orchestrator output is the traceability matrix, gaps, recommended follow-up issues, and a clear recommendation: close as accepted, close with explicit reduced scope, or keep open.

## Executor Delegation Modes

Default mode when available: keep the current chat as Orchestrator and spawn specialized sub-agents for bounded work. Use an implementation Executor for one issue's code/config/docs work, and a Closure Executor for mechanical post-acceptance merge/push/tracker closure. The goal is to preserve the Orchestrator's context window for long-lived coordination, spawning agents, review, follow-up retries, and human-facing decisions. This is not a token-saving rule for any role.

When project instructions establish this delegated flow, user requests such as "take the next task", "implement this issue", "review the Executor result", "close the task", or "complete closure" are explicit authorization to run the corresponding sub-agent workflow. Do not require the user to include the literal words "sub-agent" or "delegate" every time. The Orchestrator owns planning, handoff quality, review, acceptance decisions, retry/follow-up control, and final accountability; it should not perform low-level implementation, repository mechanics, or tracker closure itself when a suitable agent path exists.

Use a short bootstrap prompt instead of forking the full conversation context only when the Executor can read the complete task contract from Linear or another explicit handoff. Do not make the Executor infer missing scope, acceptance criteria, verification, or project constraints from a deliberately underspecified prompt.

Fallback mode: create a focused prompt for a separate Executor chat when sub-agent tools are unavailable, the user explicitly wants a separate chat, or the implementation requires isolation that the current sub-agent runtime cannot provide. Direct Orchestrator implementation is a last-resort fallback only when no sub-agent or separate-chat Executor path is available, or when the user explicitly asks the Orchestrator to do the implementation personally without agents.

Default handoff storage: write the full task-specific Executor prompt into the selected issue as a Linear comment titled `Executor Handoff` before spawning the sub-agent. Then give the sub-agent a short bootstrap prompt with project path, issue ID/link, and instructions to read the latest `Executor Handoff` comment. This keeps the durable task contract in Linear for humans, replacement sub-agents, and later review; the short bootstrap is only a pointer to that contract.

If Linear is unavailable or the `Executor Handoff` comment cannot be created/read, do not silently proceed with a link-only handoff. Either include the full Executor prompt directly in the sub-agent/separate-chat prompt, or stop and report that the durable handoff could not be prepared.

Important lessons from the `UIW-20` trial:

- Sub-agents may work in the same repository checkout, not an isolated copy. While the Executor is running, the Orchestrator should treat the task branch as write-locked and avoid parallel file edits in that workspace.
- A long-running Executor may time out on the first wait while still making progress. Do not assume failure from one wait timeout; first use read-only status checks such as `git status --short --branch`, then wait again or request a status update before intervening.
- The Executor's first result can be mostly correct but still require review cleanup. Use `send_input` to request a narrow follow-up from the same sub-agent when the fix depends on its implementation context.
- `UIW-20` and `UIW-21` both surfaced missing-final-newline hygiene after the first Executor result. Include final-newline checks in future handoffs and usually ask the same Executor to fix this kind of small review issue before acceptance.
- Require a task commit before accepting the Executor result. An uncommitted working tree is reviewable, but not closure-ready.
- If the created feature branch misleadingly tracks `origin/main`, fix or note branch hygiene. A local task branch should either have no upstream or track its own remote feature branch after push.
- Close the sub-agent after acceptance or after deciding to abandon its result.

## Human Action Required Escalation

Use `BLOCKED_HUMAN_ACTION_REQUIRED` when the Executor cannot continue because a visible tool/editor action must be performed by the human. The common Unity case is Unity MCP waiting on an Editor domain reload, script reload, modal confirmation, or PlayMode/test-runner state that cannot be accepted through MCP.

Require the Executor to report:

- `Status: BLOCKED_HUMAN_ACTION_REQUIRED`
- Cause, including the exact tool/editor state or repeated timeout.
- Required human action, for example "confirm Reload/Domain Reload in Unity Editor".
- Current branch, commit status, and relevant uncommitted files.
- Last successful step and the next step to resume.

The Orchestrator must immediately surface the required action in the human-facing chat. After the human confirms completion, resume the same Executor with a narrow message such as:

```text
Human action completed: <action>.
Continue from BLOCKED_HUMAN_ACTION_REQUIRED.
Re-check Unity/editor state, rerun the pending verification step, and report the result.
```

If the original Executor cannot be resumed, launch a replacement Executor with the issue link, latest `Executor Handoff`, and the blocker/resume context. Do not treat a human-action blocker as task failure unless the required action cannot be completed.

## Closure Delegation

After the Orchestrator has reviewed and accepted an implementation result, delegate mechanical closure to a Closure Executor sub-agent when sub-agent tools are available. This keeps the human-facing chat focused on decisions and review while agents perform routine repository/tracker operations.

A user request such as "close this task", "finish closure", or "закрывай работу над таском" is explicit authorization to launch the Closure Executor in projects where this delegated flow is the default. Do not treat the absence of the literal word "sub-agent" as a reason to do repository/tracker closure directly.

The Orchestrator must make the acceptance decision before closure delegation. The Closure Executor does not review the implementation, reinterpret acceptance criteria, edit files, resolve merge conflicts, or perform local documentation reconciliation.

Post-closure documentation drift is an Orchestrator follow-up after the Closure Executor succeeds. Keep the closure handoff mechanical and do not include local file edits for documentation drift.

Write a Linear comment titled `Closure Handoff` before launching the Closure Executor:

```text
## Closure Handoff

Project: <absolute-project-path>
Role: Closure Executor
Issue: <issue-id> - <title>
Issue URL: <url-if-available>

Accepted implementation:
- Branch: <feature-branch>
- Commit(s): <accepted-commit-shas>
- Orchestrator acceptance summary: <summary>

Authorized closure actions:
- Merge <feature-branch> into <integration-branch>.
- Push <integration-branch>.
- Add final Linear note: <note>.
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
- Merge result and integration branch head.
- Push result and remote sync state.
- Linear updates made.
- Final repository status.
- Blockers or risks.
```

Then spawn the Closure Executor with a short bootstrap prompt:

```text
Project: <absolute-project-path>
Role: Closure Executor
Issue: <issue-id> - <title>
Issue URL: <url-if-available>

Read the Linear issue and the latest comment titled "Closure Handoff".
If Linear is unavailable, the issue cannot be opened, or that handoff comment is missing, stop and report the blocker.

Hard rules:
- Perform only the authorized closure actions.
- Do not edit files manually.
- Do not perform local documentation drift review.
- Do not resolve merge conflicts.
- Stop on unexpected dirty status, accepted branch/commit mismatch, failed merge, failed push, or tracker update failure.
- Final report must include merge result, push result, Linear updates, final repository status, and blockers.
```

If sub-agent tooling or Linear handoff access is unavailable, first use a separate-chat Closure Executor fallback when practical and paste the full `Closure Handoff` content directly if Linear access is uncertain. The Orchestrator may perform closure directly only when no Closure Executor path is available or the user explicitly asks for direct Orchestrator closure, and must report that fallback reason before doing the mechanical work.

## Delegation Prompt Template

Create a focused full handoff prompt and save it as the latest Linear issue comment titled `Executor Handoff` when Linear is available:

```text
## Executor Handoff

Project: <absolute-project-path>
Role: Executor
Issue: <issue-id> - <title>
Issue URL: <url-if-available>

Read first:
- AGENTS.md
- <project-context-files>
- <parent-plan-if-relevant>
- <selected-issue-and-comments>
- <completed-blocker-or-predecessor-issues-and-comments-when-relevant>

Branch workflow:
- Start from latest <integration-branch>.
- Create branch: feature/<tracker-generated-issue-slug-without-leading-owner-namespace>.
- Do not work directly on <integration-branch>.
- Do not leave the feature branch tracking <integration-branch> as its upstream; unset upstream or push/set upstream to the feature branch if required by the task workflow.
- Commit task changes before final report unless explicitly blocked.
- Final repository status must be clean, or any uncommitted/generated files must be explicitly reported.

Scope:
- <in-scope bullets>

Non-goals:
- <out-of-scope bullets>

Acceptance criteria:
- <criteria bullets>

Parent traceability:
- This issue advances: <parent acceptance target(s)>
- Known parent gaps/deferred items: <items or none>
- If this is a final child or audit, produce/update the parent traceability matrix before recommending parent closure.

Verification required:
- <commands/tools/checks>
- Focused issue-specific tests plus directly relevant existing suites; run broad unrelated suites only when the touched surface justifies them, they are cheap, or the issue explicitly asks for them.
- `git diff --check <integration-branch>...HEAD`.
- Final-newline check for changed text files such as `.cs`, `.md`, `.asmdef`, `.json`, `.yaml`, and `.yml`.

Tooling expectations:
- Verify available tracker, IDE, editor, and validation tools before editing.
- Use IDE diagnostics/build/refactor/format tools for changed code when available.
- Use editor-specific tools for editor refresh/compile, tests, console state, and live API checks when available.
- Report unavailable, timed-out, skipped, or fallback tooling explicitly.
- If Unity/editor tooling is blocked by a human-visible domain reload, modal confirmation, or similar prompt, report `BLOCKED_HUMAN_ACTION_REQUIRED` with the required action and resume point instead of waiting indefinitely.
- Rerun repository status after editor/tool checks and separate generated noise from task changes.

Issue tracker updates:
- Add implementation notes and verification results.
- Do not close the issue unless explicitly asked.

Local documentation:
- Do not update `AGENTS.md` or other rules docs merely to record task progress or mirror tracker status. The tracker is the progress source of truth.
- If implementation changes durable project facts, update canonical context/docs with stable facts only and avoid transient phrases such as `review pending`, `implemented on branch`, or `requires closure`.

Final response must include:
- Branch name
- Commits
- Changed files
- Verification results
- Issue tracker updates
- Final repository status and branch upstream notes
- Remaining risks or blockers
```

Then spawn the sub-agent with a short bootstrap prompt:

```text
Project: <absolute-project-path>
Role: Executor
Issue: <issue-id> - <title>
Issue URL: <url-if-available>

Read the Linear issue and the latest comment titled "Executor Handoff".
If Linear is unavailable, the issue cannot be opened, or that handoff comment is missing, stop and report the blocker.

Hard rules:
- Work only on this issue.
- Follow AGENTS.md and _bmad-output/project-context.md.
- Start from latest main and create the required feature branch.
- Do not merge or close the issue.
- If Unity/editor tooling requires human confirmation, report `BLOCKED_HUMAN_ACTION_REQUIRED` with the exact action needed, current branch/status, last successful step, and resume instruction.
- Commit completed changes before final report unless blocked.
- Final report must include branch, commits, changed files, verification, Linear updates, final repository status, and risks.
```

For separate-chat fallback, either give the same short bootstrap prompt when the human will open Linear in that chat, or paste the full `Executor Handoff` content directly if Linear access is uncertain.

## Review Checklist

When the Executor reports back:

- Fetch or inspect the task branch.
- Check `git status --short --branch`.
- Review the committed diff against the integration branch. If changes are still uncommitted, review enough to triage but request a commit before acceptance.
- Confirm the branch only touches the selected issue scope.
- Check acceptance criteria one by one.
- Check the child-to-parent traceability note: what parent target was advanced, what remains open, and whether any parent goal was narrowed by the implementation.
- Confirm the Executor discovered available tools and reported any fallback.
- Inspect verification evidence, not just claims.
- Check issue tracker comments/status.
- Run an independent Orchestrator spot-check proportional to risk, such as `git diff --check`, a final-newline check for changed text files, targeted diagnostics, focused tests, or forbidden dependency scans. The Orchestrator does not need to rerun every expensive Executor check when the evidence is credible, but must verify enough to make acceptance defensible.
- Rerun repository status after review checks and identify unrelated generated files before closure.
- Identify residual risks and missing tests.
- Prefer requesting a narrow Executor follow-up for issues inside the Executor's implementation, especially when the same sub-agent can fix them with its local context. Make a direct Orchestrator adjustment only when it is clearly a small review/closure edit and report it.

## Closure Workflow

Only after acceptance criteria and verification are satisfied:

1. Record the accepted branch, accepted commits, verification evidence, and Orchestrator acceptance summary.
2. If this was the final active child under a parent, run the Parent Reconciliation Gate before authorizing any parent closure.
3. Write a `Closure Handoff` Linear comment with exact authorized actions and stop conditions.
4. Launch a Closure Executor sub-agent when available. If sub-agent tooling is unavailable, use a separate-chat Closure Executor fallback when practical. Direct Orchestrator closure is allowed only when no Closure Executor path is available or the user explicitly asks for direct Orchestrator closure; report that fallback reason before doing mechanical work.
5. Review the Closure Executor report: merge result, pushed integration branch, Linear updates, parent/plan note updates, final repository status, and blockers.
6. If closure succeeded, run a post-closure documentation drift check before the final human report. Scan local instructions, project context, and relevant docs for the closed issue id/title and stale transient state such as `Todo`, `In Progress`, `review pending`, `implemented on branch`, `requires closure`, or obsolete next-task markers.
7. If drift is found, make a small context-only commit on the integration branch, push it, and include the commit in the closure result. Do not treat local progress text as authoritative over tracker status.
8. Confirm the next task should start from the updated integration branch.
9. If closure was blocked, surface the blocker and decide whether to fix, retry with the same Closure Executor, launch a replacement Closure Executor, or use a separate-chat Closure Executor fallback. Direct Orchestrator fallback remains the last resort.

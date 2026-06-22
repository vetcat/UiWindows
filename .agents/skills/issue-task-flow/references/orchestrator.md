# Orchestrator Mode

Use Orchestrator mode to coordinate issue work without doing the implementation yourself.

## Startup Checklist

1. Read local project instructions and context.
2. Check repository status and current branch.
3. Verify tracker, IDE, editor, validation, and multi-agent tooling available in the current session.
4. Open the parent task plan or tracker query named by the project.
5. Identify the next issue by project ordering rules.
6. If the project uses ordered titles or numeric prefixes, sort by that explicit order instead of tracker API return order.
7. Verify blockers, status, labels, comments, and acceptance criteria.
8. Read relevant comments on completed blockers, predecessor tasks, or setup tasks when they define workflow or constraints for the selected issue.
9. Map the selected child issue to the parent acceptance target it advances, and note parent targets that remain open or intentionally deferred.
10. Confirm the branch naming convention and integration branch. For Executor work, default to `feature/<issue-slug>` unless local project instructions say otherwise.

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

Preferred mode when available: keep the current chat as Orchestrator and spawn one Executor sub-agent for exactly one issue. The goal is to preserve the Orchestrator's context window for long-lived coordination, spawning Executors, review, and human-facing decisions. This is not a token-saving rule for either role.

Use a short bootstrap prompt instead of forking the full conversation context only when the Executor can read the complete task contract from Linear or another explicit handoff. Do not make the Executor infer missing scope, acceptance criteria, verification, or project constraints from a deliberately underspecified prompt.

Fallback mode: create a focused prompt for a separate Executor chat when sub-agent tools are unavailable, the user explicitly wants a separate chat, or the implementation requires isolation that the current sub-agent runtime cannot provide.

Default handoff storage: write the full task-specific Executor prompt into the selected issue as a Linear comment titled `Executor Handoff` before spawning the sub-agent. Then give the sub-agent a short bootstrap prompt with project path, issue ID/link, and instructions to read the latest `Executor Handoff` comment. This keeps the durable task contract in Linear for humans, replacement sub-agents, and later review; the short bootstrap is only a pointer to that contract.

If Linear is unavailable or the `Executor Handoff` comment cannot be created/read, do not silently proceed with a link-only handoff. Either include the full Executor prompt directly in the sub-agent/separate-chat prompt, or stop and report that the durable handoff could not be prepared.

Important lessons from the `UIW-20` trial:

- Sub-agents may work in the same repository checkout, not an isolated copy. While the Executor is running, the Orchestrator should treat the task branch as write-locked and avoid parallel file edits in that workspace.
- A long-running Executor may time out on the first wait while still making progress. Do not assume failure from one wait timeout; check repository status or wait again before intervening.
- The Executor's first result can be mostly correct but still require review cleanup. Use `send_input` to request a narrow follow-up from the same sub-agent when the fix depends on its implementation context.
- Require a task commit before accepting the Executor result. An uncommitted working tree is reviewable, but not closure-ready.
- If the created feature branch misleadingly tracks `origin/main`, fix or note branch hygiene. A local task branch should either have no upstream or track its own remote feature branch after push.
- Close the sub-agent after acceptance or after deciding to abandon its result.

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

Tooling expectations:
- Verify available tracker, IDE, editor, and validation tools before editing.
- Use IDE diagnostics/build/refactor/format tools for changed code when available.
- Use editor-specific tools for editor refresh/compile, tests, console state, and live API checks when available.
- Report unavailable, timed-out, skipped, or fallback tooling explicitly.
- Rerun repository status after editor/tool checks and separate generated noise from task changes.

Issue tracker updates:
- Add implementation notes and verification results.
- Do not close the issue unless explicitly asked.

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
- Run an independent Orchestrator spot-check proportional to risk, such as `git diff --check`, targeted diagnostics, focused tests, or forbidden dependency scans. The Orchestrator does not need to rerun every expensive Executor check when the evidence is credible, but must verify enough to make acceptance defensible.
- Rerun repository status after review checks and identify unrelated generated files before closure.
- Identify residual risks and missing tests.
- Prefer requesting a narrow Executor follow-up for issues inside the Executor's implementation, especially when the same sub-agent can fix them with its local context. Make a direct Orchestrator adjustment only when it is clearly a small review/closure edit and report it.

## Closure Workflow

Only after acceptance criteria and verification are satisfied:

1. Ensure the task branch is up to date with the integration branch.
2. Check repository status and confirm unrelated local/generated changes are excluded or intentionally accepted.
3. Merge using the project's preferred strategy.
4. Push the integration branch.
5. Verify the local integration branch is not still ahead of its remote after the push.
6. Update issue status and add final notes if the user asked to complete closure.
7. Update parent plan notes or next-task markers when the project uses them, including parent acceptance target status and any explicit deferred gaps.
8. If this was the final active child under a parent, run the Parent Reconciliation Gate before recommending parent closure.
9. Confirm the next task should start from the updated integration branch.

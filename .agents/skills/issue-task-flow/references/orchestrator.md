# Orchestrator Mode

Use Orchestrator mode to coordinate issue work without doing the implementation yourself.

## Startup Checklist

1. Read local project instructions and context.
2. Check repository status and current branch.
3. Open the parent task plan or tracker query named by the project.
4. Identify the next issue by project ordering rules.
5. If the project uses ordered titles or numeric prefixes, sort by that explicit order instead of tracker API return order.
6. Verify blockers, status, labels, comments, and acceptance criteria.
7. Read relevant comments on completed blockers, predecessor tasks, or setup tasks when they define workflow or constraints for the selected issue.
8. Confirm the branch naming convention and integration branch.

If the next task is unclear, report the ambiguity and ask for a choice instead of guessing.

## Delegation Prompt Template

Create a focused prompt for one Executor chat:

```text
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
- Create branch: <tracker-generated-branch-or-issue-slug>.
- Do not work directly on <integration-branch>.

Scope:
- <in-scope bullets>

Non-goals:
- <out-of-scope bullets>

Acceptance criteria:
- <criteria bullets>

Verification required:
- <commands/tools/checks>

Issue tracker updates:
- Add implementation notes and verification results.
- Do not close the issue unless explicitly asked.

Final response must include:
- Branch name
- Commits
- Changed files
- Verification results
- Issue tracker updates
- Remaining risks or blockers
```

## Review Checklist

When the Executor reports back:

- Fetch or inspect the task branch.
- Check `git status --short --branch`.
- Review the diff against the integration branch.
- Confirm the branch only touches the selected issue scope.
- Check acceptance criteria one by one.
- Inspect verification evidence, not just claims.
- Check issue tracker comments/status.
- Identify residual risks and missing tests.
- Do not silently fix executor work unless the user asks; either request executor follow-up or make a clearly scoped review adjustment.

## Closure Workflow

Only after acceptance criteria and verification are satisfied:

1. Ensure the task branch is up to date with the integration branch.
2. Merge using the project's preferred strategy.
3. Push the integration branch.
4. Update issue status and final notes if the user asked to complete closure.
5. Confirm the next task should start from the updated integration branch.

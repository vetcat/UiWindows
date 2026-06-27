# AI Agent Reference Prompt

Use this document when asking an AI coding agent in another Unity project to use this repository as a UI architecture reference.

## Full Prompt

```text
You are working in a Unity project at: <TARGET_PROJECT_PATH>.

Use https://github.com/vetcat/UiWindows as the reference repository for UI architecture.
Treat it as an architecture and implementation reference, not as code to copy wholesale.

Before designing or editing code, read these reference files from the UiWindows repository:

1. README.md
2. docs/index.md
3. docs/project-architecture-skeleton.md
4. docs/uiwindows-mvp-openui-migration-guide.md
5. Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md
6. docs/r3-mvp-conventions.md
7. docs/uiwindows-mvp-pooling-lifecycle.md
8. docs/reference-adoption-checklist.md
9. docs/reference-architecture-diagram.md

Use these rules from the reference:

- UI.Windows owns window creation, loading, show/hide lifecycle, layouts, pooling, and cleanup.
- Open windows through WindowSystem.Show, WindowSystem.ShowSync, or a narrow project wrapper that delegates to UI.Windows.
- Do not use GameObject.SetActive as the runtime UI window lifecycle.
- Bind presenters only after UI.Windows has created or loaded a window instance.
- Keep one presenter binding per pooled window instance.
- Create a fresh show scope on each show and dispose it on hide/pool return.
- Put R3 subscriptions, Unity UI listeners, timers, frame streams, and transient request subscriptions in show scope when they should only run while visible.
- Keep mutable reactive primitives private to their owning service.
- Expose model/read-model state through read-only ports and mutate through explicit command methods.
- Keep domain/model services independent from UI.Windows, Unity UI, presenters, DOTween, OpenUI, Zenject, and UniRx.
- Use DOTween only in UI/effects rendering code if the target project allows it.

Your task:

<DESCRIBE_TARGET_PROJECT_UI_TASK_HERE>

Implementation constraints:

- Follow the target project's existing folder, assembly, naming, and scene conventions.
- Adapt the reference patterns to the target project instead of importing UiWindows sample scenes or demo data.
- Start with a small vertical slice unless the target project already has the foundation in place.
- Add focused tests for presenter behavior and real UI.Windows Show -> Hide -> Reopen lifecycle where practical.
- Report any target-project differences from the reference before making broad architectural changes.
```

## Short Prompt

Use this when the agent already has the reference repository available locally:

```text
Use the UiWindows repository as the reference for UI.Windows MVP architecture.
Read README.md, docs/index.md, docs/reference-adoption-checklist.md,
docs/reference-architecture-diagram.md, docs/uiwindows-mvp-openui-migration-guide.md,
docs/r3-mvp-conventions.md, docs/uiwindows-mvp-pooling-lifecycle.md, and
Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md.

Apply the lifecycle, boundary, and testing patterns to the target project.
Do not copy demo scenes/data blindly, do not import OpenUI infrastructure, do not add Zenject or UniRx, and do not bypass UI.Windows lifecycle with GameObject.SetActive.
```

## Review Prompt

Use this to review an implementation that claims to follow the reference:

```text
Review this Unity UI implementation against the UiWindows reference architecture.

Check:

- Does UI.Windows still own window show/hide/loading/pooling/cleanup?
- Are presenters bound after UI.Windows creates the window?
- Does each pooled show create a fresh show scope?
- Are show-scoped R3 subscriptions and Unity UI listeners disposed on hide?
- Are model/read-model ports read-only from the presenter side?
- Do mutations cross boundaries through explicit command methods?
- Are ProjectContext/domain services free from UI.Windows, Unity UI, presenter, DOTween, OpenUI, Zenject, and UniRx dependencies?
- Are DOTween usages limited to UI/effects rendering and cleaned on hide/pool return?
- Is there a real Show -> Hide -> Reopen PlayMode test for pooled windows?
- Did the implementation avoid copying sample-only demo data and scene wiring into production code?

Lead with concrete findings and file/line references.
```

## Agent Output Expectations

Ask the agent to report:

- Which reference docs it read.
- Which target-project files were changed.
- Which reference patterns were reused.
- Which reference patterns were intentionally changed and why.
- Which tests or validations were run.
- Remaining gaps, especially around mobile readiness, loading/prewarm policy, pooling, and runtime-built UI.

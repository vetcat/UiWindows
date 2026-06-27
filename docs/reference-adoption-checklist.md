# Reference Adoption Checklist

Use this checklist when another Unity project wants to adopt the UI architecture patterns from this repository.

The goal is to transfer the proven boundaries, lifecycle rules, and verification strategy. Do not copy the sample project wholesale unless the target project explicitly wants the demo scenes, demo models, and example UI.

## Before You Start

- Confirm the target project uses Unity UI and wants UI.Windows-owned window lifecycle, loading, layouts, pooling, and cleanup.
- Confirm whether the target project will use the same UI.Windows fork or an already installed compatible UI.Windows package.
- Confirm whether the target project will use R3 for model/read-model state and request streams.
- Confirm whether DOTween is allowed for UI/effects rendering. DOTween is optional for architecture, but this repository uses it for representative UI FX.
- Identify one small vertical slice to port first, for example a HUD panel with one model binding and one command.
- Identify the target project's existing scene bootstrap mechanism. If it has no clear mechanism, start with a simple CompositionRoot equivalent.

## Source Material To Read

Read these in order:

1. [../README.md](../README.md)
2. [index.md](index.md)
3. [project-architecture-skeleton.md](project-architecture-skeleton.md)
4. [uiwindows-mvp-openui-migration-guide.md](uiwindows-mvp-openui-migration-guide.md)
5. [../Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md](../Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md)
6. [r3-mvp-conventions.md](r3-mvp-conventions.md)
7. [uiwindows-mvp-pooling-lifecycle.md](uiwindows-mvp-pooling-lifecycle.md)
8. [reference-architecture-diagram.md](reference-architecture-diagram.md)

## Adoption Steps

1. Establish code boundaries.
   - Create or identify equivalents for `CompositionRoot`, `ProjectContext`, and `UiWindowsMvp`.
   - Keep reusable scene composition independent from UI.Windows, Unity UI, presenter implementations, OpenUI, Zenject, UniRx, and R3 unless the target project intentionally chooses a different boundary.

2. Establish model/read-model ports.
   - Expose read-only state through `ReadOnlyReactiveProperty<T>` or equivalent read-only surfaces.
   - Expose events and requests through `Observable<T>` or equivalent streams.
   - Keep mutable state private to the owning service.
   - Route mutations through explicit command methods.

3. Add the presenter lifecycle adapter.
   - Use `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` as the reference implementation.
   - Bind presenters after UI.Windows creates the window instance.
   - Keep one presenter binding per pooled window instance.
   - Create a fresh show scope on show.
   - Dispose the show scope on hide or pool return.

4. Build one vertical slice.
   - Start with one window, one view prefab, one presenter, one presenter factory, one runtime window source or launcher, and one model/read-model port.
   - Open the window through `WindowSystem.Show` or a narrow wrapper that delegates to it.
   - Do not use direct `GameObject.SetActive` as the runtime show/hide path.

5. Add focused tests before widening the migration.
   - Test presenter behavior without depending on a full scene when practical.
   - Test real UI.Windows `Show -> Hide -> Reopen` cycles for pooled windows.
   - Verify subscriptions and button listeners do not duplicate after hide/show.
   - Verify hidden windows do not consume transient request streams.

6. Expand to navigation and overlays.
   - Add modal requests, hint requests, and FX requests through project-owned ports.
   - Keep request payloads stable and UI-free.
   - Let the UI layer decide how to render requests.

7. Add integration coverage.
   - Create one canonical runtime scene equivalent to `Assets/Scenes/SampleScene.unity`.
   - Add one integrated workflow test that drives representative HUD, navigation, modal, request stream, pooling, and cleanup behavior.

8. Harden for production.
   - Define mobile performance targets.
   - Define loading, first-show, and prewarm policy.
   - Review layout rebuilds, raycast targets, text settings, and runtime-created fallback UI.
   - Consolidate repeated launcher/runtime window source boilerplate only after the first slices are proven.

## Copy Deliberately

Good candidates to adapt:

- `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter`
- `Assets/Scripts/CompositionRoot/Runtime`
- `Assets/Scripts/ProjectContext/Runtime/*` port shapes
- `Assets/Scripts/UiWindowsMvp/Tests/PlayMode/*LifecycleTests.cs`
- `Assets/Scripts/UiWindowsMvp/Tests/PlayMode/SampleSceneIntegratedAcceptanceTests.cs`

Do not copy blindly:

- Demo model data and SampleScene-specific launchers.
- OpenUI runtime infrastructure.
- Zenject installers or UniRx subscription style from OpenUI.
- Runtime fallback UI builders as final production UI.
- Project-specific Linear/task-flow files unless the target project also uses the same flow.

## Acceptance Checklist

Before calling an adoption slice complete:

- Windows open and close through UI.Windows lifecycle.
- Presenter dependencies are explicit and narrow.
- Model/read-model services do not reference UI.Windows, Unity UI, presenters, DOTween, OpenUI, Zenject, or UniRx.
- Show-scoped subscriptions are disposed on hide.
- Pooled reopen does not add duplicate model subscriptions or button handlers.
- Transient request streams are not consumed while their overlay window is hidden.
- DOTween, if used, is killed or cleaned on hide/pool return/final disposal.
- At least one PlayMode test exercises real `WindowSystem.Show -> Hide -> Reopen`.
- The target project has a short local README or architecture note explaining which parts of this reference were adopted and which were intentionally changed.

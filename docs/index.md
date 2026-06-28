# UiWindows Documentation Index

This index helps humans and AI agents use the repository as a UI architecture reference for other Unity projects.

## Recommended Read Order

1. [../README.md](../README.md) - repository purpose, quick architecture summary, and reference usage prompt.
2. [reference-adoption-checklist.md](reference-adoption-checklist.md) - step-by-step checklist for adopting the architecture in another Unity project.
3. [ai-agent-reference-prompt.md](ai-agent-reference-prompt.md) - ready-to-use prompts for implementation and review agents.
4. [reference-architecture-diagram.md](reference-architecture-diagram.md) - layer, dependency, runtime show, and request-port diagrams.
5. [project-architecture-skeleton.md](project-architecture-skeleton.md) - folder, assembly, scene, and ownership boundaries.
6. [uiwindows-mvp-openui-migration-guide.md](uiwindows-mvp-openui-migration-guide.md) - core UI.Windows MVP migration rules and request-port patterns.
7. [../Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md](../Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md) - presenter/window adapter API and lifecycle mapping.
8. [r3-mvp-conventions.md](r3-mvp-conventions.md) - R3 dependency pins, public-port rules, and subscription lifetime conventions.
9. [uiwindows-mvp-pooling-lifecycle.md](uiwindows-mvp-pooling-lifecycle.md) - pooled window lifecycle, show-scope cleanup, and verification checklist.
10. [uiwindows-mvp-final-reconciliation.md](uiwindows-mvp-final-reconciliation.md) - completed `UIW-1` traceability matrix and migration evidence.
11. [uiwindows-mvp-production-hardening-plan.md](uiwindows-mvp-production-hardening-plan.md) - active hardening plan and known next improvements.
12. [uiwindows-mvp-production-hardening-baseline.md](uiwindows-mvp-production-hardening-baseline.md) - Editor-only baseline evidence, mobile-readiness gaps, and initial production hardening targets.

## Reference Adoption

- [reference-adoption-checklist.md](reference-adoption-checklist.md) explains how to transfer the architecture to another Unity project without copying demo code blindly.
- [ai-agent-reference-prompt.md](ai-agent-reference-prompt.md) gives implementation and review prompts for AI agents using this repository as a source reference.
- [reference-architecture-diagram.md](reference-architecture-diagram.md) gives Mermaid diagrams for the layer ownership, runtime show flow, request-port flow, and pooled-window rule.

## Architecture And Boundaries

- [project-architecture-skeleton.md](project-architecture-skeleton.md) defines the repository's architectural layers and scene roles.
- [../Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md](../Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter/README.md) defines the current presenter/window adapter API.
- [reference-architecture-diagram.md](reference-architecture-diagram.md) gives a compact visual map of the same boundaries.
- [r3-mvp-conventions.md](r3-mvp-conventions.md) defines the reactive state and subscription ownership rules.
- [uiwindows-mvp-pooling-lifecycle.md](uiwindows-mvp-pooling-lifecycle.md) defines show-scoped lifecycle behavior for pooled UI.Windows windows.
- [uiwindows-mvp-openui-migration-guide.md](uiwindows-mvp-openui-migration-guide.md) defines how OpenUI behavior was adapted without copying OpenUI infrastructure.

## Dependency And Setup Notes

- [ui-windows-fork-workflow.md](ui-windows-fork-workflow.md) records the pinned UI.Windows fork workflow.
- [dotween-ui-fx-dependency.md](dotween-ui-fx-dependency.md) records DOTween source/version/setup and usage boundaries.
- [r3-mvp-conventions.md](r3-mvp-conventions.md) records R3 and NuGetForUnity restore rules.

## Migration And Verification Evidence

- [uiwindows-mvp-final-reconciliation.md](uiwindows-mvp-final-reconciliation.md) is the final evidence package for the completed `UIW-1` migration.
- [uiwindows-mvp-visual-layout-parity-audit.md](uiwindows-mvp-visual-layout-parity-audit.md) records visual/layout parity and accepted deviations for migrated prefabs.
- [uiwindows-mvp-production-hardening-plan.md](uiwindows-mvp-production-hardening-plan.md) records the next production/mobile hardening iteration.
- [uiwindows-mvp-production-hardening-baseline.md](uiwindows-mvp-production-hardening-baseline.md) records the `UIW-27` Editor-only baseline evidence, target-device evidence gap, and initial mobile-readiness decision rules.

## Code Entry Points

- `Assets/Scripts/UiWindowsMvp/Runtime/UIAdapter` - reusable presenter lifecycle adapter.
- `Assets/Scripts/ProjectContext/Runtime` - model/read-model/command/request port examples.
- `Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows` - UI.Windows MVP view, presenter, launcher, and window examples.
- `Assets/Scripts/UiWindowsMvp/Tests/PlayMode` - presenter, lifecycle, pooling, and integrated workflow tests.
- `Assets/Prefabs/UiWindowsMvp/SampleSceneWindows` - migrated UI.Windows-compatible view prefabs.
- `Assets/Scenes/SampleScene.unity` - canonical runtime/integration scene.
- `Assets/Scenes/Develop/UIDevelopScene.unity` - static layout/prefab inspection scene.

## Reference Use Guidance

When using this repository from another project, copy patterns deliberately:

- Reuse lifecycle, boundary, and verification rules.
- Adapt sample presenters, ports, launchers, and scenes to the target project's domain.
- Keep target-project models independent from UI.Windows, Unity UI, presenters, DOTween, OpenUI, Zenject, and UniRx.
- Validate real UI.Windows lifecycle with PlayMode tests instead of relying only on prefab inspection.

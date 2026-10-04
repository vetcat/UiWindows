# UI.Windows MVP Reference

This repository is a Unity reference project for building UI on top of `UI.Windows-submodule` with a project-owned MVP / Model-View-View-Presenter layer and `Cysharp/R3` reactive model ports.

It is a working architecture sample, pattern library, and migration record for other Unity projects. The reusable MVP adapter and scene CompositionRoot are also available as [standalone UPM packages](UPM/README.md); sample scenes, models, views, art, and DOTween remain in the reference project.

## Install The Reusable Packages

In a Unity 6.6 project, add the corrected `com.me.ui.windows` fork and `com.vetcat.uiwindows.mvp` using the Git URLs in [UPM/README.md](UPM/README.md). Add `com.vetcat.composition-root` when you want the scene bootstrap helper. R3 is optional for these packages and is installed separately by projects that use it.

The repository root is a reference Unity project. Use the `?path=UPM/<package-name>` URLs to install the standalone packages, and pin a published commit for reproducible imports.

## Source Editor Tooling

Open this reference project with Unity `6000.6.4f1`. Editor automation uses Unity CLI
and the intentionally pinned `com.unity.pipeline` `0.8.0-exp.1`; the source does not
install Unity MCP. Pipeline is source tooling, not a dependency of the reusable
MVP/CompositionRoot packages. Linear and Rider MCP remain separate tools.

See [source CLI setup and checks](docs/upm-package-workflow.md#source-editor-tooling)
for exact-project readiness, command discovery, compile/Console checks, and the
focused nonzero test-result gate. Do not target another open project's Editor.

## Project History

This project was created as a synthesis of two existing UI architecture references:

- [OpenUI](https://github.com/vetcat/OpenUI) was used as the source for the MVP / Model-View-View-Presenter concept, presenter/view responsibilities, UI behavior examples, and representative SampleScene layouts.
- [UI.Windows-submodule](https://github.com/chromealex/UI.Windows-submodule) was used as the source for the window lifecycle model: loading, unloading, layouts, pooling, resource management, and runtime window ownership.

The original problem with directly adopting OpenUI was that its sample approach creates UI prefabs at application start regardless of whether a player will use those windows in the current session. That is workable for small demos, but it is a poor fit for large Unity projects with many windows, mobile memory pressure, and session-specific UI usage.

The task for this repository was:

```text
Keep the MVP / MVVP architecture idea and usage examples from OpenUI,
but replace the eager prefab/window ownership approach with UI.Windows lifecycle,
loading, unloading, layouts, pooling, and resource management.

Keep dependency ownership explicit and project-local.
Use R3 as the reactive foundation.
Use a simple CompositionRoot for explicit scene wiring.
Port representative OpenUI UI examples onto the new approach.
Verify pooled show/hide lifecycle, show-scoped subscription cleanup, and integrated SampleScene behavior.
Document the resulting architecture so it can be reused as a reference for other Unity projects.
```

The project-owned implementation and documentation in this reference repository were produced end-to-end with OpenAI Codex as the LLM coding agent, under Vitaly's direction, review, and task acceptance. Third-party packages and referenced repositories remain external sources; Codex produced the integration code, migration code, tests, and project documentation in this repository.

## Current Status

- The initial migration milestone is complete.
- The migrated SampleScene UI covers representative OpenUI-inspired HUD, settings, shop, modal, hints, FX, object indicator, and integrated workflow examples.
- The production hardening and mobile readiness follow-up phase is complete.
- The project is suitable as a reference for architecture, lifecycle, R3 ownership, test patterns, and prefab/scene organization.
- Mobile performance budgets, runtime fallback cleanup, and larger-data scaling are documented and locally hardened for the reference scene. Low-end Android target-device profiling remains a deferred next-stage evidence gap.

## How To Use This Repository As A Reference

For humans:

1. Start with this README.
2. Read [docs/index.md](docs/index.md) for the documentation map.
3. Read [docs/uiwindows-mvp-openui-migration-guide.md](docs/uiwindows-mvp-openui-migration-guide.md) for the core architecture rules.
4. Read [docs/reference-adoption-checklist.md](docs/reference-adoption-checklist.md) before applying the pattern to another project.
5. Read [docs/reference-architecture-diagram.md](docs/reference-architecture-diagram.md) for the layer and lifecycle diagrams.
6. Read [the presenter lifecycle](UPM/com.vetcat.uiwindows.mvp/Documentation~/presenter-lifecycle.md) before binding presenters through the installed adapter package.
7. Inspect one complete vertical slice before porting the pattern, for example `UiTopLeft`, `UiSettings`, `UiShop`, or `UiModal`.

For AI agents in another project, provide this repository link and an instruction like:

```text
Use https://github.com/vetcat/UiWindows as the UI architecture reference.
Read README.md, docs/index.md, docs/uiwindows-mvp-openui-migration-guide.md,
docs/reference-adoption-checklist.md, docs/reference-architecture-diagram.md,
docs/r3-mvp-conventions.md, and docs/uiwindows-mvp-pooling-lifecycle.md first.
Reuse the architecture patterns, lifecycle rules, and test strategy, but adapt names,
models, prefabs, and scene composition to the target project. Do not import OpenUI
runtime infrastructure or bypass UI.Windows lifecycle with GameObject.SetActive.
```

For a longer ready-to-use prompt, see [docs/ai-agent-reference-prompt.md](docs/ai-agent-reference-prompt.md).

## Architecture Summary

| Layer | Location | Responsibility |
| --- | --- | --- |
| Composition root | `UPM/com.vetcat.composition-root` | Scene service construction, initialization, post-Awake startup, disposal, and explicit dependency wiring. No UI.Windows, MVP, OpenUI, or R3 dependency in the reusable runtime assembly. |
| Model and ports | `Assets/Scripts/ProjectContext` | Project/application services and explicit read-model/command/request ports. May expose read-only R3 state or streams. Must not depend on UI.Windows windows, Unity UI views, presenters, DOTween, or OpenUI. |
| UI adapter | `UPM/com.vetcat.uiwindows.mvp/Runtime` | Presenter contracts, UI.Windows window binding, lifecycle event forwarding, and show-scoped subscription ownership. |
| UI implementation | `Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows` | UI.Windows windows, views, presenters, launcher examples, DOTween UI/effects rendering, and SampleScene-specific wiring. |
| Prefabs | `Assets/Prefabs/UiWindowsMvp` | UI.Windows-compatible view prefabs with serialized Unity UI references. |
| Runtime scene | `Assets/Scenes/SampleScene.unity` | Canonical integration scene for runtime lifecycle and workflow verification. |
| Layout preview | `Assets/Scenes/Develop/UIDevelopScene.unity` | Static prefab/layout inspection scene. It does not prove runtime lifecycle behavior. |

## Core Rules To Reuse

- UI.Windows owns window creation, loading, show/hide lifecycle, layouts, pooling, and cleanup.
- Open windows through `WindowSystem.Show`, `WindowSystem.ShowSync`, or a narrow wrapper that delegates to those APIs.
- Do not use `GameObject.SetActive` as the runtime window lifecycle.
- Bind presenters after UI.Windows has produced a window instance.
- Pooled windows keep one presenter binding per pooled window instance.
- Each show creates a fresh show scope; hide/pool return disposes it.
- Put R3 subscriptions, Unity UI button listeners, timers, frame streams, and request-stream listeners into show scope when they should only run while visible.
- Public model/read-model ports expose read-only R3 surfaces such as `ReadOnlyReactiveProperty<T>` or `Observable<T>`.
- Mutable `ReactiveProperty<T>` and `Subject<T>` stay inside their owner.
- Mutations cross boundaries through explicit command methods.
- Domain/model services publish state, commands, or requests; the UI layer decides how to render them.
- DOTween is allowed in UI/effects rendering code, not in ProjectContext public ports.

## Code Examples To Inspect

| Use case | Primary files |
| --- | --- |
| Presenter adapter lifecycle | `UPM/com.vetcat.uiwindows.mvp/Runtime/WindowPresenterBinding.cs`, `WindowPresenterBinder.cs`, `WindowPresenterShowScope.cs` |
| Simple HUD binding | `UiTopLeftPresenter.cs`, `UiTopLeftWindow.cs`, `UiRuntimeWindowSource.cs`, `UiRuntimeWindowHandle.cs`, `UiTopLeftDemoLauncher.cs` |
| Currency HUD and FX target | `UiTopRightPresenter.cs`, `UiFxTargetRegistry.cs`, `UiFxPresenter.cs` |
| Settings and localization | `UiSettingsPresenter.cs`, `UiDownRightPresenter.cs`, `ProjectContext/Runtime/Settings`, `ProjectContext/Runtime/Localization` |
| Shop collection pooling | `UiShopPresenter.cs`, `PooledViewCollection.cs`, `ProjectContext/Runtime/Shop` |
| Modal request port | `ProjectContext/Runtime/UiRequests/UiModalService.cs`, `UiModalPresenter.cs`, `UiModalDemoLauncher.cs` |
| Hint and FX request streams | `UiHintsPresenter.cs`, `UiFxPresenter.cs`, `ProjectContext/Runtime/UiRequests/UiFeedbackService.cs` |
| Object indicator layer | `UiObjectIndicatorPresenter.cs`, `UiRuntimeWindowHandle.cs`, `CameraWorldToScreenAdapter.cs` |
| Integrated acceptance workflow | `Assets/Scripts/UiWindowsMvp/Tests/PlayMode/SampleSceneIntegratedAcceptanceTests.cs` |

## Documentation Map

- [UPM/README.md](UPM/README.md) - standalone package installation, dependencies, and test setup.
- [docs/upm-package-workflow.md](docs/upm-package-workflow.md) - canonical source ownership and publication/update workflow.
- [docs/index.md](docs/index.md) - documentation index and recommended read order.
- [docs/reference-adoption-checklist.md](docs/reference-adoption-checklist.md) - step-by-step adoption checklist for another Unity project.
- [docs/ai-agent-reference-prompt.md](docs/ai-agent-reference-prompt.md) - ready-to-use prompts for implementation and review agents.
- [docs/reference-architecture-diagram.md](docs/reference-architecture-diagram.md) - layer, dependency, runtime show, and request-port diagrams.
- [docs/project-architecture-skeleton.md](docs/project-architecture-skeleton.md) - folder, assembly, scene, and ownership boundaries.
- [UPM/com.vetcat.uiwindows.mvp/Documentation~/presenter-lifecycle.md](UPM/com.vetcat.uiwindows.mvp/Documentation~/presenter-lifecycle.md) - current presenter/window adapter API and lifecycle mapping.
- [docs/r3-mvp-conventions.md](docs/r3-mvp-conventions.md) - R3 dependency pins, public-port rules, and lifetime rules.
- [docs/uiwindows-mvp-pooling-lifecycle.md](docs/uiwindows-mvp-pooling-lifecycle.md) - pooled UI.Windows window lifecycle and verification checklist.
- [docs/uiwindows-mvp-openui-migration-guide.md](docs/uiwindows-mvp-openui-migration-guide.md) - final OpenUI-to-UI.Windows MVP migration rules.
- [docs/uiwindows-mvp-final-reconciliation.md](docs/uiwindows-mvp-final-reconciliation.md) - traceability evidence for the completed migration milestone.
- [docs/uiwindows-mvp-production-hardening-plan.md](docs/uiwindows-mvp-production-hardening-plan.md) - production hardening and mobile readiness plan.
- [docs/uiwindows-mvp-loading-prewarm-policy.md](docs/uiwindows-mvp-loading-prewarm-policy.md) - loading, prewarm, first-show, and residency policy for migrated window families.
- [docs/uiwindows-mvp-mobile-ui-audit.md](docs/uiwindows-mvp-mobile-ui-audit.md) - mobile UI layout, raycast, and legacy text audit for migrated prefabs.
- [docs/uiwindows-mvp-dynamic-scaling.md](docs/uiwindows-mvp-dynamic-scaling.md) - dynamic shop collection and object indicator update-source scaling decisions.
- [docs/uiwindows-mvp-prefab-first-fallback-audit.md](docs/uiwindows-mvp-prefab-first-fallback-audit.md) - prefab-first fallback decisions for migrated views and retained dynamic UI paths.
- [docs/uiwindows-mvp-validation-gates.md](docs/uiwindows-mvp-validation-gates.md) - local validation command and mandatory/conditional gates for future agents.
- [docs/uiwindows-mvp-production-hardening-final-reconciliation.md](docs/uiwindows-mvp-production-hardening-final-reconciliation.md) - final production-hardening traceability matrix, deferred gaps, and parent closure record.

## Dependency Baseline

- Unity target `6000.6.4f1`.
- `com.me.ui.windows`: `https://github.com/vetcat/UI.Windows-submodule.git#6339d2ecdaaa7b1b0e06d9608497809eeeb9bef7` (package `1.2.9`, synchronous owner shutdown).
- `R3.Unity`: `https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity#1.3.1`.
- NuGet `R3`: `1.3.1`, restored under `Packages/nuget-packages`.
- NuGetForUnity: `v4.5.0`.
- DOTween Free `1.2.825` under `Assets/Plugins/Demigiant/DOTween`.
- Source Editor automation: Unity CLI with Pipeline `0.8.0-exp.1`, excluded from reusable package dependencies.

See [docs/r3-mvp-conventions.md](docs/r3-mvp-conventions.md), [docs/ui-windows-fork-workflow.md](docs/ui-windows-fork-workflow.md), and [docs/dotween-ui-fx-dependency.md](docs/dotween-ui-fx-dependency.md) for setup details.

## What Not To Copy Blindly

- Install the standalone packages for reusable runtime infrastructure. Copy sample scenes, models, and demo UI only when the target project needs those examples.
- Do not import OpenUI runtime infrastructure just because OpenUI inspired the behavior.
- Do not move SampleScene-specific launcher or demo data into a production game's domain model.
- Do not treat `UIDevelopScene` as runtime verification; use PlayMode tests and `SampleScene` lifecycle flows.
- Do not preserve runtime-built fallback UI as a final production asset without reviewing the production hardening notes.

## Verification References

The strongest current verification path is:

- `tools/validate-uiwindows-hardening` for local architecture/dependency, lifecycle shortcut, diff whitespace, and final-newline gates.
- Focused adapter/CompositionRoot PlayMode tests in the canonical UPM packages, plus reference presenters/lifecycle tests under `Assets/Scripts/UiWindowsMvp/Tests/PlayMode` and model tests under `Assets/Scripts/ProjectContext/Tests/PlayMode`.
- Real `WindowSystem.Show -> Hide -> Reopen` lifecycle tests for pooled windows.
- `SampleSceneIntegratedAcceptanceTests.SampleScene_RunIntegratedMigratedWorkflowAndRepresentativeShowScopeCleanup`.
- Static dependency and lifecycle scans recorded in the final reconciliation docs.

When adapting this architecture in another project, keep the same verification shape: focused presenter tests, real UI.Windows lifecycle tests, and one integrated scene workflow that exercises representative navigation, requests, pooling, and cleanup.

## Recommended Next-Stage Work

The completed `UIW-26` production-hardening phase covers the local reference-scene
baseline, loading/prewarm policy, launcher consolidation, mobile UI cleanup,
dynamic-path scaling, prefab-first fallback cleanup, and validation gates.

The remaining work is outside the closed migration and hardening milestones:

- Run low-end Android target-device profiling for startup, cold first show, warm
  reopen, frame pacing, memory, and managed allocations.
- Use the profiling results to decide whether shop/settings/modal windows need
  earlier prewarm, asynchronous loading, loading affordances, or cleanup-after-use.
- Add shop visible-row virtualization only when real catalog size or profiling
  evidence justifies the added state and edge cases.
- Add object-indicator culling/throttling and source-specific multi-indicator FX
  routing only when production scenes have multiple real targets and measured need.
- Consider a TextMeshPro and layout simplification pass for production UI polish
  after the target platform and content constraints are known.

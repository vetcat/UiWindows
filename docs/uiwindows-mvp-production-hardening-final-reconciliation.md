# UI.Windows MVP Production Hardening Final Reconciliation

Date: 2026-06-28
Issue: `UIW-34` - `07 - Final production hardening reconciliation and next-stage readiness review`
Parent: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`

## Purpose

This document is the final reconciliation gate for `UIW-26`.

It maps the parent production-hardening and mobile-readiness targets to current
repository artifacts, verification evidence, accepted deviations, deferred gaps, and
the parent closure recommendation. It should be read together with:

- `docs/uiwindows-mvp-production-hardening-plan.md`
- `docs/uiwindows-mvp-production-hardening-baseline.md`
- `docs/uiwindows-mvp-loading-prewarm-policy.md`
- `docs/uiwindows-mvp-mobile-ui-audit.md`
- `docs/uiwindows-mvp-dynamic-scaling.md`
- `docs/uiwindows-mvp-prefab-first-fallback-audit.md`
- `docs/uiwindows-mvp-validation-gates.md`
- `docs/uiwindows-mvp-final-reconciliation.md`

`UIW-26` is not a reopening of the completed `UIW-1` migration milestone. It is a
production-hardening pass over the accepted UI.Windows MVP reference implementation.

## Evidence Reviewed

The reconciliation reviewed:

- Linear parent `UIW-26` and child issues `UIW-27` through `UIW-34`.
- Child issue descriptions, implementation notes, Orchestrator reviews, closure notes,
  and post-closure documentation drift notes.
- Current local production-hardening docs and validation tooling.
- Current repository state on `main` as of `4ff0bde` before the `UIW-34` branch.
- The earlier `UIW-1` final reconciliation pattern in
  `docs/uiwindows-mvp-final-reconciliation.md`.

Live Linear status checked on 2026-06-28 before this document:

| Issue | Title | Linear status at reconciliation start | Primary artifact |
| --- | --- | --- | --- |
| `UIW-27` | `00 - Establish production hardening baseline and mobile readiness targets` | `Done` | `docs/uiwindows-mvp-production-hardening-baseline.md` |
| `UIW-28` | `01 - Consolidate UI.Windows MVP launcher and runtime window source infrastructure` | `Done` | `UiRuntimeWindowHandle<TWindow, TView>` |
| `UIW-29` | `02 - Define mobile loading, prewarm, and first-show policy` | `Done` | `docs/uiwindows-mvp-loading-prewarm-policy.md` |
| `UIW-30` | `03 - Audit and optimize UI layout, raycast, and text settings for mobile` | `Done` | `docs/uiwindows-mvp-mobile-ui-audit.md` |
| `UIW-31` | `04 - Scale dynamic collections and object indicator update paths` | `Done` | `docs/uiwindows-mvp-dynamic-scaling.md` |
| `UIW-32` | `05 - Move fallback-built UI toward prefab-first production assets` | `Done` | `docs/uiwindows-mvp-prefab-first-fallback-audit.md` |
| `UIW-33` | `06 - Add production hardening validation gates for future agents` | `Done` | `tools/validate-uiwindows-hardening` |
| `UIW-34` | `07 - Final production hardening reconciliation and next-stage readiness review` | `In Progress` | This document |

## Final Recommendation

`UIW-26` is ready for parent closure after `UIW-34` itself is reviewed, accepted,
merged, pushed, and closed through the normal task flow.

No additional `UIW-26` child issue is required before parent closure. The remaining
mobile evidence gap is intentionally deferred by the parent plan and `UIW-27`
baseline: low-end Android and iOS target-device builds/profiling were not authorized
for this iteration. That gap should be carried into a next-stage profiling plan when
Vitaly wants target-device evidence.

This is not a claim that the reference project is proven production-ready on low-end
Android hardware. It is a claim that `UIW-26` satisfied its accepted local hardening
scope: Editor/static baseline, loading policy, targeted mobile UI cleanup, dynamic
path scaling, prefab-first cleanup, and local validation gates while preserving the
accepted architecture boundaries.

## Traceability Matrix

| Parent target | Implemented artifact(s) | Verification evidence | Status |
| --- | --- | --- | --- |
| Preserve `UIW-1` architecture boundaries: UI.Windows owns lifecycle/resource/pooling, presenters own view behavior, and `ProjectContext` stays independent from UI.Windows, Unity UI, presenters, DOTween, OpenUI, Zenject, and UniRx. | Existing UI.Windows MVP adapter and ports; `UiRuntimeWindowHandle<TWindow, TView>`; `tools/validate-uiwindows-hardening`; boundary docs. | `UIW-28` through `UIW-33` static scans; validation gate scans for forbidden dependencies/lifecycle bypasses; Rider/Unity checks where code/assets changed. | Done |
| Reduce repeated launcher/runtime-source/factory/presenter-binding boilerplate. | `Assets/Scripts/UiWindowsMvp/Runtime/SampleSceneWindows/UiRuntimeWindowHandle.cs`; shared `UiRuntimeWindowSource<TWindow, TView>` path; updated launchers and docs. | `UIW-28` Executor and Orchestrator evidence: Rider diagnostics/build, Unity compile, `UiWindowsMvp.Tests.PlayMode` 36/36, integrated SampleScene spot-check, forbidden scans, final-newline and diff checks. | Done |
| Establish measurable mobile-readiness targets for first-show latency, UI allocations, layout rebuild cost, active tweens, raycast cost, and representative scene acceptance. | `docs/uiwindows-mvp-production-hardening-baseline.md`. | `UIW-27` baseline: 11-prefab inventory, focused integrated SampleScene test evidence, target rules, Editor/static/mobile-inference/device-gap separation. | Done |
| Define loading/prewarm/first-show policy for HUDs, modal/overlay windows, shop/settings windows, and request-driven FX/hints. | `docs/uiwindows-mvp-loading-prewarm-policy.md`; `UiWindowLoadingPolicyTests`. | `UIW-29` evidence: policy table per window family, Editor cold first-show and warm-reopen measurements, same pooled instance/binding assertions, `UiWindowsMvp.Tests.PlayMode` 37/37. | Done |
| Audit and improve UI layout, raycast, text, and canvas behavior for mobile. | `docs/uiwindows-mvp-mobile-ui-audit.md`; targeted prefab and fallback-builder raycast/text changes. | `UIW-30` evidence: active prefab raycast targets reduced 41 to 25; fixed-label legacy Text BestFit reduced 9 to 5; `UIDevelopScene` preview coverage preserved; `UiWindowsMvp.Tests.PlayMode` 37/37; direct count checks. | Done |
| Scale dynamic shop collection and object indicator update paths beyond the small demo data set. | `PooledViewCollection<TView>` count-aware active-row reuse; `UiShopView.ScrollToTop()` narrow content layout rebuild; `IUiObjectIndicatorUpdateSource`; `R3UiObjectIndicatorUpdateSource`; `docs/uiwindows-mvp-dynamic-scaling.md`. | `UIW-31` evidence: focused shop row reuse and shared indicator update-source tests; full `UiWindowsMvp.Tests.PlayMode` 39/39; hidden indicator unregister and show-scope cleanup coverage. | Done |
| Move demo/fallback runtime UI construction toward prefab-first, data/config-driven production patterns where useful. | `UiPrefabReferenceGuard`; removed broad static `BuildDefaultLayout` fallbacks from prefab-backed views; `UiViewTestFixtures` for tests; `docs/uiwindows-mvp-prefab-first-fallback-audit.md`. | `UIW-32` evidence: Rider diagnostics/build, Unity scene validation for `UIDevelopScene` and `SampleScene`, `UiWindowsMvp.Tests.PlayMode` 39/39, production `new GameObject` scan limited to documented dynamic paths. | Done |
| Add durable local validation gates for architecture, lifecycle, dependency, diff, text hygiene, and future-agent handoff checks. | `tools/validate-uiwindows-hardening`; `docs/uiwindows-mvp-validation-gates.md`; README/docs index links. | `UIW-33` evidence: `tools/validate-uiwindows-hardening --self-test`, `python3 -m py_compile tools/validate-uiwindows-hardening`, validation gate pass with generated pycache present, `git diff --check`, final clean worktree. | Done |
| Improve readability and maintainability for human and AI agents. | Shared launcher primitive; production-hardening docs; prefab-first guard; local validation command; docs index links. | Repeated Linear Orchestrator reviews accepted the changes as easier to scan and safer for future agents; validation gate codifies manual scans into one command. | Done |
| Keep mobile readiness evidence honest: Editor/static evidence is useful, but not low-end Android proof. | Baseline, loading policy, mobile UI audit, dynamic scaling doc, validation gates, and this reconciliation all label evidence class and deferred device gap. | Every measurement-oriented child records no Android/iOS build or target-device profiling. This reconciliation preserves that boundary and recommends next-stage device profiling outside `UIW-26`. | Done |
| Low-end Android and iOS target-device profiling for startup, first-show, frame pacing, memory, and allocations. | No repository artifact in `UIW-26`; intentionally outside this iteration unless Vitaly authorizes device work. | `UIW-26` planning inputs and `UIW-27` baseline explicitly defer device builds/profiling. Subsequent docs repeat that Editor evidence is not device proof. | Deferred |
| Very large shop catalog virtualization, object-indicator culling/throttling, and source-specific multi-indicator FX routing. | Current implementation adds count-aware pooling and a shared update source; full virtualization/culling remains future work. | `docs/uiwindows-mvp-dynamic-scaling.md` documents why current scope is sufficient for the reference scene and when to add larger production mechanisms. | Deferred |
| Future Addressables/remote/heavy-window asynchronous loading policy. | Current policy keeps direct-prefab UI.Windows sources with `forceSyncLoad` and `showSync`; future async policy is not needed for this reference scene. | `docs/uiwindows-mvp-loading-prewarm-policy.md` states future Addressables/remote/heavy windows need fresh device evidence and explicit UI.Windows loading/prewarm paths. | Deferred |

## Child Issue Evidence Summary

`UIW-27` established the baseline. It recorded Editor-only evidence, the 11-prefab
inventory, initial targets, child classification, and the explicit low-end Android /
iOS device-evidence gap. It did not change production code.

`UIW-28` consolidated runtime launcher infrastructure. The repeated lazy runtime
source creation, synchronous `WindowSystem.Show`, immediate UI.Windows hide, and
guarded presenter binding path now live in `UiRuntimeWindowHandle<TWindow, TView>`,
while concrete launchers retain special behavior locally.

`UIW-29` documented the loading, prewarm, first-show, and residency policy for all
migrated window families. The policy keeps persistent HUD/overlay surfaces resident,
keeps optional settings/modal paths on demand by default, recommends early idle
prewarm for likely/heavy shop flows, and preserves UI.Windows lifecycle ownership.

`UIW-30` performed the mobile UI audit and targeted cleanup. The pass reduced
avoidable raycast targets and fixed-label BestFit while leaving intentional controls,
blockers, dynamic/localized text, and broader layout/content changes documented for
later evidence.

`UIW-31` reduced dynamic-path churn. Shop rows reuse active pooled instances by
count, shop scroll reset rebuilds only the scroll content layout, and visible object
indicators share one UI-layer frame update source.

`UIW-32` moved prefab-backed static view shells away from broad runtime fallback
builders. Required prefab references now fail clearly, and tests use test-only
fixtures rather than relying on production views to rebuild full hierarchies.

`UIW-33` added the durable validation gate. Future agents can run one local command
for architecture/dependency, lifecycle shortcut, diff whitespace, and final-newline
checks, while still treating Rider, Unity, PlayMode, and target-device profiling as
conditional evidence.

`UIW-34` is this reconciliation. It should be accepted only if the document, final
validation evidence, and Linear final reconciliation note are present.

## Accepted Deviations And Deferred Gaps

- Target-device Android/iOS profiling is deferred. This is an explicit scope boundary,
  not hidden incomplete work.
- Editor stopwatch and managed-heap measurements are trend/regression evidence only.
  They are not marker-bounded Android profiler captures or exact allocation counts.
- The shop remains full materialized and pool-backed. Large production catalogs should
  add visible-row virtualization only when real catalog size or profiling evidence
  justifies the added complexity.
- Object indicators still update visible positions every frame. Culling, throttling,
  and a fuller multi-target manager remain deferred until production scenes have
  multiple real targets and measured need.
- Legacy `UnityEngine.UI.Text` remains in the migrated prefabs. BestFit was reduced
  where safe, but a TextMeshPro migration was not part of `UIW-26`.
- Direct-prefab runtime sources still use synchronous first show for this reference
  scene. Addressables, remote content, or larger window hierarchies require fresh
  loading policy and target-device evidence.

## UIW-34 Verification Evidence

This reconciliation branch changed documentation only:

- `README.md`
- `docs/index.md`
- `docs/uiwindows-mvp-production-hardening-final-reconciliation.md`

Verification run on 2026-06-28:

- `tools/validate-uiwindows-hardening --self-test`: passed.
- `tools/validate-uiwindows-hardening`: passed, including static
  architecture/dependency scans, `git diff --check` checks, and final-newline
  hygiene for the 3 changed text files.
- Unity MCP required resource pre-checks were read:
  `mcpforunity://custom-tools`, `mcpforunity://instances`, and
  `mcpforunity://editor/state`.
- Unity refresh was requested with asset scope and no compile request before test
  verification. The editor remained idle and ready for tools.
- Integrated SampleScene PlayMode acceptance test passed 1/1:
  `UiWindowsMvp.Tests.PlayMode.SampleSceneIntegratedAcceptanceTests.SampleScene_RunIntegratedMigratedWorkflowAndRepresentativeShowScopeCleanup`.
  Unity-reported summary duration was 1.7963768 seconds.
- Unity Console after the PlayMode run initially contained the known Test Runner
  result-save entry typed as `Exception` and one
  `Unity.PerformanceTesting.Editor.TestRunBuilder` cleanup warning. That ephemeral
  noise was cleared, then the Console error/warning query returned 0 entries.
- Rider diagnostics/build were not required because no C# files changed. Rider MCP
  availability was verified by listing the opened Unity solution modules.

This verification is sufficient to recommend `UIW-26` closure after normal review
and closure workflow. It remains Editor/local evidence only; it does not close the
deferred low-end Android target-device evidence gap.

## Closure Position

Recommended parent decision: close `UIW-26` after `UIW-34` is accepted and closed.

Recommended next-stage work, outside `UIW-26`, is a target-device profiling plan for
low-end Android startup, cold first-show, frame pacing, memory, and allocation
evidence. That plan should consume the local policy and validation artifacts created
here instead of reopening this parent hardening scope.

# UI.Windows MVP Production Hardening Plan

Date: 2026-06-27
Parent: `UIW-26` - `[Plan] UI.Windows MVP production hardening and mobile readiness`

## Purpose

`UIW-1` completed the migration goal: UI.Windows owns lifecycle/loading/pooling, the project owns the MVP adapter layer, R3 is the reactive foundation, and representative OpenUI SampleScene surfaces are migrated without Zenject or UniRx.

`UIW-26` is the next iteration. Its goal is to harden the implemented architecture for production-scale Unity/mobile work and for continued AI/human maintenance. This plan should improve readability, reduce repeated wiring, define mobile readiness evidence, and add validation gates without reopening the already accepted `UIW-1` migration scope.

## Non-Goals

- Do not replace UI.Windows lifecycle ownership with direct `SetActive`, manual instantiation, or manual destruction of runtime windows.
- Do not introduce Zenject, UniRx, OpenUI runtime infrastructure, or a project-owned custom reactive framework.
- Do not move domain/model ports toward Unity UI, UI.Windows windows, presenters, DOTween, or prefab references.
- Do not optimize by removing the presenter/read-model separation that made the migration testable.
- Do not close `UIW-26` only because all child issues are closed; run the final reconciliation child issue first.

## Improvement Targets

- Make the current implementation easier to scan and modify by humans and AI agents.
- Reduce duplicated launcher, runtime window source, factory, and presenter-binding code where a small shared primitive clearly lowers maintenance cost.
- Establish mobile readiness targets before making broad optimization changes.
- Define an explicit loading/prewarm/first-show policy for HUD, modal, hints, FX, settings, shop, and object indicator surfaces.
- Audit and improve UI layout, raycast, text, and canvas behavior for mobile.
- Scale dynamic shop collection and object indicator paths beyond the small demo data set.
- Move runtime-built fallback UI toward prefab-first production assets where practical.
- Add lightweight validation commands that future agents can run before reporting task completion.

## Mobile Readiness Inputs

- Primary target class: low-end Android devices.
- Runtime target: 60 FPS. Use the 16.67 ms frame budget as context for UI cost decisions while preserving headroom for gameplay and rendering.
- Initial baseline mode: capture Editor/Unity Profiler evidence first. Target-device builds and profiling are intentionally out of scope until Vitaly authorizes device work.
- Baseline docs must separate measured Editor evidence from mobile inference and explicitly mark unavailable target-device evidence as a device gap.
- Highest mobile risk to track: cold startup and cold first-show speed on low-end Android devices.

## Child Issue Order

Linear child issue status is the source of truth. Future agents should query Linear before choosing work.

- `UIW-27` - `00 - Establish production hardening baseline and mobile readiness targets`
- `UIW-28` - `01 - Consolidate UI.Windows MVP launcher and runtime window source infrastructure`
- `UIW-29` - `02 - Define mobile loading, prewarm, and first-show policy`
- `UIW-30` - `03 - Audit and optimize UI layout, raycast, and text settings for mobile`
- `UIW-31` - `04 - Scale dynamic collections and object indicator update paths`
- `UIW-32` - `05 - Move fallback-built UI toward prefab-first production assets`
- `UIW-33` - `06 - Add production hardening validation gates for future agents`
- `UIW-34` - `07 - Final production hardening reconciliation and next-stage readiness review`

## Execution Flow

Future Orchestrator chats should open `UIW-26`, inspect ordered child issues, and pick the first child issue that is not `Done` or `Canceled` unless Vitaly chooses another issue.

For each child issue:

1. Read `AGENTS.md` and `_bmad-output/project-context.md`.
2. Read the relevant project-specific skills before architecture or prefab work.
3. Create an `Executor Handoff` in Linear when delegating.
4. Work on a dedicated `feature/<issue-slug>` branch.
5. Keep changes inside the selected issue scope.
6. Run focused verification plus any issue-specific validation.
7. Record durable implementation notes and verification evidence in Linear.
8. After accepted closure, run a documentation drift check and update local context only for durable facts.

## Final Reconciliation Rule

`UIW-34` must reconcile `UIW-26` before parent closure. The final review must map:

- parent target
- implemented artifact
- verification evidence
- status: `Done`, `Partial`, `Deferred`, or `Missing`

If the production-hardening result is narrower than the parent goal, create follow-up tasks or ask Vitaly for explicit reduced-scope acceptance before closing `UIW-26`.

# UI.Windows Fork And UPM Dependency Workflow

Date: 2026-06-07
Compatibility revision updated: 2026-10-04

## Decision

This project uses a project-owned fork of `UI.Windows-submodule` as the Unity Package Manager source.

- Upstream repository: `https://github.com/chromealex/UI.Windows-submodule`
- Project fork: `https://github.com/vetcat/UI.Windows-submodule`
- Compatibility branch: `unity6000-compat`
- Original upstream baseline: `60a4bf6e47c85ad57935f633a53fc3ca8b707167`
- Current integration commit: `939e4f4e80a76f76ff608acfb9c2c4e566e268b2`, package `1.2.8`

The fork branch was created at the researched baseline commit:

```text
60a4bf6e47c85ad57935f633a53fc3ca8b707167
```

## UPM Target

`package.json` is at the repository root in `UI.Windows-submodule`, so `?path=` is not required.

The current committed-state dependency in `Packages/manifest.json` is:

```json
"com.me.ui.windows": "https://github.com/vetcat/UI.Windows-submodule.git#939e4f4e80a76f76ff608acfb9c2c4e566e268b2"
```

For committed project state, the revision must be a commit hash or immutable tag, not a floating branch.

For local investigation only, a branch target is acceptable:

```json
"com.me.ui.windows": "https://github.com/vetcat/UI.Windows-submodule.git#unity6000-compat"
```

Do not commit the branch-target form unless the dependency workflow is explicitly changed.

## Unity 6.6 Compatibility Revision

The published integration commit contains compatibility patch
`cb77d43933409ec1b1525086cc2596e2a74d6e36`, merged through
[fork PR #1](https://github.com/vetcat/UI.Windows-submodule/pull/1).
It replaces obsolete identity APIs with full `EntityId` values, adapts the layout
max-size argument, preserves animation-state serialization, declares stable package
dependencies, and keeps FMOD/URP integrations optional. Shared prefabs contain no
source-demo registry/root or mandatory URP components. Existing asset GUIDs remain.

See the fork's
[compatibility notes](https://github.com/vetcat/UI.Windows-submodule/blob/939e4f4e80a76f76ff608acfb9c2c4e566e268b2/COMPATIBILITY.md)
for changed files, integration requirements, and older API version guards.

The 2026-10-04 compatibility verification in PixelLords on Unity `6000.6.4f1`
covered Editor compilation, six binding tests, two real show/hide/reopen and R3
frame tests, prefab save/reimport, optional URP inclusion, and comparison of 407
existing metadata files without GUID mismatches. Current verification artifacts
are recorded in [QP-1](https://linear.app/qpixelstudio/issue/QP-1/naladit-obshij-upm-workflow-uiwindows-dlya-pixellords-i).
The source reference project separately passed 41 focused package/reference
PlayMode tests on the same Editor version, including real `SampleScene`
show/hide/reopen flows; its resolver/settings migration and remaining warnings
are recorded in [source verification](upm-package-workflow.md#reference-verification-on-unity-66).
Other consumer verification must be recorded separately; the older `UIW-12`
snapshot below remains historical evidence.

## Fork Setup

The fork should keep `upstream` configured to the original author repository:

```bash
git clone https://github.com/vetcat/UI.Windows-submodule.git
cd UI.Windows-submodule
git remote add upstream https://github.com/chromealex/UI.Windows-submodule.git
git fetch upstream
git checkout unity6000-compat
```

If a local clone already exists, verify remotes instead:

```bash
git remote -v
git remote set-url upstream https://github.com/chromealex/UI.Windows-submodule.git
git fetch upstream
```

## Upstream Update Procedure

Use merge or rebase consistently for the compatibility branch. Merge is the safer default for preserving patch history:

```bash
git checkout unity6000-compat
git fetch upstream
git merge --no-ff upstream/master
```

If the branch is intentionally kept linear, rebase can be used instead:

```bash
git checkout unity6000-compat
git fetch upstream
git rebase upstream/master
```

After resolving conflicts and applying compatibility patches:

```bash
git status --short
git rev-parse HEAD
git push origin unity6000-compat
```

Then update the Unity project dependency target to the resulting commit hash or to an immutable tag created in the fork.

## Unity Verification After Updates

Every fork update that changes the pinned dependency must be verified in the Unity project before it is accepted:

- Update `Packages/manifest.json` to the candidate commit or tag.
- Let Unity resolve/import packages.
- Check package resolution output and Unity Console errors.
- Run compile verification for the reference project's target Unity `6000.6.4f1`.
- Run focused package tests and real UI.Windows show/hide/reopen flows in the affected consumers.
- Record package-resolution or compile errors before starting broader UI.Windows integration work.

## Patch Policy

Compatibility patches belong in the fork when they are required for `UI.Windows-submodule` itself to compile or run under the target Unity version.

Examples that can live in the fork:

- Package metadata compatibility fixes.
- Assembly definition reference fixes.
- Unity package dependency compatibility adjustments.
- Minimal runtime fixes inside UI.Windows needed to preserve its own lifecycle, resources, pooling, or layout behavior.

Project-owned adapter code stays in this repository's canonical `UPM` packages;
reference/game models, scenes, and behavior stay in their application project.
See [upm-package-workflow.md](upm-package-workflow.md) for those source boundaries.

Examples that must stay in this project:

- MVP or Model-View-View-Presenter adapter layer.
- Scene `CompositionRoot`.
- Local event, observable, and disposable primitives.
- Project services, models, presenters, and UI registry.
- OpenUI example ports or project-specific UI behavior.

Do not import OpenUI as part of this dependency workflow.

## UIW-12 Verification Snapshot

Verified on 2026-06-07:

- Upstream `https://github.com/chromealex/UI.Windows-submodule` is reachable.
- Upstream `master` and `HEAD` resolve to `60a4bf6e47c85ad57935f633a53fc3ca8b707167`.
- Baseline commit exists and is a commit object.
- Baseline commit summary: `[Templates] EmptyLayout size fix`.
- `package.json` is located at repository root.
- `?path=` is not required for the UPM Git dependency.
- Fork `https://github.com/vetcat/UI.Windows-submodule` exists.
- Fork branch `unity6000-compat` exists at `60a4bf6e47c85ad57935f633a53fc3ca8b707167`.
- `Packages/manifest.json` was not changed in `UIW-12`; no Unity package resolution/import was required for this task.

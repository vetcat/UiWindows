# UI.Windows Fork And UPM Dependency Workflow

Date: 2026-06-07

## Decision

This project uses a project-owned fork of `UI.Windows-submodule` as the Unity Package Manager source.

- Upstream repository: `https://github.com/chromealex/UI.Windows-submodule`
- Project fork: `https://github.com/vetcat/UI.Windows-submodule`
- Compatibility branch: `unity6000-compat`
- Baseline commit: `60a4bf6e47c85ad57935f633a53fc3ca8b707167`

The fork branch was created at the researched baseline commit:

```text
60a4bf6e47c85ad57935f633a53fc3ca8b707167
```

## UPM Target

`package.json` is at the repository root in `UI.Windows-submodule`, so `?path=` is not required.

Use this exact committed-state dependency target when `UIW-2` adds the package to `Packages/manifest.json`:

```json
"com.me.ui.windows": "https://github.com/vetcat/UI.Windows-submodule.git#60a4bf6e47c85ad57935f633a53fc3ca8b707167"
```

For committed project state, the revision must be a commit hash or immutable tag, not a floating branch.

For local investigation only, a branch target is acceptable:

```json
"com.me.ui.windows": "https://github.com/vetcat/UI.Windows-submodule.git#unity6000-compat"
```

Do not commit the branch-target form unless the dependency workflow is explicitly changed.

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
- Run compile verification for Unity `6000.4.4f1`.
- Record package-resolution or compile errors before starting broader UI.Windows integration work.

## Patch Policy

Compatibility patches belong in the fork when they are required for `UI.Windows-submodule` itself to compile or run under the target Unity version.

Examples that can live in the fork:

- Package metadata compatibility fixes.
- Assembly definition reference fixes.
- Unity package dependency compatibility adjustments.
- Minimal runtime fixes inside UI.Windows needed to preserve its own lifecycle, resources, pooling, or layout behavior.

Project-owned adapter code must stay in this Unity project, not in the fork.

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

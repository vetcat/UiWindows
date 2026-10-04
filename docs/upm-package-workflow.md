# Reusable UI Package Workflow

## Source Ownership

| Source | Owner | Reference-project use |
| --- | --- | --- |
| `vetcat/UI.Windows-submodule` | Window lifecycle, loading, layouts, pools, resources, and Unity compatibility. | Verified Git commit in `Packages/manifest.json`. |
| `UPM/com.vetcat.uiwindows.mvp` | Presenter lifecycle adapter and focused binding/show-scope tests. | `file:../UPM/com.vetcat.uiwindows.mvp`. |
| `UPM/com.vetcat.composition-root` | Scene composition/lifecycle helper and focused tests. | `file:../UPM/com.vetcat.composition-root`. |
| `Assets/Scripts` | Reference-scene installers, domain ports/models, demo UI, R3 smoke, and integration tests. | Local sample code; excluded from published packages. |

The UPM directories are the single source of reusable code. Modify them directly;
the reference project and Git consumers compile the same files. There is no export
step, mirrored `Assets` runtime, or independently maintained embedded distribution.
Local paths are relative to the project `Packages` folder, as specified by the
[Unity Package Manager manual](https://docs.unity3d.com/6000.0/Documentation/Manual/upm-localpath.html).

The standalone MVP assembly depends on UI.Windows and `IDisposable`. It does not
require CompositionRoot, R3, DOTween, FMOD, or URP. The CompositionRoot assembly
depends only on Unity. Applications decide how to construct presenters, supply
reactive model ports, and render effects.

## Source Editor Tooling

Use the installed Unity CLI and package-management skills for Editor automation.
Open this source project with Unity `6000.6.4f1`; avoid launching a second Editor
for the same path. `com.unity.pipeline` `0.8.0-exp.1` is the intentional source
tooling dependency and is retained in the manifest/lock. `com.coplaydev.unity-mcp`
is not installed here. Linear/Rider MCP and other projects' tooling are unchanged.
Neither reusable UPM package depends on Pipeline, Unity MCP, R3, or DOTween.

Check installed CLI version/latest without automatically upgrading. Set the path
to this checkout, and retain the explicit selector on every Editor-driving call:

```bash
task_project=/Users/vitaly/Projects/UiWindows
unity version --format json
unity self-update --check --format json
unity status --until-ready --timeout 40 --project-path "$task_project" --format json
unity command --query run_tests --detail full --project-path "$task_project" --format json
unity recompile --project-path "$task_project" --format json
unity command console_status --project-path "$task_project" --format json
```

Verify the selected path, PID, and Editor version before mutations. Discover live
schemas for additional commands (`console`, package/asset commands, `test_status`)
with `command --query <term> --detail full` and the same project selector. Automation
calls should also identify `--caller plugin --skill unity-cli`. Use `command eval`
for focused public Editor APIs when a registered command does not cover the check.
Change packages through asynchronous Package Manager commands/`Client` requests,
poll completion, and let Unity write manifest/lock; do not edit them by hand or
busy-loop on the Editor main thread.

The focused source PlayMode baseline is:

| Assembly | Expected tests |
| --- | --- |
| `UiWindowsMvp.UIAdapter.Tests.PlayMode` | 6 |
| `CompositionRoot.Tests.PlayMode` | 4 |
| `UiWindowsMvp.Tests.PlayMode` | 34 |

Run each assembly separately using the discovered schema, for example:

```bash
unity command run_tests --mode playmode --filter_type assembly --filter UiWindowsMvp.UIAdapter.Tests.PlayMode --async_tests true --project-path "$task_project" --format json
unity command test_status --project-path "$task_project" --format json
```

Poll until completed before starting the next run. Inspect the nonzero inventory,
passed/failed/skipped/inconclusive counts, and named results; an accepted dispatch
or zero-test result is not a pass. Preserve JSON plus actual NUnit XML exported
through the source Test Runner API/window under ignored `Logs/QP1-validation/`.
Inspect `scriptCompilationFailed` and actual Console errors/warnings separately
from an up-to-date compile summary. Preserve the original loaded scene state and
any test-modified preferences. This gate does not run player/WebGL builds.

## Local Development

1. Read `AGENTS.md`, [project context](project-context.md), and the relevant
   architecture page. Work on a task branch and inspect the worktree first.
2. Change reusable behavior in its UPM owner. Keep game/sample behavior under
   `Assets/Scripts`; keep the base library's compatibility changes in its fork.
3. Preserve existing script and asset `.meta` GUIDs. Runtime GUIDs match the
   original reference scripts and the PixelLords embedded packages, so changing
   the import location preserves serialized references.
4. Run `tools/validate-uiwindows-hardening`, relevant focused package tests,
   reference-scene lifecycle/integration tests when affected, and Editor
   compile/Console checks. No player builds are part of the default gate.

The reference manifest enables package tests using `testables`. Adapter unit tests
live in `UiWindowsMvp.UIAdapter.Tests.PlayMode`; scene/presenter integration tests
remain in `UiWindowsMvp.Tests.PlayMode`. Their asmdefs have distinct names and GUIDs.
CompositionRoot tests live only in `CompositionRoot.Tests.PlayMode` in its package.

## Publish And Update Consumers

1. Review the source changes and verification evidence. Publish the accepted
   base fork revision first when the adapter needs a new base version.
2. Update the MVP package's `com.me.ui.windows` version requirement, the reference
   project's base Git pin, and its resolved lock entry together. Let Unity resolve
   any changed registry dependencies and verify compilation before acceptance.
3. Review and merge the UiWindows task into `main` through the normal PR process.
   Verify that the selected commits are reachable in the remote repositories.
4. In each consumer, update the direct Git entries to those published commits.
   See [UPM installation](../UPM/README.md). Use immutable commits/tags for committed
   state; record both the UiWindows revision and base-fork revision.
5. Remove overlapping embedded package folders and old duplicate runtime scripts.
   Preserve consumer-only assets/settings and verify their serialized references.
6. Let Unity resolve/import, then verify compilation, Console diagnostics, and
   relevant `Show -> Hide -> Reopen`/cleanup flows. Commit the resolver-generated
   lock changes with the manifest update and verification record.

Install only the shared modules used by a consumer. PixelLords uses all three;
QuantumAsteroids currently uses the base UI.Windows package.

UPM edits belong in the source repository. A bug found in a consumer should return
to its owner, receive a focused regression check, and be distributed by a new Git
revision update. Editing a consumer's package cache is temporary investigation;
it does not publish a reusable fix.

## Reference Metadata

The reference project targets Unity `6000.6.4f1`. Its local package manifest/lock
entries point at the canonical UPM folders; they do not claim a Git publication.
An Editor import and tests provide runtime evidence separately from those pins.
Its base package is UI.Windows `1.2.9`, pinned to the published shutdown commit
`6339d2ecdaaa7b1b0e06d9608497809eeeb9bef7`. The MVP package retains its compatible
minimum base version requirement `1.2.8`. See [fork compatibility](ui-windows-fork-workflow.md)
for the baseline, published patch, and verification record.

## Reference Verification On Unity 6.6

The post-Awake startup and scene-owner shutdown update passed 44 PlayMode tests
on Unity `6000.6.4f1`: 4 CompositionRoot, 6 adapter and 34 reference integration
tests, with no failed/skipped/inconclusive tests. The new reference test proves
synchronous singleton release, exactly-once module cleanup, and that deferred
destruction preserves a replacement. PixelLords separately passed 15 tests,
including real FeelTest pooling, shutdown and scene reload. XML/JSON evidence is
under ignored `Temp/Codex/FeelTest/` in each project. Source compilation had no
errors; the existing DOTween teardown warnings remain. Rider's current solution
did not index source package paths; live Unity compilation is authoritative.
The static architecture/newline checks passed. The full shell gate still reports
the pre-existing trailing whitespace in `ProjectSettings.asset`, excluded from
this change. The original clean `UIDevelopScene` was restored. No device or
player-build validation is part of this update.

Earlier verification of the compatibility/tooling baseline follows.

Verified on 2026-10-04 in the live source Editor `6000.6.4f1`:

- UI.Windows resolved as Git package `1.2.8` at the integration commit above;
  both reusable packages resolved from their canonical local `UPM` folders.
  Editor assembly source inspection confirmed the adapter and CompositionRoot
  runtime files compile from those folders.
- `UiWindowsMvp.UIAdapter.Tests.PlayMode`: 6/6 passed.
- `CompositionRoot.Tests.PlayMode`: 2/2 passed.
- `UiWindowsMvp.Tests.PlayMode`: 33/33 passed, including the integrated
  `SampleScene` workflow, nine real window lifecycle tests, and loading-policy
  smoke. All runs had nonzero counts and no failed, skipped, or inconclusive tests.
- Editor compilation succeeded. The final compile check was up to date;
  `scriptCompilationFailed` was false and Unity Console had zero errors.
  The original clean `SampleScene` was restored after the test runs.
- The obsolete `com.unity.modules.vr` dependency failed resolution on Unity 6.6
  and was removed through the Editor's Package Manager Client API. The final
  resolver lock records Unity 6.6 built-ins, including URP `17.6.0` and uGUI
  `2.6.0`, with the matching Editor-generated project/URP settings migrations.
- Sample test identity comparisons were migrated from
  `GetInstanceID()`/`int` to `GetEntityId()`/`EntityId`. Reusable runtime contracts
  and GUIDs did not change. Five icon sprites and their actual prefab Image
  references resolved with the existing GUIDs and local file ID `21300000`,
  including after restoration and forced import of the original metadata.

Warnings remain: the initial compile reported 34 sample/test warnings (obsolete
object-search overloads and the settings view's inherited helper-name collision).
Reference test teardown emitted 15 DOTween warnings, including a Safe Mode summary
of 14 captured destroyed-target accesses. These did not fail the tests or appear
as Unity Console errors; warning cleanup is outside this package ownership pass.
This verification does not establish warning-free runtime behavior or player-build
compatibility.

During the initial import-validation pass, Unity MCP exposed only the separate
QuantumAsteroids Editor, and Rider did not index this source project. That pass
used temporary Pipeline `0.8.0-exp.1`, then removed it through `Client.Remove`.
The subsequent source tooling migration intentionally retains that same Pipeline
version for Unity CLI and removes source Unity MCP through Package Manager.
The active tooling baseline is defined above; the earlier removal is historical.
Local test and Editor evidence is retained under ignored `Logs/QP1-validation/`;
the durable task checkpoint is in
[QP-1](https://linear.app/qpixelstudio/issue/QP-1/naladit-obshij-upm-workflow-uiwindows-dlya-pixellords-i).
No player or WebGL builds were run.

The post-migration CLI-only rerun used CLI `1.0.0-beta.12` (also the latest version
reported by its non-mutating check), source Unity `6000.6.4f1`, and Pipeline
`0.8.0-exp.1`. After Package Manager completed Unity MCP removal, all three suites
passed again: 6/6 adapter, 2/2 CompositionRoot, and 33/33 reference tests, with zero
failed/skipped/inconclusive. Fresh actual NUnit XML and completed CLI JSON are
`Logs/QP1-validation/cli-{mvp-binding,composition-root,reference}-playmode.{xml,json}`.
The clean `SampleScene` and the three test-modified PlayerPrefs keys were restored.
Final `recompile` was up to date (0 errors/warnings in that summary), while live
Console ground truth reported compilation failure false, 0 errors, and 15 DOTween
warnings. Pipeline's retained buffer also contains one earlier sample compiler
warning; this is not a warning-free runtime claim.

The five icons retain their Unity 6.6 TextureImporter migration (version 13 and iOS
platform naming). Original GUIDs, Single Sprite mode, existing import settings,
Sprite local file IDs `21300000`, and all five prefab Image references remain
unchanged, including the captured forced-import check. No reusable runtime,
prefab, scene, or package API changes were needed for the tooling migration.
Unrelated default ProjectAuditor settings were quarantined recoverably under
ignored `Logs/QP1-validation/generated-noise/`, rather than published.

Architecture guidance is retained in ordinary [project context](project-context.md),
the [documentation index](index.md), and independent `.agents/skills` for issue
delivery, MVP architecture, and view prefab work.

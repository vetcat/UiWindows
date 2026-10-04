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
Its base package is UI.Windows `1.2.8`, pinned to the published integration commit
`939e4f4e80a76f76ff608acfb9c2c4e566e268b2`. The MVP package declares that same
base version requirement. See [fork compatibility](ui-windows-fork-workflow.md)
for the baseline, published patch, and verification record.

## Reference Verification On Unity 6.6

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
- Unity's API Updater migrated sample test identity comparisons from
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

Unity MCP exposed only the separate QuantumAsteroids Editor, and Rider did not
index this source project. Validation used the live source Editor through temporary
Pipeline `0.8.0-exp.1`; it was removed through `Client.Remove` afterwards and is
absent from the final manifest/lock. Local test and Editor evidence is retained
under ignored `Logs/QP1-validation/`; the durable task checkpoint is in
[QP-1](https://linear.app/qpixelstudio/issue/QP-1/naladit-obshij-upm-workflow-uiwindows-dlya-pixellords-i).
No player or WebGL builds were run.

Architecture guidance is retained in ordinary [project context](project-context.md),
the [documentation index](index.md), and independent `.agents/skills` for issue
delivery, MVP architecture, and view prefab work.

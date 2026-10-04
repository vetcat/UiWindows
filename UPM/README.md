# Standalone UPM packages

These folders are the canonical reusable source. This reference project's
`Packages/manifest.json` consumes them through local `file:` dependencies.
Git UPM imports the same folders; no export or source copying is needed.

| Package | Responsibility | Dependencies |
| --- | --- | --- |
| `com.vetcat.uiwindows.mvp` | Presenter binding, lifecycle forwarding, and show-scoped subscription cleanup. | Base `com.me.ui.windows` fork. |
| `com.vetcat.composition-root` | Explicit scene installers, service initialization, and disposal. | Unity only. |

Sample scenes, game models, demo windows/views/presenters, reactive smoke code,
and DOTween stay in the reference project. R3 and CompositionRoot are optional
for users of the MVP adapter.

## Install In Another Project

Use Unity `6000.6` or later. Add these dependencies to the consumer's
`Packages/manifest.json`, replacing `<UIWINDOWS_COMMIT>` with a published UiWindows commit:

```json
{
  "dependencies": {
    "com.me.ui.windows": "https://github.com/vetcat/UI.Windows-submodule.git#939e4f4e80a76f76ff608acfb9c2c4e566e268b2",
    "com.vetcat.uiwindows.mvp": "https://github.com/vetcat/UiWindows.git?path=UPM/com.vetcat.uiwindows.mvp#<UIWINDOWS_COMMIT>",
    "com.vetcat.composition-root": "https://github.com/vetcat/UiWindows.git?path=UPM/com.vetcat.composition-root#<UIWINDOWS_COMMIT>"
  }
}
```

Install only packages your project uses. The base package needs a direct Git
entry because the MVP package declares a package version dependency, which does
not identify the fork's Git repository. The current MVP package requires base
UI.Windows `1.2.8`; the example pins its published Unity 6.6 compatibility revision.
Use a verified base compatibility commit;
see [the fork workflow](../docs/ui-windows-fork-workflow.md).

Remove previous embedded copies and duplicate runtime scripts when switching
to Git UPM. Preserve their `.meta` GUIDs when moving project-specific assets;
the published reusable runtime retains its original script/asmdef GUIDs.

## Package Tests

To include focused tests in the Unity Test Runner, add these entries beside the
`dependencies` object in the consumer manifest:

```json
"testables": [
  "com.vetcat.composition-root",
  "com.vetcat.uiwindows.mvp"
]
```

The suites are `CompositionRoot.Tests.PlayMode` and
`UiWindowsMvp.UIAdapter.Tests.PlayMode`. The reference scene's additional
presenter, pooling, and integrated workflow tests remain in
`UiWindowsMvp.Tests.PlayMode` under `Assets/Scripts`.

Read the [MVP API](com.vetcat.uiwindows.mvp/README.md),
[presenter lifecycle](com.vetcat.uiwindows.mvp/Documentation~/presenter-lifecycle.md),
and [CompositionRoot API](com.vetcat.composition-root/README.md) before integration.
See [source and publication workflow](../docs/upm-package-workflow.md) for local
development, verification, and consumer revision updates.

# Standalone UPM packages

These package folders can be used as embedded packages or imported from this
repository using `?path=UPM/<package-name>#<commit>` after publishing this branch.
The reference project's Assets remain unchanged, so the original demo still works.
Do not install both the old Assets scripts and these packages into the same project.

`com.vetcat.uiwindows.mvp` requires `com.me.ui.windows` to be installed first.
Unity 6000.6 requires the corresponding UI.Windows compatibility fixes.
`com.vetcat.composition-root` is independent of UI.Windows and R3.

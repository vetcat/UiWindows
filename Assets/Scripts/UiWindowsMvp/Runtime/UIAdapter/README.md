# UI Adapter Boundary

Future `UIW-5` presenter/window adapter code belongs here.

This folder intentionally contains no presenter implementation in `UIW-3`. UI.Windows remains responsible for window show/hide, loading, unloading, layouts, pooling, and resource lifecycle.

This assembly may depend on `CompositionRoot.Runtime`; the reusable CompositionRoot module must not depend on UI, MVP, or UI.Windows.

R3 is already available through `UIW-13` and documented in `docs/r3-mvp-conventions.md`.

Future presenter adapter code should use the R3 conventions from that document for read-only reactive surfaces, owner-private mutable subjects/properties, command methods, and `IDisposable` subscription ownership. Do not add UniRx or a parallel custom reactive framework here.

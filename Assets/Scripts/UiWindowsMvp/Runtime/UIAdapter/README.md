# UI Adapter Boundary

Future `UIW-5` presenter/window adapter code belongs here.

This folder intentionally contains no presenter implementation in `UIW-3`. UI.Windows remains responsible for window show/hide, loading, unloading, layouts, pooling, and resource lifecycle.

This assembly may depend on `CompositionRoot.Runtime`; the reusable CompositionRoot module must not depend on UI, MVP, or UI.Windows.

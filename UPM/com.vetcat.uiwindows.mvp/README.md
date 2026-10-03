# UiWindows MVP

Extracted from vetcat/UiWindows, commit `7b89105`. Assembly: `UiWindowsMvp.UIAdapter`.
This package contains the reusable adapter, with no sample game models or views.
UI.Windows owns loading, layouts, show/hide, pooling and resource cleanup.

Bind a presenter after UI.Windows creates a window, normally inside the callback
of `WindowSystem.Show` or `ShowSync`:

```csharp
if (!WindowPresenterBinder.TryGetBinding(window, out _))
    WindowPresenterBinder.Bind(window, presenterFactory);
```

Keep one presenter binding for the lifetime of a pooled window instance.
`Initialize` runs once. Each `OnShowBegin(IUiShowScope)` receives a new scope;
`OnHideEnd` disposes its subscriptions, including R3 subscriptions registered
with `scope.Add(readModel.Value.Subscribe(view.SetValue))`. Hidden pooled windows
must not retain model subscriptions or input handlers. Unregister UI listeners
by adding their removal action to the same scope.

Final `OnDeInitialized`/window destruction disposes the binding and presenter.
Do not create a second pool or another show/hide state machine in presenters.

The adapter uses `IDisposable` and does not require a reactive framework.
For R3 projects, expose read-only model ports and pass them through presenter
constructors. CompositionRoot is a separate optional package; keep service
resolution in installers/factories, outside views and presenters.

Android policy belongs to the game: prewarm frequently opened windows during
loading, cache selectively, release rare/heavy windows, and measure first-open
and repeated-open costs on the target device. A package import is not a mobile
performance measurement.

Tests: `UiWindowsMvp.Tests.PlayMode`.

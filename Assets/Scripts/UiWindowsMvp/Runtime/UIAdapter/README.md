# UI Adapter Boundary

`UIW-5` adds the minimal presenter/window lifecycle adapter here.

UI.Windows remains responsible for window show/hide, loading, unloading, layouts, pooling, and resource lifecycle. Bind presenters after UI.Windows has created a window instance, normally from a `WindowSystem.Show` or `WindowSystem.ShowSync` callback:

```csharp
WindowPresenterBinder.Bind(window, presenterFactory);
```

Current adapter names:

- `IUiPresenter` and `IWindowPresenter<TWindow>` define presenter lifecycle and concrete window binding.
- `IWindowPresenterFactory<TWindow>` creates presenters with explicit constructor dependencies or narrow ports.
- `WindowPresenterBinder` attaches one presenter binding to a `WindowBase` instance and subscribes to UI.Windows lifecycle events.
- `WindowPresenterBinding<TWindow>` maps UI.Windows lifecycle to presenter calls.
- `IUiShowScope` and `WindowPresenterShowScope` own show-scoped `IDisposable` subscriptions.

Lifecycle mapping:

- `WindowEvent.OnInitialized` -> `IUiPresenter.Initialize()`.
- `WindowEvent.OnShowBegin` -> new `IUiShowScope`, then `IUiPresenter.OnShowBegin(scope)`.
- `WindowEvent.OnShowEnd` -> `IUiPresenter.OnShowEnd()`.
- `WindowEvent.OnHideBegin` -> `IUiPresenter.OnHideBegin()`.
- `WindowEvent.OnHideEnd` -> `IUiPresenter.OnHideEnd()`, then show-scope disposal.
- `WindowEvent.OnDeInitialized` -> idempotent final binding and presenter disposal.

This assembly intentionally uses `IDisposable` as the subscription ownership boundary instead of depending directly on R3. R3 subscriptions implement `IDisposable` and should be added to `IUiShowScope` for show-scoped cleanup. Do not add a second reactive framework here.

This assembly may depend on `CompositionRoot.Runtime`; the reusable CompositionRoot module must not depend on UI, MVP, or UI.Windows.

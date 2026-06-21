using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftDemoLauncher : IDisposable
    {
        private readonly UiTopLeftView viewPrefab;
        private readonly UiTopLeftPresenterFactory presenterFactory;

        private UiTopLeftRuntimeWindowSource runtimeSource;
        private UiTopLeftWindow currentWindow;
        private bool disposed;

        public UiTopLeftDemoLauncher(UiTopLeftView viewPrefab, UiTopLeftPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiTopLeftWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();

            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException(
                    "A WindowSystem must exist before showing the UiTopLeft demo slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiTopLeftRuntimeWindowSource.Create(viewPrefab);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiTopLeftWindow;
        }

        public void Hide()
        {
            if (currentWindow == null || WindowSystem.HasInstance() == false ||
                currentWindow.GetState() >= ObjectState.Hiding)
            {
                return;
            }

            currentWindow.Hide(TransitionParameters.Default.ReplaceImmediately(true));
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Hide();
            currentWindow = null;
            runtimeSource?.Dispose();
            runtimeSource = null;
        }

        private void BindPresenter(WindowBase window)
        {
            if (window is not UiTopLeftWindow uiTopLeftWindow)
            {
                throw new InvalidOperationException(
                    $"Expected {nameof(UiTopLeftWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = uiTopLeftWindow;
            if (WindowPresenterBinder.TryGetBinding(uiTopLeftWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(uiTopLeftWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiTopLeftDemoLauncher));
            }
        }
    }
}

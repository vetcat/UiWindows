using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiObjectIndicatorDemoLauncher : IDisposable
    {
        private readonly UiObjectIndicatorView viewPrefab;
        private readonly UiObjectIndicatorPresenterFactory presenterFactory;

        private UiObjectIndicatorRuntimeWindowSource runtimeSource;
        private UiObjectIndicatorWindow currentWindow;
        private bool disposed;

        public UiObjectIndicatorDemoLauncher(
            UiObjectIndicatorView viewPrefab,
            UiObjectIndicatorPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiObjectIndicatorWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();

            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException(
                    "A WindowSystem must exist before showing the UiObjectIndicator demo slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiObjectIndicatorRuntimeWindowSource.Create(viewPrefab);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiObjectIndicatorWindow;
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
            if (window is not UiObjectIndicatorWindow indicatorWindow)
            {
                throw new InvalidOperationException(
                    $"Expected {nameof(UiObjectIndicatorWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = indicatorWindow;
            if (WindowPresenterBinder.TryGetBinding(indicatorWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(indicatorWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiObjectIndicatorDemoLauncher));
            }
        }
    }
}

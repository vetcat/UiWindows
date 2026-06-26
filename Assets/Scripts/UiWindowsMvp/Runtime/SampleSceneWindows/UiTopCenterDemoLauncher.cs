using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopCenterDemoLauncher : IDisposable
    {
        private readonly UiTopCenterView viewPrefab;
        private readonly UiTopCenterPresenterFactory presenterFactory;

        private UiTopCenterRuntimeWindowSource runtimeSource;
        private UiTopCenterWindow currentWindow;
        private bool disposed;

        public UiTopCenterDemoLauncher(UiTopCenterView viewPrefab, UiTopCenterPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiTopCenterWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();

            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException(
                    "A WindowSystem must exist before showing the UiTopCenter demo slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiTopCenterRuntimeWindowSource.Create(viewPrefab);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiTopCenterWindow;
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
            if (window is not UiTopCenterWindow uiTopCenterWindow)
            {
                throw new InvalidOperationException(
                    $"Expected {nameof(UiTopCenterWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = uiTopCenterWindow;
            if (WindowPresenterBinder.TryGetBinding(uiTopCenterWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(uiTopCenterWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiTopCenterDemoLauncher));
            }
        }
    }
}

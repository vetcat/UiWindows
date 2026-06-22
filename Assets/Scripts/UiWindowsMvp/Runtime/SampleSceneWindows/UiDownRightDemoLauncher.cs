using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownRightDemoLauncher : IDisposable
    {
        private readonly UiDownRightView viewPrefab;
        private readonly UiDownRightPresenterFactory presenterFactory;

        private UiDownRightRuntimeWindowSource runtimeSource;
        private UiDownRightWindow currentWindow;
        private bool disposed;

        public UiDownRightDemoLauncher(UiDownRightView viewPrefab, UiDownRightPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiDownRightWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();

            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException(
                    "A WindowSystem must exist before showing the UiDownRight demo slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiDownRightRuntimeWindowSource.Create(viewPrefab);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiDownRightWindow;
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
            if (window is not UiDownRightWindow uiDownRightWindow)
            {
                throw new InvalidOperationException(
                    $"Expected {nameof(UiDownRightWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = uiDownRightWindow;
            if (WindowPresenterBinder.TryGetBinding(uiDownRightWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(uiDownRightWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiDownRightDemoLauncher));
            }
        }
    }
}

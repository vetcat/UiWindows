using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopRightDemoLauncher : IDisposable
    {
        private readonly UiTopRightView viewPrefab;
        private readonly UiTopRightPresenterFactory presenterFactory;

        private UiTopRightRuntimeWindowSource runtimeSource;
        private UiTopRightWindow currentWindow;
        private bool disposed;

        public UiTopRightDemoLauncher(UiTopRightView viewPrefab, UiTopRightPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiTopRightWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();

            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException(
                    "A WindowSystem must exist before showing the UiTopRight demo slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiTopRightRuntimeWindowSource.Create(viewPrefab);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiTopRightWindow;
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
            if (window is not UiTopRightWindow uiTopRightWindow)
            {
                throw new InvalidOperationException(
                    $"Expected {nameof(UiTopRightWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = uiTopRightWindow;
            if (WindowPresenterBinder.TryGetBinding(uiTopRightWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(uiTopRightWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiTopRightDemoLauncher));
            }
        }
    }
}

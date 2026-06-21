using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiFxDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 12;

        private readonly UiFxView viewPrefab;
        private readonly UiFxPresenterFactory presenterFactory;

        private UiRuntimeWindowSource<UiFxWindow, UiFxView> runtimeSource;
        private UiFxWindow currentWindow;
        private bool disposed;

        public UiFxDemoLauncher(UiFxView viewPrefab, UiFxPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiFxWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();
            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException("A WindowSystem must exist before showing the FX slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiRuntimeWindowSource<UiFxWindow, UiFxView>.Create(
                viewPrefab,
                nameof(UiFxWindow),
                LayoutTagId,
                takeFocus: false);

            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiFxWindow;
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
            if (window is not UiFxWindow fxWindow)
            {
                throw new InvalidOperationException($"Expected {nameof(UiFxWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = fxWindow;
            if (WindowPresenterBinder.TryGetBinding(fxWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(fxWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiFxDemoLauncher));
            }
        }
    }
}
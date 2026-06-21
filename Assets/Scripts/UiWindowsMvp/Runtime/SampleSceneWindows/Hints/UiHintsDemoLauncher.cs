using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiHintsDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 11;

        private readonly UiHintsView viewPrefab;
        private readonly UiHintsPresenterFactory presenterFactory;

        private UiRuntimeWindowSource<UiHintsWindow, UiHintsView> runtimeSource;
        private UiHintsWindow currentWindow;
        private bool disposed;

        public UiHintsDemoLauncher(UiHintsView viewPrefab, UiHintsPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiHintsWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();
            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException("A WindowSystem must exist before showing the hints slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiRuntimeWindowSource<UiHintsWindow, UiHintsView>.Create(
                viewPrefab,
                nameof(UiHintsWindow),
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
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiHintsWindow;
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
            if (window is not UiHintsWindow hintsWindow)
            {
                throw new InvalidOperationException($"Expected {nameof(UiHintsWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = hintsWindow;
            if (WindowPresenterBinder.TryGetBinding(hintsWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(hintsWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiHintsDemoLauncher));
            }
        }
    }
}
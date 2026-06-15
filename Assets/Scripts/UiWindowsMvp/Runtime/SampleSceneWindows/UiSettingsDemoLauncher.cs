using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiSettingsDemoLauncher : IDisposable
    {
        private readonly UiSettingsView viewPrefab;
        private readonly UiSettingsPresenterFactory presenterFactory;

        private UiSettingsRuntimeWindowSource runtimeSource;
        private UiSettingsWindow currentWindow;
        private bool disposed;

        public UiSettingsDemoLauncher(UiSettingsView viewPrefab, UiSettingsPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiSettingsWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();

            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException(
                    "A WindowSystem must exist before showing the UiSettings demo slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiSettingsRuntimeWindowSource.Create(viewPrefab);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiSettingsWindow;
        }

        public void Hide()
        {
            if (currentWindow == null || currentWindow.GetState() >= ObjectState.Hiding)
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
            if (window is not UiSettingsWindow uiSettingsWindow)
            {
                throw new InvalidOperationException(
                    $"Expected {nameof(UiSettingsWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = uiSettingsWindow;
            if (WindowPresenterBinder.TryGetBinding(uiSettingsWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(uiSettingsWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiSettingsDemoLauncher));
            }
        }
    }
}
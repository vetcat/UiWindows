using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiSettingsDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 2;

        private readonly UiRuntimeWindowHandle<UiSettingsWindow, UiSettingsView> windowHandle;
        private bool disposed;

        public UiSettingsDemoLauncher(UiSettingsView viewPrefab, UiSettingsPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiSettingsWindow, UiSettingsView>(
                viewPrefab,
                presenterFactory,
                nameof(UiSettingsWindow),
                LayoutTagId,
                takeFocus: true,
                "A WindowSystem must exist before showing the UiSettings demo slice.");
        }

        public UiSettingsWindow CurrentWindow => windowHandle.CurrentWindow;

        public void Show()
        {
            ThrowIfDisposed();
            windowHandle.Show();
        }

        public void Hide()
        {
            windowHandle.Hide();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            windowHandle.Dispose();
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

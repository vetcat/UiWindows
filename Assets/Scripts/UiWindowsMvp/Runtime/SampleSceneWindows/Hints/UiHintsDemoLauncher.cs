using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiHintsDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 11;

        private readonly UiRuntimeWindowHandle<UiHintsWindow, UiHintsView> windowHandle;
        private bool disposed;

        public UiHintsDemoLauncher(UiHintsView viewPrefab, UiHintsPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiHintsWindow, UiHintsView>(
                viewPrefab,
                presenterFactory,
                nameof(UiHintsWindow),
                LayoutTagId,
                takeFocus: false,
                "A WindowSystem must exist before showing the hints slice.");
        }

        public UiHintsWindow CurrentWindow => windowHandle.CurrentWindow;

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
                throw new ObjectDisposedException(nameof(UiHintsDemoLauncher));
            }
        }
    }
}

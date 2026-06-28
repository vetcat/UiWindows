using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiFxDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 12;

        private readonly UiRuntimeWindowHandle<UiFxWindow, UiFxView> windowHandle;
        private bool disposed;

        public UiFxDemoLauncher(UiFxView viewPrefab, UiFxPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiFxWindow, UiFxView>(
                viewPrefab,
                presenterFactory,
                nameof(UiFxWindow),
                LayoutTagId,
                takeFocus: false,
                "A WindowSystem must exist before showing the FX slice.");
        }

        public UiFxWindow CurrentWindow => windowHandle.CurrentWindow;

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
                throw new ObjectDisposedException(nameof(UiFxDemoLauncher));
            }
        }
    }
}

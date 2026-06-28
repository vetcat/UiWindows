using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 1;

        private readonly UiRuntimeWindowHandle<UiTopLeftWindow, UiTopLeftView> windowHandle;
        private bool disposed;

        public UiTopLeftDemoLauncher(UiTopLeftView viewPrefab, UiTopLeftPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiTopLeftWindow, UiTopLeftView>(
                viewPrefab,
                presenterFactory,
                nameof(UiTopLeftWindow),
                LayoutTagId,
                takeFocus: false,
                "A WindowSystem must exist before showing the UiTopLeft demo slice.");
        }

        public UiTopLeftWindow CurrentWindow => windowHandle.CurrentWindow;

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
                throw new ObjectDisposedException(nameof(UiTopLeftDemoLauncher));
            }
        }
    }
}

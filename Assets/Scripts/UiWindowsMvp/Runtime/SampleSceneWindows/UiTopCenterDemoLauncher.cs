using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopCenterDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 16;

        private readonly UiRuntimeWindowHandle<UiTopCenterWindow, UiTopCenterView> windowHandle;
        private bool disposed;

        public UiTopCenterDemoLauncher(UiTopCenterView viewPrefab, UiTopCenterPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiTopCenterWindow, UiTopCenterView>(
                viewPrefab,
                presenterFactory,
                nameof(UiTopCenterWindow),
                LayoutTagId,
                takeFocus: false,
                "A WindowSystem must exist before showing the UiTopCenter demo slice.");
        }

        public UiTopCenterWindow CurrentWindow => windowHandle.CurrentWindow;

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
                throw new ObjectDisposedException(nameof(UiTopCenterDemoLauncher));
            }
        }
    }
}

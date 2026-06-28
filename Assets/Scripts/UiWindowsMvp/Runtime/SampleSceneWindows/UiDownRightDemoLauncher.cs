using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownRightDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 14;

        private readonly UiRuntimeWindowHandle<UiDownRightWindow, UiDownRightView> windowHandle;
        private bool disposed;

        public UiDownRightDemoLauncher(UiDownRightView viewPrefab, UiDownRightPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiDownRightWindow, UiDownRightView>(
                viewPrefab,
                presenterFactory,
                nameof(UiDownRightWindow),
                LayoutTagId,
                takeFocus: false,
                "A WindowSystem must exist before showing the UiDownRight demo slice.");
        }

        public UiDownRightWindow CurrentWindow => windowHandle.CurrentWindow;

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
                throw new ObjectDisposedException(nameof(UiDownRightDemoLauncher));
            }
        }
    }
}

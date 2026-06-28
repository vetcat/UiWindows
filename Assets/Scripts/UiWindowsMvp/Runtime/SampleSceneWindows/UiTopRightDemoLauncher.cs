using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopRightDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 13;

        private readonly UiRuntimeWindowHandle<UiTopRightWindow, UiTopRightView> windowHandle;
        private bool disposed;

        public UiTopRightDemoLauncher(UiTopRightView viewPrefab, UiTopRightPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiTopRightWindow, UiTopRightView>(
                viewPrefab,
                presenterFactory,
                nameof(UiTopRightWindow),
                LayoutTagId,
                takeFocus: false,
                "A WindowSystem must exist before showing the UiTopRight demo slice.");
        }

        public UiTopRightWindow CurrentWindow => windowHandle.CurrentWindow;

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
                throw new ObjectDisposedException(nameof(UiTopRightDemoLauncher));
            }
        }
    }
}

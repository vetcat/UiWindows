using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiObjectIndicatorDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 22;

        private readonly UiRuntimeWindowHandle<UiObjectIndicatorWindow, UiObjectIndicatorView> windowHandle;
        private bool disposed;

        public UiObjectIndicatorDemoLauncher(
            UiObjectIndicatorView viewPrefab,
            UiObjectIndicatorPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiObjectIndicatorWindow, UiObjectIndicatorView>(
                viewPrefab,
                presenterFactory,
                nameof(UiObjectIndicatorWindow),
                LayoutTagId,
                takeFocus: false,
                "A WindowSystem must exist before showing the UiObjectIndicator demo slice.");
        }

        public UiObjectIndicatorWindow CurrentWindow => windowHandle.CurrentWindow;

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
                throw new ObjectDisposedException(nameof(UiObjectIndicatorDemoLauncher));
            }
        }
    }
}

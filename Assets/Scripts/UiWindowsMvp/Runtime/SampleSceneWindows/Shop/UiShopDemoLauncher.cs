using System;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiShopDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 2;

        private readonly UiRuntimeWindowHandle<UiShopWindow, UiShopView> windowHandle;
        private bool disposed;

        public UiShopDemoLauncher(UiShopView viewPrefab, UiShopPresenterFactory presenterFactory)
        {
            windowHandle = new UiRuntimeWindowHandle<UiShopWindow, UiShopView>(
                viewPrefab,
                presenterFactory,
                nameof(UiShopWindow),
                LayoutTagId,
                takeFocus: true,
                "A WindowSystem must exist before showing the UiShop demo slice.");
        }

        public UiShopWindow CurrentWindow => windowHandle.CurrentWindow;

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
                throw new ObjectDisposedException(nameof(UiShopDemoLauncher));
            }
        }
    }
}

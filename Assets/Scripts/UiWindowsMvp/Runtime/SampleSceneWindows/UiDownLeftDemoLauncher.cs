using System;
using R3;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownLeftDemoLauncher : IDisposable
    {
        private const int LayoutTagId = 15;

        private readonly UiRuntimeWindowHandle<UiDownLeftWindow, UiDownLeftView> windowHandle;
        private readonly IUiShopVisibilityReadModel shopVisibility;

        private IDisposable shopVisibilitySubscription;
        private bool wantsVisible;
        private bool disposed;

        public UiDownLeftDemoLauncher(
            UiDownLeftView viewPrefab,
            UiDownLeftPresenterFactory presenterFactory,
            IUiShopVisibilityReadModel shopVisibility)
        {
            windowHandle = new UiRuntimeWindowHandle<UiDownLeftWindow, UiDownLeftView>(
                viewPrefab,
                presenterFactory,
                nameof(UiDownLeftWindow),
                LayoutTagId,
                takeFocus: false,
                "A WindowSystem must exist before showing the UiDownLeft demo slice.");
            this.shopVisibility = shopVisibility ?? throw new ArgumentNullException(nameof(shopVisibility));
        }

        public UiDownLeftWindow CurrentWindow => windowHandle.CurrentWindow;

        public void Start()
        {
            ThrowIfDisposed();
            if (shopVisibilitySubscription != null)
            {
                return;
            }

            shopVisibilitySubscription = shopVisibility.IsShopVisible.Subscribe(HandleShopVisibilityChanged);
        }

        public void Show()
        {
            ThrowIfDisposed();
            wantsVisible = true;

            if (shopVisibility.IsShopVisible.CurrentValue)
            {
                HideWindow();
                return;
            }

            ShowWindow();
        }

        public void Hide()
        {
            wantsVisible = false;
            HideWindow();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            shopVisibilitySubscription?.Dispose();
            shopVisibilitySubscription = null;
            windowHandle.Dispose();
        }

        private void HandleShopVisibilityChanged(bool isShopVisible)
        {
            if (disposed)
            {
                return;
            }

            if (isShopVisible)
            {
                HideWindow();
                return;
            }

            if (wantsVisible)
            {
                ShowWindow();
            }
        }

        private void ShowWindow()
        {
            windowHandle.Show();
        }

        private void HideWindow()
        {
            windowHandle.Hide();
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiDownLeftDemoLauncher));
            }
        }
    }
}

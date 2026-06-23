using System;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownLeftDemoLauncher : IDisposable
    {
        private readonly UiDownLeftView viewPrefab;
        private readonly UiDownLeftPresenterFactory presenterFactory;
        private readonly IUiShopVisibilityReadModel shopVisibility;

        private UiDownLeftRuntimeWindowSource runtimeSource;
        private UiDownLeftWindow currentWindow;
        private IDisposable shopVisibilitySubscription;
        private bool wantsVisible;
        private bool disposed;

        public UiDownLeftDemoLauncher(
            UiDownLeftView viewPrefab,
            UiDownLeftPresenterFactory presenterFactory,
            IUiShopVisibilityReadModel shopVisibility)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
            this.shopVisibility = shopVisibility ?? throw new ArgumentNullException(nameof(shopVisibility));
        }

        public UiDownLeftWindow CurrentWindow => currentWindow;

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
            HideWindow();
            currentWindow = null;
            runtimeSource?.Dispose();
            runtimeSource = null;
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
            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException(
                    "A WindowSystem must exist before showing the UiDownLeft demo slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiDownLeftRuntimeWindowSource.Create(viewPrefab);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiDownLeftWindow;
        }

        private void HideWindow()
        {
            if (currentWindow == null || WindowSystem.HasInstance() == false ||
                currentWindow.GetState() >= ObjectState.Hiding)
            {
                return;
            }

            currentWindow.Hide(TransitionParameters.Default.ReplaceImmediately(true));
        }

        private void BindPresenter(WindowBase window)
        {
            if (window is not UiDownLeftWindow uiDownLeftWindow)
            {
                throw new InvalidOperationException(
                    $"Expected {nameof(UiDownLeftWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = uiDownLeftWindow;
            if (WindowPresenterBinder.TryGetBinding(uiDownLeftWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(uiDownLeftWindow, presenterFactory);
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

using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiShopDemoLauncher : IDisposable
    {
        private readonly UiShopView viewPrefab;
        private readonly UiShopPresenterFactory presenterFactory;

        private UiShopRuntimeWindowSource runtimeSource;
        private UiShopWindow currentWindow;
        private bool disposed;

        public UiShopDemoLauncher(UiShopView viewPrefab, UiShopPresenterFactory presenterFactory)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public UiShopWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();

            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException("A WindowSystem must exist before showing the UiShop demo slice.");
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiShopRuntimeWindowSource.Create(viewPrefab);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as UiShopWindow;
        }

        public void Hide()
        {
            if (currentWindow == null || WindowSystem.HasInstance() == false ||
                currentWindow.GetState() >= ObjectState.Hiding)
            {
                return;
            }

            currentWindow.Hide(TransitionParameters.Default.ReplaceImmediately(true));
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Hide();
            currentWindow = null;
            runtimeSource?.Dispose();
            runtimeSource = null;
        }

        private void BindPresenter(WindowBase window)
        {
            if (window is not UiShopWindow uiShopWindow)
            {
                throw new InvalidOperationException($"Expected {nameof(UiShopWindow)}, got {window.GetType().Name}.");
            }

            currentWindow = uiShopWindow;
            if (WindowPresenterBinder.TryGetBinding(uiShopWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(uiShopWindow, presenterFactory);
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
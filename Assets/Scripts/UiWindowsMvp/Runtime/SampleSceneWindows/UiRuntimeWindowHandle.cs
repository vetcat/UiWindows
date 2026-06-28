using System;
using UiWindowsMvp.UIAdapter;
using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.WindowTypes;

namespace UiWindowsMvp.SampleSceneWindows
{
    internal sealed class UiRuntimeWindowHandle<TWindow, TView> : IDisposable
        where TWindow : LayoutWindowType
        where TView : WindowComponent
    {
        private readonly TView viewPrefab;
        private readonly IWindowPresenterFactory<TWindow> presenterFactory;
        private readonly string windowName;
        private readonly int layoutTagId;
        private readonly bool takeFocus;
        private readonly string missingWindowSystemMessage;

        private UiRuntimeWindowSource<TWindow, TView> runtimeSource;
        private TWindow currentWindow;
        private bool disposed;

        public UiRuntimeWindowHandle(
            TView viewPrefab,
            IWindowPresenterFactory<TWindow> presenterFactory,
            string windowName,
            int layoutTagId,
            bool takeFocus,
            string missingWindowSystemMessage)
        {
            this.viewPrefab = viewPrefab != null ? viewPrefab : throw new ArgumentNullException(nameof(viewPrefab));
            this.presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
            this.windowName = string.IsNullOrWhiteSpace(windowName)
                ? throw new ArgumentException("Window name must be provided.", nameof(windowName))
                : windowName;
            this.layoutTagId = layoutTagId;
            this.takeFocus = takeFocus;
            this.missingWindowSystemMessage = string.IsNullOrWhiteSpace(missingWindowSystemMessage)
                ? "A WindowSystem must exist before showing a runtime UI window."
                : missingWindowSystemMessage;
        }

        public TWindow CurrentWindow => currentWindow;

        public void Show()
        {
            ThrowIfDisposed();

            if (WindowSystem.HasInstance() == false)
            {
                throw new InvalidOperationException(missingWindowSystemMessage);
            }

            if (currentWindow != null && currentWindow.GetState() < ObjectState.Hiding)
            {
                return;
            }

            runtimeSource ??= UiRuntimeWindowSource<TWindow, TView>.Create(
                viewPrefab,
                windowName,
                layoutTagId,
                takeFocus);
            var initialParameters = new InitialParameters
            {
                showSync = true
            };

            currentWindow = WindowSystem.Show(
                runtimeSource.WindowSource,
                initialParameters,
                BindPresenter,
                TransitionParameters.Default.ReplaceImmediately(true)).screen as TWindow;
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
            if (window is not TWindow typedWindow)
            {
                throw new InvalidOperationException(
                    $"Expected {typeof(TWindow).Name}, got {GetActualWindowName(window)}.");
            }

            currentWindow = typedWindow;
            if (WindowPresenterBinder.TryGetBinding(typedWindow, out _))
            {
                return;
            }

            WindowPresenterBinder.Bind(typedWindow, presenterFactory);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private static string GetActualWindowName(WindowBase window)
        {
            return window != null ? window.GetType().Name : "<null>";
        }
    }
}

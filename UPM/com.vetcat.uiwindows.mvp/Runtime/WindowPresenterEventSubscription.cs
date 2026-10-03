#nullable enable

using System;
using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.Modules;

namespace UiWindowsMvp.UIAdapter
{
    internal sealed class WindowPresenterEventSubscription<TWindow> : IDisposable
        where TWindow : WindowBase
    {
        private readonly Action<WindowObject> onInitialized;
        private readonly Action<WindowObject> onShowBegin;
        private readonly Action<WindowObject> onShowEnd;
        private readonly Action<WindowObject> onHideBegin;
        private readonly Action<WindowObject> onHideEnd;
        private readonly Action<WindowObject> onDeInitialized;
        private WindowPresenterBinding<TWindow>? binding;
        private TWindow? window;
        private WindowSystemEvents? events;
        private bool isDisposed;

        public WindowPresenterEventSubscription(WindowPresenterBinding<TWindow> binding)
        {
            this.binding = binding ?? throw new ArgumentNullException(nameof(binding));
            window = binding.Window;
            events = WindowSystem.GetEvents() ?? throw new InvalidOperationException(
                "WindowSystem events are not available. Bind presenters from a WindowSystem.Show/ShowSync callback after WindowSystem is initialized, or disable event subscription for direct tests.");

            onInitialized = OnInitialized;
            onShowBegin = OnShowBegin;
            onShowEnd = OnShowEnd;
            onHideBegin = OnHideBegin;
            onHideEnd = OnHideEnd;
            onDeInitialized = OnDeInitialized;

            Register(WindowEvent.OnInitialized, onInitialized);
            Register(WindowEvent.OnShowBegin, onShowBegin);
            Register(WindowEvent.OnShowEnd, onShowEnd);
            Register(WindowEvent.OnHideBegin, onHideBegin);
            Register(WindowEvent.OnHideEnd, onHideEnd);
            Register(WindowEvent.OnDeInitialized, onDeInitialized);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            var eventRegistry = events;
            var targetWindow = window;
            if (eventRegistry != null && targetWindow != null)
            {
                eventRegistry.UnRegister(targetWindow, WindowEvent.OnInitialized, onInitialized);
                eventRegistry.UnRegister(targetWindow, WindowEvent.OnShowBegin, onShowBegin);
                eventRegistry.UnRegister(targetWindow, WindowEvent.OnShowEnd, onShowEnd);
                eventRegistry.UnRegister(targetWindow, WindowEvent.OnHideBegin, onHideBegin);
                eventRegistry.UnRegister(targetWindow, WindowEvent.OnHideEnd, onHideEnd);
                eventRegistry.UnRegister(targetWindow, WindowEvent.OnDeInitialized, onDeInitialized);
            }

            binding = null;
            window = null;
            events = null;
        }

        private void Register(WindowEvent windowEvent, Action<WindowObject> callback)
        {
            var eventRegistry =
                events ?? throw new ObjectDisposedException(nameof(WindowPresenterEventSubscription<TWindow>));
            var targetWindow = window ??
                               throw new ObjectDisposedException(nameof(WindowPresenterEventSubscription<TWindow>));
            eventRegistry.Register(targetWindow, windowEvent, callback);
        }

        private void OnInitialized(WindowObject windowObject)
        {
            if (isDisposed)
            {
                return;
            }

            binding?.OnWindowInitialized();
        }

        private void OnShowBegin(WindowObject windowObject)
        {
            if (isDisposed)
            {
                return;
            }

            binding?.OnWindowShowBegin();
        }

        private void OnShowEnd(WindowObject windowObject)
        {
            if (isDisposed)
            {
                return;
            }

            binding?.OnWindowShowEnd();
        }

        private void OnHideBegin(WindowObject windowObject)
        {
            if (isDisposed)
            {
                return;
            }

            binding?.OnWindowHideBegin();
        }

        private void OnHideEnd(WindowObject windowObject)
        {
            if (isDisposed)
            {
                return;
            }

            binding?.OnWindowHideEnd();
        }

        private void OnDeInitialized(WindowObject windowObject)
        {
            if (isDisposed)
            {
                return;
            }

            var currentBinding = binding;
            Dispose();
            currentBinding?.OnWindowDeInitialized();
        }
    }
}
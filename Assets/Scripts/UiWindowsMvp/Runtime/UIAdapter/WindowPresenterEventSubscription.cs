using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.Modules;

namespace UiWindowsMvp.UIAdapter
{
    internal sealed class WindowPresenterEventSubscription<TWindow> : IDisposable, IEquatable<WindowPresenterEventSubscription<TWindow>>
        where TWindow : WindowBase
    {
        private readonly WindowPresenterBinding<TWindow> binding;
        private readonly TWindow window;
        private readonly WindowSystemEvents events;
        private bool isDisposed;
        private bool unregisterOnDispose = true;

        public WindowPresenterEventSubscription(WindowPresenterBinding<TWindow> binding)
        {
            this.binding = binding ?? throw new ArgumentNullException(nameof(binding));
            window = binding.Window;
            events = WindowSystem.GetEvents() ?? throw new InvalidOperationException("WindowSystem events are not available. Bind presenters from a WindowSystem.Show/ShowSync callback after WindowSystem is initialized, or disable event subscription for direct tests.");

            Register(WindowEvent.OnInitialized, OnInitialized);
            Register(WindowEvent.OnShowBegin, OnShowBegin);
            Register(WindowEvent.OnShowEnd, OnShowEnd);
            Register(WindowEvent.OnHideBegin, OnHideBegin);
            Register(WindowEvent.OnHideEnd, OnHideEnd);
            Register(WindowEvent.OnDeInitialized, OnDeInitialized);
        }

        public bool Equals(WindowPresenterEventSubscription<TWindow> other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            if (unregisterOnDispose)
            {
                Unregister(WindowEvent.OnInitialized, OnInitialized);
                Unregister(WindowEvent.OnShowBegin, OnShowBegin);
                Unregister(WindowEvent.OnShowEnd, OnShowEnd);
                Unregister(WindowEvent.OnHideBegin, OnHideBegin);
                Unregister(WindowEvent.OnHideEnd, OnHideEnd);
                Unregister(WindowEvent.OnDeInitialized, OnDeInitialized);
            }
        }

        private void Register(WindowEvent windowEvent, Action<WindowObject, WindowPresenterEventSubscription<TWindow>> callback)
        {
            events.Register(this, window, windowEvent, callback);
        }

        private void Unregister(WindowEvent windowEvent, Action<WindowObject, WindowPresenterEventSubscription<TWindow>> callback)
        {
            events.UnRegister(this, window, windowEvent, callback);
        }

        private static void OnInitialized(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            subscription.binding.OnWindowInitialized();
        }

        private static void OnShowBegin(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            subscription.binding.OnWindowShowBegin();
        }

        private static void OnShowEnd(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            subscription.binding.OnWindowShowEnd();
        }

        private static void OnHideBegin(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            subscription.binding.OnWindowHideBegin();
        }

        private static void OnHideEnd(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            subscription.binding.OnWindowHideEnd();
        }

        private static void OnDeInitialized(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            subscription.unregisterOnDispose = false;
            subscription.Dispose();
            subscription.binding.OnWindowDeInitialized();
        }
    }
}

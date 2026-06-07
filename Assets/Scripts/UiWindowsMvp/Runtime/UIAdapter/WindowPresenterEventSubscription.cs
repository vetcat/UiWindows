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
            isDisposed = true;
        }

        private void Register(WindowEvent windowEvent, Action<WindowObject, WindowPresenterEventSubscription<TWindow>> callback)
        {
            events.Register(this, window, windowEvent, callback);
        }

        private static void OnInitialized(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            if (subscription.isDisposed)
            {
                return;
            }

            subscription.binding.OnWindowInitialized();
        }

        private static void OnShowBegin(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            if (subscription.isDisposed)
            {
                return;
            }

            subscription.binding.OnWindowShowBegin();
        }

        private static void OnShowEnd(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            if (subscription.isDisposed)
            {
                return;
            }

            subscription.binding.OnWindowShowEnd();
        }

        private static void OnHideBegin(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            if (subscription.isDisposed)
            {
                return;
            }

            subscription.binding.OnWindowHideBegin();
        }

        private static void OnHideEnd(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            if (subscription.isDisposed)
            {
                return;
            }

            subscription.binding.OnWindowHideEnd();
        }

        private static void OnDeInitialized(WindowObject windowObject, WindowPresenterEventSubscription<TWindow> subscription)
        {
            if (subscription.isDisposed)
            {
                return;
            }

            subscription.Dispose();
            subscription.binding.OnWindowDeInitialized();
        }
    }
}

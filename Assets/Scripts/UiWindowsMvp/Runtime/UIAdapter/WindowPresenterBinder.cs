using System;
using UnityEngine;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.UIAdapter
{
    public static class WindowPresenterBinder
    {
        public static WindowPresenterBinding<TWindow> Bind<TWindow>(
            TWindow window,
            IWindowPresenterFactory<TWindow> factory,
            bool subscribeToWindowSystemEvents = true)
            where TWindow : WindowBase
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return Bind(window, factory.Create(window), subscribeToWindowSystemEvents);
        }

        public static WindowPresenterBinding<TWindow> Bind<TWindow>(
            TWindow window,
            IWindowPresenter<TWindow> presenter,
            bool subscribeToWindowSystemEvents = true)
            where TWindow : WindowBase
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            var anchor = GetOrAddAnchor(window);
            if (anchor.HasBinding)
            {
                throw new InvalidOperationException($"Window '{window.name}' already has an active presenter binding.");
            }

            var binding = new WindowPresenterBinding<TWindow>(window, presenter);
            try
            {
                anchor.SetBinding(binding);
                binding.AttachAnchor(anchor);

                if (subscribeToWindowSystemEvents)
                {
                    binding.AttachLifecycleSubscription(new WindowPresenterEventSubscription<TWindow>(binding));
                }

                CatchUpLifecycle(window, binding);
                return binding;
            }
            catch
            {
                binding.Dispose();
                throw;
            }
        }

        public static bool TryGetBinding<TWindow>(TWindow window, out IWindowPresenterBinding<TWindow> binding)
            where TWindow : WindowBase
        {
            binding = null;
            if (window == null)
            {
                return false;
            }

            var anchor = window.GetComponent<WindowPresenterBindingAnchor>();
            if (anchor == null || anchor.HasBinding == false)
            {
                return false;
            }

            binding = anchor.Binding as IWindowPresenterBinding<TWindow>;
            return binding != null;
        }

        private static WindowPresenterBindingAnchor GetOrAddAnchor(WindowBase window)
        {
            var anchor = window.GetComponent<WindowPresenterBindingAnchor>();
            if (anchor != null)
            {
                return anchor;
            }

            return window.gameObject.AddComponent<WindowPresenterBindingAnchor>();
        }

        private static void CatchUpLifecycle<TWindow>(TWindow window, WindowPresenterBinding<TWindow> binding)
            where TWindow : WindowBase
        {
            switch (window.GetState())
            {
                case ObjectState.Initialized:
                case ObjectState.Hidden:
                    binding.OnWindowInitialized();
                    break;
                case ObjectState.Showing:
                    binding.OnWindowShowBegin();
                    break;
                case ObjectState.Shown:
                    binding.OnWindowShowBegin();
                    binding.OnWindowShowEnd();
                    break;
                case ObjectState.Hiding:
                    binding.OnWindowShowBegin();
                    binding.OnWindowHideBegin();
                    break;
                case ObjectState.DeInitializing:
                case ObjectState.DeInitialized:
                    throw new InvalidOperationException($"Cannot bind presenter to window '{window.name}' in state {window.GetState()}.");
            }
        }
    }
}

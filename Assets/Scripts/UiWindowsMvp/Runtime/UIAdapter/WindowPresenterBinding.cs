using System;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.UIAdapter
{
    public sealed class WindowPresenterBinding<TWindow> : IWindowPresenterBinding<TWindow>
        where TWindow : WindowBase
    {
        private readonly IWindowPresenter<TWindow> presenter;
        private WindowPresenterShowScope showScope;
        private IDisposable lifecycleSubscription;
        private WindowPresenterBindingAnchor anchor;
        private bool isInitialized;

        public WindowPresenterBinding(TWindow window, IWindowPresenter<TWindow> presenter)
        {
            Window = window != null ? window : throw new ArgumentNullException(nameof(window));
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.presenter.Bind(window);
        }

        public TWindow Window { get; }

        WindowBase IWindowPresenterBinding.Window => Window;

        public IUiPresenter Presenter => presenter;

        public bool IsDisposed { get; private set; }

        public void OnWindowInitialized()
        {
            ThrowIfDisposed();
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            presenter.Initialize();
        }

        public void OnWindowShowBegin()
        {
            ThrowIfDisposed();
            OnWindowInitialized();
            DisposeShowScope();
            showScope = new WindowPresenterShowScope();

            try
            {
                presenter.OnShowBegin(showScope);
            }
            catch
            {
                DisposeShowScope();
                throw;
            }
        }

        public void OnWindowShowEnd()
        {
            ThrowIfDisposed();
            presenter.OnShowEnd();
        }

        public void OnWindowHideBegin()
        {
            ThrowIfDisposed();
            presenter.OnHideBegin();
        }

        public void OnWindowHideEnd()
        {
            if (IsDisposed)
            {
                return;
            }

            presenter.OnHideEnd();
            DisposeShowScope();
        }

        public void OnWindowDeInitialized()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            var subscription = lifecycleSubscription;
            lifecycleSubscription = null;
            subscription?.Dispose();

            DisposeShowScope();
            presenter.Dispose();

            var owner = anchor;
            anchor = null;
            owner?.ClearBinding(this);
        }

        internal void AttachAnchor(WindowPresenterBindingAnchor bindingAnchor)
        {
            anchor = bindingAnchor ?? throw new ArgumentNullException(nameof(bindingAnchor));
        }

        internal void AttachLifecycleSubscription(IDisposable subscription)
        {
            lifecycleSubscription = subscription ?? throw new ArgumentNullException(nameof(subscription));
        }

        private void DisposeShowScope()
        {
            var scope = showScope;
            showScope = null;
            scope?.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(WindowPresenterBinding<TWindow>));
            }
        }
    }
}

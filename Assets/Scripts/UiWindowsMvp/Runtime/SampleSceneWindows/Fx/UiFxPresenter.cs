using System;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiFxPresenter : IWindowPresenter<UiFxWindow>
    {
        private readonly IUiFeedbackReadModel readModel;
        private readonly Func<UiFxWindow, UiFxView> viewResolver;

        private UiFxWindow window;
        private UiFxView view;
        private bool disposed;

        public UiFxPresenter(IUiFeedbackReadModel readModel)
            : this(readModel, ResolveView)
        {
        }

        internal UiFxPresenter(IUiFeedbackReadModel readModel, Func<UiFxWindow, UiFxView> viewResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiFxWindow window)
        {
            ThrowIfDisposed();
            this.window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void Initialize()
        {
            ThrowIfDisposed();
            view = ResolveRequiredView();
            view.EnsureLayout();
        }

        public void OnShowBegin(IUiShowScope showScope)
        {
            ThrowIfDisposed();
            if (showScope == null)
            {
                throw new ArgumentNullException(nameof(showScope));
            }

            view = ResolveRequiredView();
            view.EnsureLayout();
            showScope.Add(readModel.FxRequests.Subscribe(view.PlayFx));
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
            view?.StopAllFx();
        }

        public void OnHideEnd()
        {
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            view?.StopAllFx();
            view = null;
            window = null;
        }

        private UiFxView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiFxView was not loaded by the UiFxWindow layout.");
            }

            return resolved;
        }

        private static UiFxView ResolveView(UiFxWindow window)
        {
            if (window == null)
            {
                return null;
            }

            return window.TryGetView(out var resolved) ? resolved : null;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiFxPresenter));
            }
        }
    }
}
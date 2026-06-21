using System;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiHintsPresenter : IWindowPresenter<UiHintsWindow>
    {
        private readonly IUiFeedbackReadModel readModel;
        private readonly Func<UiHintsWindow, UiHintsView> viewResolver;

        private UiHintsWindow window;
        private UiHintsView view;
        private bool disposed;

        public UiHintsPresenter(IUiFeedbackReadModel readModel)
            : this(readModel, ResolveView)
        {
        }

        internal UiHintsPresenter(
            IUiFeedbackReadModel readModel,
            Func<UiHintsWindow, UiHintsView> viewResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiHintsWindow window)
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
            showScope.Add(readModel.HintRequests.Subscribe(view.ShowHint));
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
            view?.ClearHint();
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
            view?.ClearHint();
            view = null;
            window = null;
        }

        private UiHintsView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiHintsView was not loaded by the UiHintsWindow layout.");
            }

            return resolved;
        }

        private static UiHintsView ResolveView(UiHintsWindow window)
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
                throw new ObjectDisposedException(nameof(UiHintsPresenter));
            }
        }
    }
}
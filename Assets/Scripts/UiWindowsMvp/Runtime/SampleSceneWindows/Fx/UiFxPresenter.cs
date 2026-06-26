using System;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiFxPresenter : IWindowPresenter<UiFxWindow>
    {
        private readonly IUiFeedbackReadModel readModel;
        private readonly IUiFxTargetResolver targetResolver;
        private readonly Func<UiFxWindow, UiFxView> viewResolver;

        private UiFxWindow window;
        private UiFxView view;
        private bool disposed;

        public UiFxPresenter(IUiFeedbackReadModel readModel)
            : this(readModel, null, ResolveView)
        {
        }

        public UiFxPresenter(IUiFeedbackReadModel readModel, IUiFxTargetResolver targetResolver)
            : this(readModel, targetResolver, ResolveView)
        {
        }

        internal UiFxPresenter(IUiFeedbackReadModel readModel, Func<UiFxWindow, UiFxView> viewResolver)
            : this(readModel, null, viewResolver)
        {
        }

        internal UiFxPresenter(
            IUiFeedbackReadModel readModel,
            IUiFxTargetResolver targetResolver,
            Func<UiFxWindow, UiFxView> viewResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.targetResolver = targetResolver;
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
            showScope.Add(readModel.FxRequests.Subscribe(PlayFx));
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

        private void PlayFx(UiFxRequest request)
        {
            view.PlayFx(request, ResolveTarget(request.Source), ResolveTarget(request.Target));
        }

        private RectTransform ResolveTarget(UiFxTarget? target)
        {
            return target.HasValue && targetResolver != null &&
                   targetResolver.TryGetTarget(target.Value, out var rectTransform)
                ? rectTransform
                : null;
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

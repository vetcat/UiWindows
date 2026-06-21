using System;
using ProjectContext.Player;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopRightPresenter : IWindowPresenter<UiTopRightWindow>
    {
        private readonly IPlayerReadModel readModel;
        private readonly IUiFxTargetRegistry fxTargetRegistry;
        private readonly Func<UiTopRightWindow, UiTopRightView> viewResolver;

        private UiTopRightWindow window;
        private UiTopRightView view;
        private bool disposed;

        public UiTopRightPresenter(IPlayerReadModel readModel, IUiFxTargetRegistry fxTargetRegistry)
            : this(readModel, fxTargetRegistry, ResolveView)
        {
        }

        internal UiTopRightPresenter(
            IPlayerReadModel readModel,
            IUiFxTargetRegistry fxTargetRegistry,
            Func<UiTopRightWindow, UiTopRightView> viewResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.fxTargetRegistry = fxTargetRegistry ?? throw new ArgumentNullException(nameof(fxTargetRegistry));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiTopRightWindow window)
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
            showScope.Add(readModel.Coins.Subscribe(view.SetCoins));

            if (view.CoinIconRectTransform != null)
            {
                showScope.Add(fxTargetRegistry.RegisterTarget(UiFxTarget.Coins, view.CoinIconRectTransform));
            }
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
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
            view = null;
            window = null;
        }

        private UiTopRightView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiTopRightView was not loaded by the UiTopRightWindow layout.");
            }

            return resolved;
        }

        private static UiTopRightView ResolveView(UiTopRightWindow window)
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
                throw new ObjectDisposedException(nameof(UiTopRightPresenter));
            }
        }
    }
}

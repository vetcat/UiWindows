using System;
using ProjectContext.Player;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine.Events;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiObjectIndicatorPresenter : IWindowPresenter<UiObjectIndicatorWindow>
    {
        private readonly IPlayerReadModel readModel;
        private readonly IPlayerCommands playerCommands;
        private readonly IPlayerSettings playerSettings;
        private readonly IUiObjectIndicatorAnchor indicatorAnchor;
        private readonly IUiWorldToScreenAdapter screenAdapter;
        private readonly IUiFxTargetRegistry fxTargetRegistry;
        private readonly Func<UiObjectIndicatorWindow, UiObjectIndicatorView> viewResolver;

        private UiObjectIndicatorWindow window;
        private UiObjectIndicatorView view;
        private bool isShowActive;
        private bool disposed;

        public UiObjectIndicatorPresenter(
            IPlayerReadModel readModel,
            IPlayerCommands playerCommands,
            IPlayerSettings playerSettings,
            IUiObjectIndicatorAnchor indicatorAnchor,
            IUiWorldToScreenAdapter screenAdapter,
            IUiFxTargetRegistry fxTargetRegistry)
            : this(
                readModel,
                playerCommands,
                playerSettings,
                indicatorAnchor,
                screenAdapter,
                fxTargetRegistry,
                ResolveView)
        {
        }

        internal UiObjectIndicatorPresenter(
            IPlayerReadModel readModel,
            IPlayerCommands playerCommands,
            IPlayerSettings playerSettings,
            IUiObjectIndicatorAnchor indicatorAnchor,
            IUiWorldToScreenAdapter screenAdapter,
            IUiFxTargetRegistry fxTargetRegistry,
            Func<UiObjectIndicatorWindow, UiObjectIndicatorView> viewResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.playerCommands = playerCommands ?? throw new ArgumentNullException(nameof(playerCommands));
            this.playerSettings = playerSettings ?? throw new ArgumentNullException(nameof(playerSettings));
            this.indicatorAnchor = indicatorAnchor ?? throw new ArgumentNullException(nameof(indicatorAnchor));
            this.screenAdapter = screenAdapter ?? throw new ArgumentNullException(nameof(screenAdapter));
            this.fxTargetRegistry = fxTargetRegistry ?? throw new ArgumentNullException(nameof(fxTargetRegistry));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiObjectIndicatorWindow window)
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
            view.SetRewardAmount(indicatorAnchor.RewardAmount);
            view.SetInfo(indicatorAnchor.DisplayName);

            showScope.Add(readModel.Health.Subscribe(value => view.SetHealth(value, playerSettings.MaxHealth)));
            AddButtonListener(showScope, view.ButtonAction, RewardFromIndicator);

            if (view.RewardAnchor != null)
            {
                showScope.Add(fxTargetRegistry.RegisterTarget(UiFxTarget.CharacterReward, view.RewardAnchor));
            }

            isShowActive = true;
            showScope.Add(Observable.EveryUpdate().Subscribe(_ => UpdateIndicatorPosition()));
            UpdateIndicatorPosition();
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
            isShowActive = false;
            view?.SetIndicatorVisible(false);
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
            isShowActive = false;
            view?.SetIndicatorVisible(false);
            view = null;
            window = null;
        }

        internal bool UpdateIndicatorPosition()
        {
            if (isShowActive == false ||
                view == null ||
                indicatorAnchor.TryGetWorldPosition(out var worldPosition) == false ||
                screenAdapter.TryGetScreenPosition(worldPosition, out var screenPosition) == false)
            {
                view?.SetIndicatorVisible(false);
                return false;
            }

            view.SetInfo(indicatorAnchor.DisplayName);
            view.SetIndicatorScreenPosition(screenPosition);
            view.SetIndicatorVisible(true);
            return true;
        }

        private void RewardFromIndicator()
        {
            if (isShowActive == false)
            {
                return;
            }

            playerCommands.AddCoinsWithFxFrom(
                indicatorAnchor.RewardAmount,
                UiFxTarget.CharacterReward,
                UiFxTarget.Coins);
        }

        private UiObjectIndicatorView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException(
                    "UiObjectIndicatorView was not loaded by the UiObjectIndicatorWindow layout.");
            }

            return resolved;
        }

        private static UiObjectIndicatorView ResolveView(UiObjectIndicatorWindow window)
        {
            if (window == null)
            {
                return null;
            }

            return window.TryGetView(out var resolved) ? resolved : null;
        }

        private static void AddButtonListener(IUiShowScope showScope, UnityEngine.UI.Button button, UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.AddListener(action);
            showScope.Add(new DisposableAction(() => button.onClick.RemoveListener(action)));
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiObjectIndicatorPresenter));
            }
        }
    }
}

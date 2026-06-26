using System;
using System.Globalization;
using ProjectContext.Localization;
using ProjectContext.UiRequests;
using R3;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopCenterPresenter : IWindowPresenter<UiTopCenterWindow>
    {
        public static readonly TimeSpan DefaultHoldDelay = TimeSpan.FromSeconds(0.5d);

        internal const string TimeTemplateKey = "TimeTemplate";
        internal const string HintDescriptionKey = "HintDescription";
        internal const string PressAndHoldKey = "PressAndHold";

        private readonly IUiTopCenterTimeProvider timeProvider;
        private readonly ILocalizationReadModel localizationReadModel;
        private readonly IUiFeedbackCommands feedbackCommands;
        private readonly IUiTopCenterHoldTimer holdTimer;
        private readonly Func<UiTopCenterWindow, UiTopCenterView> viewResolver;

        private UiTopCenterWindow window;
        private UiTopCenterView view;
        private IDisposable activeHold;
        private bool isPointerDown;
        private bool hintRequestedForCurrentHold;
        private bool acceptsHoldInput;
        private bool disposed;

        public UiTopCenterPresenter(
            IUiTopCenterTimeProvider timeProvider,
            ILocalizationReadModel localizationReadModel,
            IUiFeedbackCommands feedbackCommands,
            IUiTopCenterHoldTimer holdTimer)
            : this(timeProvider, localizationReadModel, feedbackCommands, holdTimer, ResolveView)
        {
        }

        internal UiTopCenterPresenter(
            IUiTopCenterTimeProvider timeProvider,
            ILocalizationReadModel localizationReadModel,
            IUiFeedbackCommands feedbackCommands,
            IUiTopCenterHoldTimer holdTimer,
            Func<UiTopCenterWindow, UiTopCenterView> viewResolver)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
            this.feedbackCommands = feedbackCommands ?? throw new ArgumentNullException(nameof(feedbackCommands));
            this.holdTimer = holdTimer ?? throw new ArgumentNullException(nameof(holdTimer));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiTopCenterWindow window)
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
            acceptsHoldInput = true;
            RefreshLocalizedText();

            showScope.Add(timeProvider.SubscribeUtcTimeChanged(RefreshTimeText));
            showScope.Add(localizationReadModel.CurrentLanguage.Subscribe(_ => RefreshLocalizedText()));
            AddHoldInputListeners(showScope, view.HoldInput);
            showScope.Add(new DisposableAction(CancelHold));
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
            acceptsHoldInput = false;
            CancelHold();
        }

        public void OnHideEnd()
        {
            acceptsHoldInput = false;
            CancelHold();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            acceptsHoldInput = false;
            CancelHold();
            view = null;
            window = null;
        }

        private void RefreshLocalizedText()
        {
            if (view == null)
            {
                return;
            }

            view.SetPressAndHoldText(localizationReadModel.Translate(PressAndHoldKey));
            RefreshTimeText();
        }

        private void RefreshTimeText()
        {
            if (view == null)
            {
                return;
            }

            var time = timeProvider.UtcNow.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            view.SetTimeText(localizationReadModel.Translate(TimeTemplateKey, time));
        }

        private void AddHoldInputListeners(IUiShowScope showScope, UiTopCenterHoldInput holdInput)
        {
            if (holdInput == null)
            {
                return;
            }

            holdInput.PointerDown += StartHold;
            holdInput.PointerUp += CancelHold;
            holdInput.PointerExit += CancelHold;
            showScope.Add(new DisposableAction(() =>
            {
                holdInput.PointerDown -= StartHold;
                holdInput.PointerUp -= CancelHold;
                holdInput.PointerExit -= CancelHold;
            }));
        }

        private void StartHold()
        {
            if (!acceptsHoldInput || isPointerDown)
            {
                return;
            }

            isPointerDown = true;
            hintRequestedForCurrentHold = false;
            activeHold?.Dispose();
            activeHold = holdTimer.Start(DefaultHoldDelay, CompleteHold);
        }

        private void CancelHold()
        {
            activeHold?.Dispose();
            activeHold = null;
            isPointerDown = false;
            hintRequestedForCurrentHold = false;
        }

        private void CompleteHold()
        {
            activeHold?.Dispose();
            activeHold = null;

            if (disposed || !acceptsHoldInput || !isPointerDown || hintRequestedForCurrentHold)
            {
                return;
            }

            hintRequestedForCurrentHold = true;
            feedbackCommands.ShowHint(
                localizationReadModel.Translate(HintDescriptionKey),
                UiHintAnchor.Top);
        }

        private UiTopCenterView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiTopCenterView was not loaded by the UiTopCenterWindow layout.");
            }

            return resolved;
        }

        private static UiTopCenterView ResolveView(UiTopCenterWindow window)
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
                throw new ObjectDisposedException(nameof(UiTopCenterPresenter));
            }
        }
    }
}

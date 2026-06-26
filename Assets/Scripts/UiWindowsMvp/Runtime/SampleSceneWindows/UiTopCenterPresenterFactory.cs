using System;
using ProjectContext.Localization;
using ProjectContext.UiRequests;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopCenterPresenterFactory : IWindowPresenterFactory<UiTopCenterWindow>
    {
        private readonly IUiTopCenterTimeProvider timeProvider;
        private readonly ILocalizationReadModel localizationReadModel;
        private readonly IUiFeedbackCommands feedbackCommands;
        private readonly IUiTopCenterHoldTimer holdTimer;

        public UiTopCenterPresenterFactory(
            IUiTopCenterTimeProvider timeProvider,
            ILocalizationReadModel localizationReadModel,
            IUiFeedbackCommands feedbackCommands,
            IUiTopCenterHoldTimer holdTimer)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
            this.feedbackCommands = feedbackCommands ?? throw new ArgumentNullException(nameof(feedbackCommands));
            this.holdTimer = holdTimer ?? throw new ArgumentNullException(nameof(holdTimer));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiTopCenterWindow> Create(UiTopCenterWindow window)
        {
            CreatedCount++;
            return new UiTopCenterPresenter(timeProvider, localizationReadModel, feedbackCommands, holdTimer);
        }
    }
}

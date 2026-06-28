using System;
using ProjectContext.Player;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiObjectIndicatorPresenterFactory : IWindowPresenterFactory<UiObjectIndicatorWindow>
    {
        private readonly IPlayerReadModel readModel;
        private readonly IPlayerCommands playerCommands;
        private readonly IPlayerSettings playerSettings;
        private readonly IUiObjectIndicatorAnchor indicatorAnchor;
        private readonly IUiWorldToScreenAdapter screenAdapter;
        private readonly IUiFxTargetRegistry fxTargetRegistry;
        private readonly IUiObjectIndicatorUpdateSource updateSource;

        public UiObjectIndicatorPresenterFactory(
            IPlayerReadModel readModel,
            IPlayerCommands playerCommands,
            IPlayerSettings playerSettings,
            IUiObjectIndicatorAnchor indicatorAnchor,
            IUiWorldToScreenAdapter screenAdapter,
            IUiFxTargetRegistry fxTargetRegistry,
            IUiObjectIndicatorUpdateSource updateSource)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.playerCommands = playerCommands ?? throw new ArgumentNullException(nameof(playerCommands));
            this.playerSettings = playerSettings ?? throw new ArgumentNullException(nameof(playerSettings));
            this.indicatorAnchor = indicatorAnchor ?? throw new ArgumentNullException(nameof(indicatorAnchor));
            this.screenAdapter = screenAdapter ?? throw new ArgumentNullException(nameof(screenAdapter));
            this.fxTargetRegistry = fxTargetRegistry ?? throw new ArgumentNullException(nameof(fxTargetRegistry));
            this.updateSource = updateSource ?? throw new ArgumentNullException(nameof(updateSource));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiObjectIndicatorWindow> Create(UiObjectIndicatorWindow window)
        {
            CreatedCount++;
            return new UiObjectIndicatorPresenter(
                readModel,
                playerCommands,
                playerSettings,
                indicatorAnchor,
                screenAdapter,
                fxTargetRegistry,
                updateSource);
        }
    }
}

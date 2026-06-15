using System;
using ProjectContext.Localization;
using ProjectContext.Settings;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiSettingsPresenterFactory : IWindowPresenterFactory<UiSettingsWindow>
    {
        private readonly IGameSettingsReadModel settingsReadModel;
        private readonly IGameSettingsCommands settingsCommands;
        private readonly ILocalizationReadModel localizationReadModel;
        private readonly ILocalizationCommands localizationCommands;

        public UiSettingsPresenterFactory(
            IGameSettingsReadModel settingsReadModel,
            IGameSettingsCommands settingsCommands,
            ILocalizationReadModel localizationReadModel,
            ILocalizationCommands localizationCommands)
        {
            this.settingsReadModel = settingsReadModel ?? throw new ArgumentNullException(nameof(settingsReadModel));
            this.settingsCommands = settingsCommands ?? throw new ArgumentNullException(nameof(settingsCommands));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
            this.localizationCommands =
                localizationCommands ?? throw new ArgumentNullException(nameof(localizationCommands));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiSettingsWindow> Create(UiSettingsWindow window)
        {
            CreatedCount++;
            return new UiSettingsPresenter(
                settingsReadModel,
                settingsCommands,
                localizationReadModel,
                localizationCommands);
        }
    }
}
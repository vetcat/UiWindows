using System;
using ProjectContext.Localization;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownRightPresenterFactory : IWindowPresenterFactory<UiDownRightWindow>
    {
        private readonly UiSettingsDemoLauncher settingsLauncher;
        private readonly ILocalizationReadModel localizationReadModel;

        public UiDownRightPresenterFactory(
            UiSettingsDemoLauncher settingsLauncher,
            ILocalizationReadModel localizationReadModel)
        {
            this.settingsLauncher = settingsLauncher ?? throw new ArgumentNullException(nameof(settingsLauncher));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiDownRightWindow> Create(UiDownRightWindow window)
        {
            CreatedCount++;
            return new UiDownRightPresenter(settingsLauncher.Show, localizationReadModel);
        }
    }
}

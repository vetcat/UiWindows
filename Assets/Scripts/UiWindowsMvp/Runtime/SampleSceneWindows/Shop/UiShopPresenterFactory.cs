using System;
using ProjectContext.Localization;
using ProjectContext.Shop;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiShopPresenterFactory : IWindowPresenterFactory<UiShopWindow>
    {
        private readonly IShopReadModel readModel;
        private readonly IShopCommands commands;
        private readonly ILocalizationReadModel localizationReadModel;

        public UiShopPresenterFactory(
            IShopReadModel readModel,
            IShopCommands commands,
            ILocalizationReadModel localizationReadModel)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiShopWindow> Create(UiShopWindow window)
        {
            CreatedCount++;
            return new UiShopPresenter(readModel, commands, localizationReadModel);
        }
    }
}

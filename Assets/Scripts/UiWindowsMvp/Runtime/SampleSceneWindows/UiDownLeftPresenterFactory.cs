using System;
using ProjectContext.Localization;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiDownLeftPresenterFactory : IWindowPresenterFactory<UiDownLeftWindow>
    {
        private readonly UiShopDemoLauncher shopLauncher;
        private readonly ILocalizationReadModel localizationReadModel;

        public UiDownLeftPresenterFactory(
            UiShopDemoLauncher shopLauncher,
            ILocalizationReadModel localizationReadModel)
        {
            this.shopLauncher = shopLauncher ?? throw new ArgumentNullException(nameof(shopLauncher));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiDownLeftWindow> Create(UiDownLeftWindow window)
        {
            CreatedCount++;
            return new UiDownLeftPresenter(shopLauncher.Show, localizationReadModel);
        }
    }
}

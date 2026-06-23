using System;
using ProjectContext.Localization;
using ProjectContext.Shop;
using ProjectContext.UiRequests;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiShopPresenterFactory : IWindowPresenterFactory<UiShopWindow>
    {
        private readonly IShopReadModel readModel;
        private readonly IShopCommands commands;
        private readonly ILocalizationReadModel localizationReadModel;
        private readonly IUiModalCommands modalCommands;
        private readonly IUiShopVisibilityCommands visibilityCommands;

        public UiShopPresenterFactory(
            IShopReadModel readModel,
            IShopCommands commands,
            ILocalizationReadModel localizationReadModel,
            IUiModalCommands modalCommands,
            IUiShopVisibilityCommands visibilityCommands)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
            this.modalCommands = modalCommands ?? throw new ArgumentNullException(nameof(modalCommands));
            this.visibilityCommands =
                visibilityCommands ?? throw new ArgumentNullException(nameof(visibilityCommands));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiShopWindow> Create(UiShopWindow window)
        {
            CreatedCount++;
            return new UiShopPresenter(
                readModel,
                commands,
                localizationReadModel,
                modalCommands,
                visibilityCommands,
                ResolveView);
        }

        private static UiShopView ResolveView(UiShopWindow window)
        {
            if (window == null)
            {
                return null;
            }

            return window.TryGetView(out var resolved) ? resolved : null;
        }
    }
}

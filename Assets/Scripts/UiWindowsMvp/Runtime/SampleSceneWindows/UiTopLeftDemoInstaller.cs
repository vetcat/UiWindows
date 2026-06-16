using CompositionRoot.Runtime;
using ProjectContext.Localization;
using ProjectContext.Player;
using ProjectContext.Settings;
using ProjectContext.Shop;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftDemoInstaller : MonoBehaviour, ICompositionInstaller
    {
        [SerializeField] private UiTopLeftView uiTopLeftViewPrefab;

        [SerializeField] private UiSettingsView uiSettingsViewPrefab;

        [SerializeField] private UiShopView uiShopViewPrefab;

        [SerializeField] private bool showOnStart = true;

        [SerializeField] private bool showSettingsOnStart;

        [SerializeField] private bool showShopOnStart;

        [SerializeField] private int healthCommandStep = UiTopLeftPresenter.DefaultHealthCommandStep;

        [SerializeField] private int xpCommandStep = UiTopLeftPresenter.DefaultXpCommandStep;

        public void Install(IServiceRegistry registry)
        {
            var playerSettings = PlayerSettings.Default;
            var playerService = new PlayerService(playerSettings);
            var gameSettingsService = new GameSettingsService();
            var localizationService = new LocalizationService();
            var shopService = new ShopService();
            var presenterFactory =
                new UiTopLeftPresenterFactory(
                    playerService,
                    playerService,
                    playerSettings,
                    healthCommandStep,
                    xpCommandStep);
            var settingsPresenterFactory =
                new UiSettingsPresenterFactory(
                    gameSettingsService,
                    gameSettingsService,
                    localizationService,
                    localizationService);
            var shopPresenterFactory =
                new UiShopPresenterFactory(
                    shopService,
                    shopService,
                    localizationService);

            registry.Register<IPlayerSettings>(playerSettings);
            registry.Register<IPlayerReadModel>(playerService);
            registry.Register<IPlayerCommands>(playerService);
            registry.Register<IPlayerService>(playerService);
            registry.Register<IGameSettingsReadModel>(gameSettingsService);
            registry.Register<IGameSettingsCommands>(gameSettingsService);
            registry.Register<IGameSettingsService>(gameSettingsService);
            registry.Register<ILocalizationReadModel>(localizationService);
            registry.Register<ILocalizationCommands>(localizationService);
            registry.Register<ILocalizationService>(localizationService);
            registry.Register<IShopReadModel>(shopService);
            registry.Register<IShopCommands>(shopService);
            registry.Register<IShopService>(shopService);
            registry.Register(presenterFactory);
            registry.Register(settingsPresenterFactory);
            registry.Register(shopPresenterFactory);
            registry.Register(new UiTopLeftDemoLauncher(uiTopLeftViewPrefab, presenterFactory));
            registry.Register(new UiSettingsDemoLauncher(uiSettingsViewPrefab, settingsPresenterFactory));
            registry.Register(new UiShopDemoLauncher(uiShopViewPrefab, shopPresenterFactory));
        }

        private void Start()
        {
            if (!showOnStart && !showSettingsOnStart && !showShopOnStart)
            {
                return;
            }

            var root = GetComponent<SceneCompositionRoot>();
            if (root == null)
            {
                throw new MissingComponentException(
                    $"{nameof(UiTopLeftDemoInstaller)} requires {nameof(SceneCompositionRoot)} on the same GameObject.");
            }

            if (showOnStart)
            {
                root.Services.Resolve<UiTopLeftDemoLauncher>().Show();
            }

            if (showSettingsOnStart)
            {
                root.Services.Resolve<UiSettingsDemoLauncher>().Show();
            }

            if (showShopOnStart)
            {
                root.Services.Resolve<UiShopDemoLauncher>().Show();
            }
        }
    }
}

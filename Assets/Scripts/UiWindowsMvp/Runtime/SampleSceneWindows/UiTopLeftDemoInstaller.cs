using CompositionRoot.Runtime;
using ProjectContext.Localization;
using ProjectContext.Player;
using ProjectContext.Settings;
using ProjectContext.Shop;
using ProjectContext.UiRequests;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftDemoInstaller : MonoBehaviour, ICompositionInstaller
    {
        [SerializeField] private UiTopLeftView uiTopLeftViewPrefab;

        [SerializeField] private UiTopRightView uiTopRightViewPrefab;

        [SerializeField] private UiDownRightView uiDownRightViewPrefab;

        [SerializeField] private UiDownLeftView uiDownLeftViewPrefab;

        [SerializeField] private UiSettingsView uiSettingsViewPrefab;

        [SerializeField] private UiShopView uiShopViewPrefab;

        [SerializeField] private UiModalView uiModalViewPrefab;

        [SerializeField] private UiHintsView uiHintsViewPrefab;

        [SerializeField] private UiFxView uiFxViewPrefab;

        [SerializeField] private UiObjectIndicatorView uiObjectIndicatorViewPrefab;

        [SerializeField] private bool showOnStart = true;

        [SerializeField] private bool showTopRightOnStart = true;

        [SerializeField] private bool showDownRightOnStart = true;

        [SerializeField] private bool showDownLeftOnStart = true;

        [SerializeField] private bool showObjectIndicatorOnStart = true;

        [SerializeField] private bool showSettingsOnStart;

        [SerializeField] private bool showShopOnStart;

        [SerializeField] private int healthCommandStep = UiTopLeftPresenter.DefaultHealthCommandStep;

        [SerializeField] private int xpCommandStep = UiTopLeftPresenter.DefaultXpCommandStep;

        public void Install(IServiceRegistry registry)
        {
            var playerSettings = PlayerSettings.Default;
            var modalService = new UiModalService();
            var feedbackService = new UiFeedbackService();
            var fxTargetRegistry = new UiFxTargetRegistry();
            var shopVisibilityState = new UiShopVisibilityState();
            var objectIndicatorTarget = new UiObjectIndicatorDemoTarget();
            var objectIndicatorScreenAdapter = new CameraWorldToScreenAdapter();
            var playerService = new PlayerService(playerSettings, feedbackService);
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
            var topRightPresenterFactory = new UiTopRightPresenterFactory(playerService, fxTargetRegistry);
            var settingsPresenterFactory =
                new UiSettingsPresenterFactory(
                    gameSettingsService,
                    gameSettingsService,
                    localizationService,
                    localizationService);
            var settingsLauncher = new UiSettingsDemoLauncher(uiSettingsViewPrefab, settingsPresenterFactory);
            var downRightPresenterFactory =
                new UiDownRightPresenterFactory(
                    settingsLauncher,
                    localizationService);
            var shopPresenterFactory =
                new UiShopPresenterFactory(
                    shopService,
                    shopService,
                    localizationService,
                    modalService,
                    shopVisibilityState);
            var shopLauncher = new UiShopDemoLauncher(uiShopViewPrefab, shopPresenterFactory);
            var downLeftPresenterFactory =
                new UiDownLeftPresenterFactory(
                    shopLauncher,
                    localizationService);
            var modalPresenterFactory = new UiModalPresenterFactory(modalService, modalService);
            var hintsPresenterFactory = new UiHintsPresenterFactory(feedbackService);
            var fxPresenterFactory = new UiFxPresenterFactory(feedbackService, fxTargetRegistry);
            var objectIndicatorPresenterFactory =
                new UiObjectIndicatorPresenterFactory(
                    playerService,
                    playerService,
                    playerSettings,
                    objectIndicatorTarget,
                    objectIndicatorScreenAdapter,
                    fxTargetRegistry);

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
            registry.Register<IUiModalReadModel>(modalService);
            registry.Register<IUiModalCommands>(modalService);
            registry.Register<IUiModalService>(modalService);
            registry.Register<IUiFeedbackReadModel>(feedbackService);
            registry.Register<IUiFeedbackCommands>(feedbackService);
            registry.Register<IUiFeedbackService>(feedbackService);
            registry.Register<IUiFxTargetResolver>(fxTargetRegistry);
            registry.Register<IUiFxTargetRegistry>(fxTargetRegistry);
            registry.Register<IUiShopVisibilityReadModel>(shopVisibilityState);
            registry.Register<IUiShopVisibilityCommands>(shopVisibilityState);
            registry.Register<IUiObjectIndicatorAnchor>(objectIndicatorTarget);
            registry.Register<IUiWorldToScreenAdapter>(objectIndicatorScreenAdapter);
            registry.Register(fxTargetRegistry);
            registry.Register(shopVisibilityState);
            registry.Register(objectIndicatorTarget);
            registry.Register(objectIndicatorScreenAdapter);
            registry.Register(presenterFactory);
            registry.Register(topRightPresenterFactory);
            registry.Register(settingsPresenterFactory);
            registry.Register(downRightPresenterFactory);
            registry.Register(downLeftPresenterFactory);
            registry.Register(shopPresenterFactory);
            registry.Register(modalPresenterFactory);
            registry.Register(hintsPresenterFactory);
            registry.Register(fxPresenterFactory);
            registry.Register(objectIndicatorPresenterFactory);
            registry.Register(new UiTopLeftDemoLauncher(uiTopLeftViewPrefab, presenterFactory));
            registry.Register(new UiTopRightDemoLauncher(uiTopRightViewPrefab, topRightPresenterFactory));
            registry.Register(settingsLauncher);
            registry.Register(new UiDownRightDemoLauncher(uiDownRightViewPrefab, downRightPresenterFactory));
            registry.Register(shopLauncher);
            registry.Register(
                new UiDownLeftDemoLauncher(uiDownLeftViewPrefab, downLeftPresenterFactory, shopVisibilityState));
            registry.Register(new UiModalDemoLauncher(uiModalViewPrefab, modalPresenterFactory, modalService));
            registry.Register(new UiHintsDemoLauncher(uiHintsViewPrefab, hintsPresenterFactory));
            registry.Register(new UiFxDemoLauncher(uiFxViewPrefab, fxPresenterFactory));
            registry.Register(new UiObjectIndicatorDemoLauncher(
                uiObjectIndicatorViewPrefab,
                objectIndicatorPresenterFactory));
        }

        private void Start()
        {
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

            if (showTopRightOnStart)
            {
                root.Services.Resolve<UiTopRightDemoLauncher>().Show();
            }

            if (showDownRightOnStart)
            {
                root.Services.Resolve<UiDownRightDemoLauncher>().Show();
            }

            root.Services.Resolve<UiDownLeftDemoLauncher>().Start();
            if (showDownLeftOnStart)
            {
                root.Services.Resolve<UiDownLeftDemoLauncher>().Show();
            }

            root.Services.Resolve<UiModalDemoLauncher>().Start();
            root.Services.Resolve<UiHintsDemoLauncher>().Show();
            root.Services.Resolve<UiFxDemoLauncher>().Show();

            if (showObjectIndicatorOnStart)
            {
                root.Services.Resolve<UiObjectIndicatorDemoLauncher>().Show();
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

using CompositionRoot.Runtime;
using ProjectContext.Player;
using UnityEngine;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftDemoInstaller : MonoBehaviour, ICompositionInstaller
    {
        [SerializeField] private UiTopLeftView uiTopLeftViewPrefab;

        [SerializeField] private bool showOnStart = true;

        [SerializeField] private int healthCommandStep = UiTopLeftPresenter.DefaultHealthCommandStep;

        [SerializeField] private int xpCommandStep = UiTopLeftPresenter.DefaultXpCommandStep;

        public void Install(IServiceRegistry registry)
        {
            var settings = PlayerSettings.Default;
            var playerService = new PlayerService(settings);
            var presenterFactory =
                new UiTopLeftPresenterFactory(playerService, playerService, settings, healthCommandStep, xpCommandStep);

            registry.Register<IPlayerSettings>(settings);
            registry.Register<IPlayerReadModel>(playerService);
            registry.Register<IPlayerCommands>(playerService);
            registry.Register<IPlayerService>(playerService);
            registry.Register(presenterFactory);
            registry.Register(new UiTopLeftDemoLauncher(uiTopLeftViewPrefab, presenterFactory));
        }

        private void Start()
        {
            if (!showOnStart)
            {
                return;
            }

            var root = GetComponent<SceneCompositionRoot>();
            if (root == null)
            {
                throw new MissingComponentException(
                    $"{nameof(UiTopLeftDemoInstaller)} requires {nameof(SceneCompositionRoot)} on the same GameObject.");
            }

            root.Services.Resolve<UiTopLeftDemoLauncher>().Show();
        }
    }
}
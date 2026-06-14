using System;
using ProjectContext.Player;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftPresenterFactory : IWindowPresenterFactory<UiTopLeftWindow>
    {
        private readonly IPlayerReadModel readModel;
        private readonly IPlayerCommands commands;
        private readonly IPlayerSettings settings;
        private readonly int healthCommandStep;
        private readonly int xpCommandStep;

        public UiTopLeftPresenterFactory(
            IPlayerReadModel readModel,
            IPlayerCommands commands,
            IPlayerSettings settings,
            int healthCommandStep = UiTopLeftPresenter.DefaultHealthCommandStep,
            int xpCommandStep = UiTopLeftPresenter.DefaultXpCommandStep)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.healthCommandStep = healthCommandStep;
            this.xpCommandStep = xpCommandStep;
        }

        public IWindowPresenter<UiTopLeftWindow> Create(UiTopLeftWindow window)
        {
            return new UiTopLeftPresenter(readModel, commands, settings, healthCommandStep, xpCommandStep);
        }
    }
}
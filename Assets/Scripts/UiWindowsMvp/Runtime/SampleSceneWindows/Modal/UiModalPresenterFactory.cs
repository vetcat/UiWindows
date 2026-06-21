using System;
using ProjectContext.UiRequests;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiModalPresenterFactory : IWindowPresenterFactory<UiModalWindow>
    {
        private readonly IUiModalReadModel readModel;
        private readonly IUiModalCommands commands;

        public UiModalPresenterFactory(IUiModalReadModel readModel, IUiModalCommands commands)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiModalWindow> Create(UiModalWindow window)
        {
            CreatedCount++;
            return new UiModalPresenter(readModel, commands);
        }
    }
}
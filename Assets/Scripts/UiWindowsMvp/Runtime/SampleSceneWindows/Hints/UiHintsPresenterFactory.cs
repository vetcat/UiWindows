using System;
using ProjectContext.UiRequests;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiHintsPresenterFactory : IWindowPresenterFactory<UiHintsWindow>
    {
        private readonly IUiFeedbackReadModel readModel;

        public UiHintsPresenterFactory(IUiFeedbackReadModel readModel)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiHintsWindow> Create(UiHintsWindow window)
        {
            CreatedCount++;
            return new UiHintsPresenter(readModel);
        }
    }
}
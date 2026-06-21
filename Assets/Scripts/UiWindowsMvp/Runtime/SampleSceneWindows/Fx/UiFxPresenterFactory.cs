using System;
using ProjectContext.UiRequests;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiFxPresenterFactory : IWindowPresenterFactory<UiFxWindow>
    {
        private readonly IUiFeedbackReadModel readModel;

        public UiFxPresenterFactory(IUiFeedbackReadModel readModel)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiFxWindow> Create(UiFxWindow window)
        {
            CreatedCount++;
            return new UiFxPresenter(readModel);
        }
    }
}
using System;
using ProjectContext.UiRequests;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiFxPresenterFactory : IWindowPresenterFactory<UiFxWindow>
    {
        private readonly IUiFeedbackReadModel readModel;
        private readonly IUiFxTargetResolver targetResolver;

        public UiFxPresenterFactory(IUiFeedbackReadModel readModel)
            : this(readModel, null)
        {
        }

        public UiFxPresenterFactory(IUiFeedbackReadModel readModel, IUiFxTargetResolver targetResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.targetResolver = targetResolver;
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiFxWindow> Create(UiFxWindow window)
        {
            CreatedCount++;
            return new UiFxPresenter(readModel, targetResolver);
        }
    }
}

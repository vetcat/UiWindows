using System;
using ProjectContext.Player;
using UiWindowsMvp.UIAdapter;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopRightPresenterFactory : IWindowPresenterFactory<UiTopRightWindow>
    {
        private readonly IPlayerReadModel readModel;
        private readonly IUiFxTargetRegistry fxTargetRegistry;

        public UiTopRightPresenterFactory(IPlayerReadModel readModel, IUiFxTargetRegistry fxTargetRegistry)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.fxTargetRegistry = fxTargetRegistry ?? throw new ArgumentNullException(nameof(fxTargetRegistry));
        }

        public int CreatedCount { get; private set; }

        public IWindowPresenter<UiTopRightWindow> Create(UiTopRightWindow window)
        {
            CreatedCount++;
            return new UiTopRightPresenter(readModel, fxTargetRegistry);
        }
    }
}

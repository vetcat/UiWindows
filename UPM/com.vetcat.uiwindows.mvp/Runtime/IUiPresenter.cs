using System;

namespace UiWindowsMvp.UIAdapter
{
    public interface IUiPresenter : IDisposable
    {
        void Initialize();

        void OnShowBegin(IUiShowScope showScope);

        void OnShowEnd();

        void OnHideBegin();

        void OnHideEnd();
    }
}
